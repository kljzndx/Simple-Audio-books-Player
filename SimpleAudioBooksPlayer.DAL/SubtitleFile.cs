using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SimpleAudioBooksPlayer.DAL
{
    public class SubtitleFile : ILibraryFile
    {
        public SubtitleFile()
        {
        }

        public SubtitleFile(FileGroup group, string filePath, DateTime modifyTime, int dbVersion)
        {
            GroupId = group.Index;
            FilePath = filePath;
            ModifyTime = modifyTime;
            DbVersion = dbVersion;

            FileName = filePath.TakeFileName();
            ParentFolderName = filePath.TakeParentFolderName();
            ParentFolderPath = filePath.TakeParentFolderPath();

            var pathParagraph = FileName.Split('.').ToList();
            pathParagraph.Remove(pathParagraph.Last());
            DisplayName = pathParagraph.Count == 1 ? pathParagraph.First() : String.Join(".", pathParagraph);
        }

        public int GroupId { get; set; }

        public string DisplayName { get; set; }
        public string FileName { get; set; }

        [Key]
        public string FilePath { get; set; }
        public DateTime ModifyTime { get; set; }

        public string ParentFolderName { get; set; }
        public string ParentFolderPath { get; set; }

        public int DbVersion { get; set; }
    }
}