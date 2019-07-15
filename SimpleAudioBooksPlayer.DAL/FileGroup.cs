using System;
using System.ComponentModel;
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
            ClassId = -1;
            Name = FolderPath.TakeFolderName();
            CreateTime = DateTime.Now;
        }

        public FileGroup(int index, int classId, string name, string folderPath, bool hasCover, DateTime time)
        {
            Index = index;
            ClassId = classId;
            Name = name;
            FolderPath = folderPath;
            HasCover = hasCover;
            CreateTime = time;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Index { get; set; }
        public int ClassId { get; set; }
        public string Name { get; set; }
        public string FolderPath { get; set; }
        public bool HasCover { get; set; }
        public DateTime CreateTime { get; set; }
    }
}