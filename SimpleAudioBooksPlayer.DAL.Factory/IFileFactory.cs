﻿using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleAudioBooksPlayer.DAL.Factory
{
    public interface IFileFactory<TFile> where TFile: class, ILibraryFile
    {
        Task<TFile> CreateByFile(StorageFile file, int dbVersion);
        Task<TFile> CreateByPath(string path, int dbVersion);
    }
}