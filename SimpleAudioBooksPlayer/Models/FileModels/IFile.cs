
using System.Threading.Tasks;
using Windows.Storage;
using SimpleAudioBooksPlayer.Models.DTO;
using System;

namespace SimpleAudioBooksPlayer.Models.FileModels
{
    public interface IFile
    {
        FileGroupDTO Group { get; }
        string FileName { get; }
        string DisplayName { get; }
        
        Task<StorageFile> GetFileAsync(Action<IFile> notFoundErrorCallback);
    }
}