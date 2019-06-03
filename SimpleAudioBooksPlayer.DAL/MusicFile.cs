﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleAudioBooksPlayer.DAL
{
    public class MusicFile : ILibraryFile
    {
        public MusicFile()
        {
        }

        public MusicFile(string title, TimeSpan duration, string filePath, DateTime modifyTime, int dbVersion)
        {
            Title = title;
            Duration = duration;
            FilePath = filePath;
            ModifyTime = modifyTime;
            DbVersion = dbVersion;

            FileName = filePath.TakeFileName();
            ParentFolderName = filePath.TakeParentFolderName();
            ParentFolderPath = filePath.TakeParentFolderPath();
            Group = new GroupItem(ParentFolderName);
        }

        [ForeignKey("Index")]
        public GroupItem Group { get; set; }

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