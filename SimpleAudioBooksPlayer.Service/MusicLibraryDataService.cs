using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using HappyStudio.UwpToolsLibrary.Auxiliarys;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.DAL.Factory;

namespace SimpleAudioBooksPlayer.Service
{
    public class MusicLibraryDataService<TFile, TFileFactory>: IObservableDataService<TFile> where TFile: class, ILibraryFile where TFileFactory: class, IFileFactory<TFile>, new()
    {
        private const int DbVersion = 1;
        private static MusicLibraryDataService<TFile, TFileFactory> _theService;

        private readonly FileGroupDataService _groupService = FileGroupDataService.Current;

        private readonly ContextHelper<FilesContext, TFile> _helper = new ContextHelper<FilesContext, TFile>();
        private List<TFile> _source;
        private readonly LibraryFileScanner _scanner;
        private readonly TFileFactory _factory = new TFileFactory();

        private MusicLibraryDataService(StorageLibrary library, params string[] extensionNames)
        {
            _scanner = new LibraryFileScanner(library, extensionNames);
        }

        public event EventHandler<IEnumerable<TFile>> DataAdded;
        public event EventHandler<IEnumerable<TFile>> DataRemoved;
        public event EventHandler<IEnumerable<TFile>> DataUpdated;

        public async Task<List<TFile>> GetData()
        {
            if (_source is null)
                _source = await _helper.ToList();

            return _source;
        }

        public async Task ScanFiles()
        {
            if (_source is null)
                await GetData();

            //await _scanner.ScanByChangeTracker(async trackers =>
            //{
            //    List<TFile> needAdd = new List<TFile>(), needUpdate = new List<TFile>(), needRemove = new List<TFile>();

            //    foreach (var item in trackers)
            //    {
            //        switch (item.ChangeType)
            //        {
            //            case StorageLibraryChangeType.Created:
            //            case StorageLibraryChangeType.MovedIntoLibrary:
            //                if (_source.All(sr => sr.FilePath != item.Path))
            //                    needAdd.Add(await _factory.CreateByPath(item.Path, DbVersion));
            //                break;

            //            case StorageLibraryChangeType.Deleted:
            //            case StorageLibraryChangeType.MovedOutOfLibrary:
            //                if (_source.Any(sr => sr.FilePath == item.Path))
            //                    needRemove.Add(_source.Find(f => f.FilePath == item.Path));
            //                break;

            //            case StorageLibraryChangeType.ContentsChanged:
            //            case StorageLibraryChangeType.ContentsReplaced:
            //                if (_source.Any(sr => sr.FilePath == item.Path))
            //                    needUpdate.Add(await _factory.CreateByPath(item.Path, DbVersion));
            //                break;

            //            case StorageLibraryChangeType.MovedOrRenamed:
            //                if (_source.Any(sr => sr.FilePath == item.PreviousPath))
            //                    needRemove.Add(_source.Find(f => f.FilePath == item.PreviousPath));
                            
            //                if (_source.All(sr => sr.FilePath != item.Path))
            //                    needAdd.Add(await _factory.CreateByPath(item.Path, DbVersion));
            //                break;
            //        }
            //    }

            //    if (needAdd.Any())
            //        await AddRange(needAdd);

            //    if (needUpdate.Any())
            //        await UpdateRange(needUpdate);

            //    if (needRemove.Any())
            //        await RemoveRange(needRemove);
            //});

            var allFilePath = new List<string>();

            await _scanner.ScanByFolder(async files =>
            {
                var fileList = files.ToList();
                var filePaths = fileList.Select(f => f.Path).ToList();
                allFilePath.AddRange(filePaths);

                {
                    var groupData = await _groupService.GetData();
                    var folderPaths = filePaths.Select(p => p.TakeParentFolderPath()).Distinct()
                        .Where(fp => groupData.All(g => g.FolderPath != fp)).ToList();

                    if (folderPaths.Any())
                        await _groupService.AddRange(folderPaths);
                }

                var groups = await _groupService.GetData();
                List<TFile> needAdd = new List<TFile>(), needUpdate = new List<TFile>();
                var oldData = _source.Where(src => src.DbVersion != DbVersion && fileList.Any(sf => sf.Path == src.FilePath));

                foreach (var file in fileList)
                {
                    // 添加数据
                    {
                        if (_source.All(f => f.FilePath != file.Path))
                            needAdd.Add(await _factory.CreateByFile(file, DbVersion, groups.Find(g => g.FolderPath == file.Path.TakeParentFolderPath())));
                    }

                    // 更新数据
                    {
                        var basicProp = await file.GetBasicPropertiesAsync();
                        var isNeed = _source.Any(d =>
                            d.FilePath == file.Path && (!d.ModifyTime.Equals(basicProp.DateModified.DateTime) ||
                            oldData.Any(od => od == d)));

                        if (isNeed)
                            needUpdate.Add(await _factory.CreateByFile(file, DbVersion, groups.Find(g => g.FolderPath == file.Path.TakeParentFolderPath())));
                    }
                }

                if (needAdd.Any())
                    await AddRange(needAdd);

                if (needUpdate.Any())
                    await UpdateRange(needUpdate);
            });

            var needRemoveFiles = new List<TFile>();
            needRemoveFiles.AddRange(_source.Where(src => allFilePath.All(fp => fp != src.FilePath)));

            if (needRemoveFiles.Any())
            {
                await RemoveRange(needRemoveFiles);

                var groups = await _groupService.GetData();
                var needRemoveGroupPaths = groups.Where(g => _source.All(src => src.ParentFolderPath != g.FolderPath))
                    .Select(g => g.FolderPath).ToList();

                if (needRemoveGroupPaths.Any())
                    await _groupService.RemoveRange(needRemoveGroupPaths);
            }
        }

        private async Task AddRange(IEnumerable<TFile> datas)
        {
            var list = datas.ToList();

            await _helper.AddRange(list);
            _source.AddRange(list);
            DataAdded?.Invoke(this, list);
        }

        private async Task UpdateRange(IEnumerable<TFile> datas)
        {
            var list = datas.ToList();

            await _helper.UpdateRange(list);

            _source.RemoveAll(lf => list.Any(f => f.FilePath == lf.FilePath));
            _source.AddRange(list);

            DataUpdated?.Invoke(this, list);
        }

        private async Task RemoveRange(IEnumerable<TFile> datas)
        {
            var list = datas.ToList();

            await _helper.RemoveRange(list);
            _source.RemoveAll(list.Contains);
            DataRemoved?.Invoke(this, list);
        }

        public static async Task<MusicLibraryDataService<TFile, TFileFactory>> GetService(params string[] extensionNames)
        {
            if (_theService is null)
                _theService = new MusicLibraryDataService<TFile, TFileFactory>(await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music), extensionNames);

            return _theService;
        }
    }
}
