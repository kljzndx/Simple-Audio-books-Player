﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleAudioBooksPlayer.DAL
{
    public class LyricFile : ILibraryFile
    {
        public LyricFile()
        {
        }

        public LyricFile(string filePath, DateTime modifyTime, int dbVersion)
        {
            FilePath = filePath;
            ModifyTime = modifyTime;
            DbVersion = dbVersion;

            FileName = filePath.TakeFileName();
            ParentFolderName = filePath.TakeParentFolderName();
            ParentFolderPath = filePath.TakeParentFolderPath();
        }

        public string FileName { get; set; }
        [Key]
        public string FilePath { get; set; }
        public DateTime ModifyTime { get; set; }

        public string ParentFolderName { get; set; }
        public string ParentFolderPath { get; set; }

        public int DbVersion { get; set; }
    }
}