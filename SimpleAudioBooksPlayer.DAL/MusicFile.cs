using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleAudioBooksPlayer.DAL
{
    public class MusicFile : ILibraryFile
    {
        public MusicFile()
        {
        }

        public MusicFile(FileGroup group, uint trackNumber, string title, TimeSpan duration, string filePath, DateTime modifyTime, int dbVersion)
        {
            GroupId = group.Index;
            TrackNumber = trackNumber;
            Duration = duration;
            FilePath = filePath;
            ModifyTime = modifyTime;
            DbVersion = dbVersion;

            FileName = filePath.TakeFileName();
            ParentFolderName = filePath.TakeParentFolderName();
            ParentFolderPath = filePath.TakeParentFolderPath();
            Title = String.IsNullOrWhiteSpace(title) ? FileName : title;
        }

        public int GroupId { get; set; }

        public uint TrackNumber { get; set; }
        public string Title { get; set; }
        public TimeSpan Duration { get; set; }

        public string FileName { get; set; }
        [Key]
        public string FilePath { get; set; }
        public DateTime ModifyTime { get; set; }

        public string ParentFolderName { get; set; }
        public string ParentFolderPath { get; set; }

        public int DbVersion { get; set; }
    }
}