using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.Models.DTO;

namespace SimpleAudioBooksPlayer.Models
{
    /// <summary>
    /// 书籍分组类
    /// </summary>
    public class FileGroupBox
    {
        public FileGroupBox(string path, IEnumerable<FileGroupDTO> list)
        {
            Name = path.TakeFolderName();
            Path = path;
            Groups = new ObservableCollection<FileGroupDTO>(list);
            Groups.CollectionChanged -= Groups_CollectionChanged;
            Groups.CollectionChanged += Groups_CollectionChanged;
        }

        public static event EventHandler HasBeenClear;

        public string Name { get; }
        public string Path { get; }
        public ObservableCollection<FileGroupDTO> Groups { get; }

        private void Groups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!Groups.Any())
            {
                HasBeenClear?.Invoke(this, EventArgs.Empty);
                Groups.CollectionChanged -= Groups_CollectionChanged;
            }
        }
    }
}