using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using SimpleAudioBooksPlayer.Models.DTO;
using System.IO;
using HappyStudio.UwpToolsLibrary.Auxiliarys;

namespace SimpleAudioBooksPlayer.Models.FileModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LibraryFileBase : ObservableObject, IFile
    {
        private WeakReference<StorageFile> _weakFile;
        private FileGroupDTO _group;
        
        protected LibraryFileBase(FileGroupDTO groupDto, string fileName)
        {
            _group = groupDto;
            FileName = fileName;
            
            var pathParagraph = FileName.Split('.').ToList();
            pathParagraph.Remove(pathParagraph.Last());
            DisplayName = pathParagraph.Count == 1 ? pathParagraph.First() : String.Join(".", pathParagraph);
        }
        
        public FileGroupDTO Group
        {
            get => _group;
            set
            {
                if (_group == null)
                    _group = value;
            }
        }
        
        [JsonProperty]
        public string FileName { get; }
        
        public string FilePath => Group.FolderPath + '\\' + FileName;
        public string DisplayName { get; }
        
        public async Task<StorageFile> GetFileAsync()
        {
            StorageFile file = null;
            _weakFile?.TryGetTarget(out file);

            if (file is null)
            {
                try
                {
                    file = await StorageFile.GetFileFromPathAsync(FilePath);
                    _weakFile = new WeakReference<StorageFile>(file);

                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.ShowAsync(ex.Message + Environment.NewLine + Environment.NewLine + FilePath, "Done");
                    return null;
                }
            }

            return file;
        }
    }
}