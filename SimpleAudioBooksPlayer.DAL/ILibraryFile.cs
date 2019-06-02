using System;

namespace SimpleAudioBooksPlayer.DAL
{
    public interface ILibraryFile
    {
        string FileName { get; set; }
        string FilePath { get; set; }
        DateTime ModifyTime { get; set; }

        string ParentFolderName { get; set; }
        string ParentFolderPath { get; set; }

        int DbVersion { get; set; }
    }
}