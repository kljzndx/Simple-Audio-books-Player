using System;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.DAL;

namespace SimpleAudioBooksPlayer.Models.DTO
{
    public class FileGroupDTO : ObservableObject, IEquatable<FileGroupDTO>
    {
        private string _name;
        private bool _hasCover;

        public FileGroupDTO(FileGroup source)
        {
            Index = source.Index;
            _name = source.Name;
            FolderPath = source.FolderPath;
            _hasCover = source.HasCover;
            CreateTime = source.CreateTime;
        }

        public int Index { get; }
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        public string FolderPath { get; }
        public bool HasCover
        {
            get => _hasCover;
            set => Set(ref _hasCover, value);
        }
        public DateTime CreateTime { get; set; }

        public bool Equals(FileGroupDTO other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Index == other.Index;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FileGroupDTO) obj);
        }

        public override int GetHashCode()
        {
            return Index;
        }
    }
}