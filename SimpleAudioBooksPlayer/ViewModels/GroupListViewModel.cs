using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.Sorters;
using SimpleAudioBooksPlayer.ViewModels.DataServer;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

namespace SimpleAudioBooksPlayer.ViewModels
{
    public class GroupListViewModel : ViewModelBase
    {
        private readonly FileGroupDataServer _server = FileGroupDataServer.Current;
        private readonly FileOpenPicker _coverPicker;

        private MusicSorterUi<FileGroupDTO> _currentSorter;
        private ClassItemDTO _currentClass;

        public readonly GroupListViewSettings Settings = GroupListViewSettings.Current;

        public GroupListViewModel()
        {
            Data = new ObservableCollection<FileGroupDTO>();

            this.LogByObject("初始化封面选取器");
            _coverPicker = new FileOpenPicker();
            _coverPicker.FileTypeFilter.Add(".png");
            _coverPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            this.LogByObject("初始化排序方法列表");
            SorterMembers = new List<MusicSorterUi<FileGroupDTO>>();
            SorterMembers.Add(new MusicSorterUi<FileGroupDTO>("Title", g => g));
            SorterMembers.Add(new MusicSorterUi<FileGroupDTO>("CreateDate", g => g.CreateTime));

            _server.DataLoaded += Server_DataLoaded;
            _server.DataAdded += Server_DataAdded;
            _server.DataUpdated += Server_DataUpdated;
            _server.ClassSeted += Server_ClassSeted;
            _server.DataRemoved += Server_DataRemoved;

            Data.CollectionChanged += Data_CollectionChanged;
        }

        public ObservableCollection<FileGroupDTO> Data { get; }
        public List<MusicSorterUi<FileGroupDTO>> SorterMembers { get; }
        public ClassItemDTO CurrentClass => _currentClass;

        private bool _isLibraryEmpty;

        public bool IsLibraryEmpty
        {
            get => _isLibraryEmpty;
            set => Set(ref _isLibraryEmpty, value);
        }

        private bool _isGroupEmpty;

        public bool IsGroupEmpty
        {
            get => _isGroupEmpty;
            set => Set(ref _isGroupEmpty, value);
        }

        public void RefreshData(ClassItemDTO classItem)
        {
            this.LogByObject("获取数据");
            Data.Clear();
            _currentClass = classItem;

            var source = _server.GetGroups(classItem);
            foreach (var groupDto in source)
                Data.Add(groupDto);

            Sort(SorterMembers[(int) Settings.SortMethod]);

            if (Settings.IsReverse)
                Reverse();
        }

        public void Sort(MusicSorterUi<FileGroupDTO> sorter)
        {
            this.LogByObject("排序数据");
            Settings.SortMethod = (GroupListSorterMember) SorterMembers.IndexOf(sorter);

            var list = Data.OrderBy(sorter.KeySelector.Invoke).ToList();
            for (var i = 0; i < list.Count; i++)
                Data.Move(Data.IndexOf(list[i]), i);

            _currentSorter = sorter;
        }

        public void Reverse()
        {
            this.LogByObject("倒序排序数据");
            Settings.IsReverse = !Settings.IsReverse;

            var list = Data.Reverse().ToList();
            for (var i = 0; i < list.Count; i++)
                Data.Move(Data.IndexOf(list[i]), i);
        }

        public async Task SetUpCover(FileGroupDTO groupDto)
        {
            this.LogByObject("启动封面选取器");
            var file = await _coverPicker.PickSingleFileAsync();
            if (file is null)
                return;

            this.LogByObject("开始设置封面");
            await _server.SetCover(groupDto, file);
        }

        public void CheckEmpty()
        {
            IsLibraryEmpty = false;
            IsGroupEmpty = false;

            if (!_server.Data.Any())
            {
                IsLibraryEmpty = true;
                return;
            }

            if (!Data.Any())
            {
                IsGroupEmpty = true;
            }
        }

        private void Server_DataLoaded(object sender, IEnumerable<FileGroupDTO> e)
        {
            if (_currentClass is null)
                return;

            RefreshData(_currentClass);

            CheckEmpty();
        }

        private void Server_DataAdded(object sender, IEnumerable<FileGroupDTO> e)
        {
            List<FileGroupDTO> needAdd = null;
            if (_currentClass == ClassListDataServer.All_ClassItem)
                needAdd = e.ToList();
            else
                needAdd = e.Where(g => g.ClassItem == _currentClass).ToList();

            if (!needAdd.Any())
                return;

            this.LogByObject("追加新数据");

            foreach (var fileGroupDto in needAdd)
                Data.Add(fileGroupDto);

            Sort(SorterMembers[(int)Settings.SortMethod]);

            if (Settings.IsReverse)
                Reverse();
        }

        private void Server_DataUpdated(object sender, IEnumerable<FileGroupDTO> e)
        {
            if (Settings.SortMethod != GroupListSorterMember.Name)
                return;

            Sort(SorterMembers[(int)Settings.SortMethod]);

            if (Settings.IsReverse)
                Reverse();
        }

        private void Server_ClassSeted(object sender, IEnumerable<FileGroupDTO> e)
        {
            if (_currentClass != ClassListDataServer.All_ClassItem)
                RefreshData(_currentClass);
        }

        private void Server_DataRemoved(object sender, IEnumerable<FileGroupDTO> e)
        {
            foreach (var fileGroupDto in Data.Where(e.Contains).ToList())
                Data.Remove(fileGroupDto);
        }

        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CheckEmpty();
        }
    }
}