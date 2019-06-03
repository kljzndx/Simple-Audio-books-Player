using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleAudioBooksPlayer.DAL.Factory
{
    public interface IFileFactory
    {
        Task<ILibraryFile> CreateByFile(StorageFile file, int dbVersion);
        Task<ILibraryFile> CreateByPath(string path, int dbVersion);
    }
}