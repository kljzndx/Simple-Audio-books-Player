﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleAudioBooksPlayer.DAL
{
    public class FileGroup
    {
        public FileGroup()
        {
        }

        public FileGroup(string folderPath)
        {
            FolderPath = folderPath;
            Name = FolderPath.TakeFolderName();
            CreateTime = DateTime.Now;
        }

        public FileGroup(int index, string name, string folderPath, bool hasCover, DateTime time)
        {
            Index = index;
            Name = name;
            FolderPath = folderPath;
            HasCover = hasCover;
            CreateTime = time;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Index { get; set; }
        public string Name { get; set; }
        public string FolderPath { get; set; }
        public bool HasCover { get; set; }
        public DateTime CreateTime { get; set; }
    }
}