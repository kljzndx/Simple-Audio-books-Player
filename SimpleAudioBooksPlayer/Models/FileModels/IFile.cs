
using System.Threading.Tasks;
using Windows.Storage;
using SimpleAudioBooksPlayer.Models.DTO;

namespace SimpleAudioBooksPlayer.Models.FileModels
{
    public interface IFile
    {
        FileGroupDTO Group { get; }
        string FileName { get; }
        string DisplayName { get; }
        
        Task<StorageFile> GetFileAsync();
    }
}