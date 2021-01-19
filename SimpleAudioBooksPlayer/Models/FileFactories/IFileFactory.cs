using System.Threading.Tasks;
using Windows.Storage;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.FileModels;

namespace SimpleAudioBooksPlayer.Models.FileFactories
{
    public interface IFileFactory<TFile> where TFile: class, IFile
    {
        Task<TFile> CreateByFile(StorageFile file, FileGroupDTO group);
        Task<TFile> CreateByPath(string path, FileGroupDTO group);
    }
}