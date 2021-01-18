using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.Models.DTO;

namespace SimpleAudioBooksPlayer.Models.FileModels
{
    public class LibraryFileBase : ObservableObject, IFile
    {
        private WeakReference<StorageFile> _weakFile;
        
        protected LibraryFileBase(FileGroupDTO groupDto, string fileName)
        {
            Group = groupDto;
            FileName = fileName;
            
            var pathParagraph = FileName.Split('.').ToList();
            pathParagraph.Remove(pathParagraph.Last());
            DisplayName = pathParagraph.Count == 1 ? pathParagraph.First() : String.Join(".", pathParagraph);
        }
        
        public FileGroupDTO Group { get; }
        public string FileName { get; }
        public string FilePath => Group.FolderPath + '\\' + FileName;
        public string DisplayName { get; }
        
        public async Task<StorageFile> GetFileAsync()
        {
            StorageFile file = null;
            _weakFile?.TryGetTarget(out file);

            if (file is null)
            {
                file = await StorageFile.GetFileFromPathAsync(FilePath);
                _weakFile = new WeakReference<StorageFile>(file);
            }

            return file;
        }
    }
}