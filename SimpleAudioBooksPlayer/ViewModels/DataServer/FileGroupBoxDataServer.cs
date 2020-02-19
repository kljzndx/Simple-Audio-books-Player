using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.Models;
using SimpleAudioBooksPlayer.Models.DTO;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public class FileGroupBoxDataServer : IDataServer<FileGroupBox, FileGroupBox>
    {
        public static readonly FileGroupBoxDataServer Current = new FileGroupBoxDataServer();

        private readonly FileGroupDataServer _fileGroupDataServer = FileGroupDataServer.Current;

        private FileGroupBoxDataServer()
        {
            Data = new ObservableCollection<FileGroupBox>();
        }

        public bool IsInit { get; private set; }
        public ObservableCollection<FileGroupBox> Data { get; }

        public event EventHandler<IEnumerable<FileGroupBox>> DataLoaded;
        public event EventHandler<IEnumerable<FileGroupBox>> DataAdded;
        public event EventHandler<IEnumerable<FileGroupBox>> DataRemoved;

        public async Task Init()
        {
            if (_fileGroupDataServer.IsInit)
                await _fileGroupDataServer.Init();

            if (IsInit)
                return;

            IsInit = true;
            AutoGroup(_fileGroupDataServer.GetGroups(ClassListDataServer.Unspecified_ClassItem));

            _fileGroupDataServer.DataAdded += FileGroupDataServer_DataAdded;
            _fileGroupDataServer.DataRemoved += FileGroupDataServer_DataRemoved;
            _fileGroupDataServer.ClassSeted += FileGroupDataServer_ClassSeted;
            FileGroupBox.HasBeenClear += ClassGroup_HasBeenClear;

            DataLoaded?.Invoke(this, Data.ToList());
        }

        private void AutoGroup(IList<FileGroupDTO> groups)
        {
            this.LogByObject("分析书籍");
            var cls = groups.GroupBy(g => g.FolderPath.TakeParentFolderPath());
            bool hasAny = Data.Any();

            List<FileGroupBox> additionList = new List<FileGroupBox>();

            foreach (var cl in cls)
            {
                FileGroupBox classGroupItem = null;
                if (hasAny)
                    classGroupItem = Data.FirstOrDefault(cg => cg.Path == cl.Key);

                if (classGroupItem == null)
                {
                    this.LogByObject("创建书籍盒");
                    classGroupItem = new FileGroupBox(cl.Key, cl);
                    Data.Add(classGroupItem);
                    additionList.Add(classGroupItem);
                }

                this.LogByObject("给盒子里添加书籍");
                foreach (var groupDto in cl)
                    if (!classGroupItem.Groups.Contains(groupDto))
                        classGroupItem.Groups.Add(groupDto);
            }

            if (additionList.Any())
                DataAdded?.Invoke(this, additionList);
        }

        private void FileGroupDataServer_DataAdded(object sender, IEnumerable<FileGroupDTO> e)
        {
            AutoGroup(e.ToList());
        }

        private void FileGroupDataServer_DataRemoved(object sender, IEnumerable<FileGroupDTO> e)
        {
            var list = e.Where(fg => fg.ClassItem == ClassListDataServer.Unspecified_ClassItem);
            foreach (var groupDto in list)
                Data.FirstOrDefault(cg => cg.Groups.Contains(groupDto))?.Groups.Remove(groupDto);
        }

        private void FileGroupDataServer_ClassSeted(object sender, IEnumerable<FileGroupDTO> e)
        {
            var list = e.ToList();

            if (list.FirstOrDefault()?.ClassItem == ClassListDataServer.Unspecified_ClassItem)
                AutoGroup(list);
            else foreach (var groupDto in list)
                Data.FirstOrDefault(cg => cg.Groups.Contains(groupDto))?.Groups.Remove(groupDto);
        }

        private void ClassGroup_HasBeenClear(object sender, EventArgs e)
        {
            Data.Remove((FileGroupBox) sender);
        }
    }
}