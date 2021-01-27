using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml.Media.Imaging;
using HappyStudio.UwpToolsLibrary.Auxiliarys.Files.Scanners;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.FileFactories;
using SimpleAudioBooksPlayer.Service;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;
using Nito.AsyncEx;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public class FileGroupDataServer : IDataServer<FileGroupDTO, FileGroupDTO>
    {
        public static readonly FileGroupDataServer Current = new FileGroupDataServer();

        private FileGroupDataService _service;
        private LibraryFolderScanner _folderScanner;
        private QueryOptions _queryOptions;
        private readonly AsyncLock _mutex = new AsyncLock();

        public FileGroupDataServer()
        {
            Data = new ObservableCollection<FileGroupDTO>();

            _queryOptions = new QueryOptions(CommonFileQuery.OrderByName, MusicFileScanner.ExtensionName.Split(' ').Select(s => "." + s).ToArray());
            _queryOptions.FolderDepth = FolderDepth.Shallow;
        }

        public bool IsInit { get; private set; }
        public ObservableCollection<FileGroupDTO> Data { get; }

        public event EventHandler<IEnumerable<FileGroupDTO>> DataLoaded;
        public event EventHandler<IEnumerable<FileGroupDTO>> DataAdded;
        public event EventHandler<IEnumerable<FileGroupDTO>> DataRemoved;
        public event EventHandler<IEnumerable<FileGroupDTO>> DataUpdated;
        public event EventHandler<IEnumerable<FileGroupDTO>> ClassSeted;

        public async Task Init()
        {
            if (IsInit)
                return;

            IsInit = true;
            _service = FileGroupDataService.Current;
            _folderScanner = new LibraryFolderScanner(await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music));

            this.LogByObject("初始化组资源");
            await FileGroupDTO.InitAssets();

            this.LogByObject("初始化文件组服务器");
            var source = await _service.GetData();
            var data = source.Select(g => new FileGroupDTO(g)).ToList();

            await CheckoutCover(data);
            
            foreach (var item in data)
                Data.Add(item);

            DataLoaded?.Invoke(this, data);
            _service.DataAdded += Service_DataAdded;
            _service.DataRemoved += Service_DataRemoved;
            _service.DataUpdated += Service_DataUpdated;

            ClassListDataServer.Current.DataRemoved += ClassDataServer_DataRemoved;
        }

        public async Task ScanFolders()
        {
            using (await _mutex.LockAsync())
            {
                var allPaths = new List<string>();

                await _folderScanner.ScanByFolderQuery(async folders =>
                {
                    var list = new List<string>();
                    foreach (var folder in folders)
                    {
                        if (folder.Name == ".bookinfo")
                            continue;

                        var fileQueryResult = folder.CreateFileQueryWithOptions(_queryOptions);
                        var count = await fileQueryResult.GetItemCountAsync();

                        if (count > 0)
                            list.Add(folder.Path);
                    }

                    if (list.Any())
                    {
                        await _service.IntelligentAddRange(list);
                        allPaths.AddRange(list);
                    }
                });

                await _service.IntelligentRemoveRange(allPaths);
            }
        }

        public async Task CheckoutCover(List<FileGroupDTO> source)
        {
            if (GroupListViewSettings.Current.IsCheckCover)
                return;
            
            List<FileGroupDTO> list = source.Where(g => g.HasCover).ToList();
            bool notFoundCover = false;
            
            foreach (var groupDto in list.Take(3))
            {
                notFoundCover = await groupDto.GetCoverFile() == null;
                if (notFoundCover)
                    break;
            }
            
            if (notFoundCover)
            {
                foreach (var groupDto in list)
                {
                    groupDto.HasCover = false;
                    await _service.SetCover(groupDto.Index, false);
                }
            }

            GroupListViewSettings.Current.IsCheckCover = true;
        }

        public List<FileGroupDTO> GetGroups(ClassItemDTO classItem)
        {
            if (classItem == ClassListDataServer.All_ClassItem)
                return Data.ToList();
            else
                return Data.Where(g => g.ClassItem == classItem).ToList();
        }

        public async Task SetClass(FileGroupDTO groupDto, ClassItemDTO classItem)
        {
            if (classItem == ClassListDataServer.All_ClassItem)
                return;

            this.LogByObject("正在设置分类");
            groupDto.ClassItem = classItem;
            await _service.SetClass(groupDto.Index, classItem.Index);
            ClassSeted?.Invoke(this, new[] {groupDto});
        }

        public async Task SetClass(IList<FileGroupDTO> groupDtoList, ClassItemDTO classItem)
        {
            if (classItem == ClassListDataServer.All_ClassItem)
                return;
            
            this.LogByObject("正在批量设置分类");

            foreach (var fileGroupDto in groupDtoList)
                fileGroupDto.ClassItem = classItem;

            await _service.SetClass(groupDtoList.Select(g => g.Index), classItem.Index);
            ClassSeted?.Invoke(this, groupDtoList);
        }

        public async Task Rename(FileGroupDTO groupDto, string newName)
        {
            this.LogByObject("正在重命名 组数据");
            groupDto.Name = newName;
            await _service.RenameGroup(groupDto.Index, newName);
            DataUpdated?.Invoke(this, new[] {groupDto});
        }

        public async Task SetCover(FileGroupDTO groupDto, StorageFile file)
        {
            this.LogByObject("正在设置封面");
            var folder = await StorageFolder.GetFolderFromPathAsync(groupDto.FolderPath);
            var bitmapFile = await file.CopyAsync(folder, $"cover.png", NameCollisionOption.ReplaceExisting);

            var bi = new BitmapImage();
            bi.SetSource(await bitmapFile.OpenAsync(FileAccessMode.Read));
            groupDto.SetCover(bi);

            await _service.SetCover(groupDto.Index);
            DataUpdated?.Invoke(this, new[] {groupDto});
        }

        private void Service_DataAdded(object sender, IEnumerable<FileGroup> e)
        {
            this.LogByObject("正在添加 组数据");
            var needAdd = e.Select(g => new FileGroupDTO(g)).ToList();
            foreach (var group in needAdd)
                Data.Add(group);

            DataAdded?.Invoke(this, needAdd);
        }

        private void Service_DataRemoved(object sender, IEnumerable<FileGroup> e)
        {
            this.LogByObject("正在移除 组数据");
            var needRemove = new List<FileGroupDTO>(Data.Where(src => e.Any(g => g.Index == src.Index)));
            foreach (var groupDto in needRemove)
                Data.Remove(groupDto);

            DataRemoved?.Invoke(this, needRemove);
        }

        private void Service_DataUpdated(object sender, IEnumerable<FileGroup> e)
        {
            this.LogByObject("正在更新 组数据");
            var list = e.ToList();
            foreach (var fileGroup in list)
                Data.First(g => g.Index == fileGroup.Index).Update(fileGroup);
        }

        private async void ClassDataServer_DataRemoved(object sender, IEnumerable<ClassItemDTO> e)
        {
            this.LogByObject("正在重置 组数据 的分类设置");
            var needReset = Data.Where(g => e.Any(c => c == g.ClassItem)).ToList();
            foreach (var groupDto in needReset)
                groupDto.ClassItem = ClassListDataServer.Unspecified_ClassItem;

            await _service.SetClass(needReset.Select(g => g.Index), -1);

            ClassSeted?.Invoke(this, needReset);
        }
    }
}