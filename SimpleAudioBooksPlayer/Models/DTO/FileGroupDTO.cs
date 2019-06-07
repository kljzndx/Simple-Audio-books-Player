using System;
using SimpleAudioBooksPlayer.DAL;

namespace SimpleAudioBooksPlayer.Models.DTO
{
    public class FileGroupDTO : IEquatable<FileGroupDTO>
    {
        public FileGroupDTO(FileGroup source)
        {
            Index = source.Index;
            Name = source.Name;
            FolderPath = source.FolderPath;
            HasCover = source.HasCover;
            CreateTime = source.CreateTime;
        }

        public int Index { get; }
        public string Name { get; }
        public string FolderPath { get; }
        public bool HasCover { get; }
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