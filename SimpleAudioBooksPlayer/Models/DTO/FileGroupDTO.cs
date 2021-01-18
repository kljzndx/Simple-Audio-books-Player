using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using HappyStudio.UwpToolsLibrary.Auxiliarys;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.Models.Sorters;
using SimpleAudioBooksPlayer.ViewModels.DataServer;
using SimpleAudioBooksPlayer.ViewModels.Events;

namespace SimpleAudioBooksPlayer.Models.DTO
{
    public class FileGroupDTO : ObservableObject, IEquatable<FileGroupDTO>, IComparable, IComparable<FileGroupDTO>
    {
        private static BitmapImage lightCover;
        private static BitmapImage darkCover;

        private static BitmapImage defaultImage;
        private static ApplicationTheme currentTheme;

        private WeakReference<BitmapImage> _cover;

        private string _name;
        private bool _hasCover;

        public FileGroupDTO(FileGroup source)
        {
            Index = source.Index;
            ClassItem = ClassListDataServer.Current.Data.FirstOrDefault(c => c.Index == source.ClassId);
            _name = source.Name;
            FolderPath = source.FolderPath;
            _hasCover = source.HasCover;
            CreateTime = source.CreateTime;

            ThemeChangeEvent.ThemeChanged += ThemeChangeEvent_ThemeChanged;
        }

        public event EventHandler<object> CoverChanged;

        public int Index { get; }
        public ClassItemDTO ClassItem { get; set; }
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        public string FolderPath { get; }
        public bool HasCover
        {
            get => _hasCover;
            set => Set(ref _hasCover, value);
        }
        public DateTime CreateTime { get; set; }

        public async Task<StorageFile> GetCoverFile()
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(FolderPath);
            return (await folder.TryGetItemAsync("cover.png")) as StorageFile;
        }
        
        public async Task<BitmapImage> GetCover()
        {
            BitmapImage cover = null;
            _cover?.TryGetTarget(out cover);
            
            if (cover is null && HasCover)
            {
                if (await GetCoverFile() is StorageFile file)
                {
                    cover = new BitmapImage();
                    cover.SetSource(await file.OpenAsync(FileAccessMode.Read));
                    _cover = new WeakReference<BitmapImage>(cover);
                    return cover;
                }
                else
                {
                    MessageBox.ShowAsync($"Cannot find cover file of \"{Name}\" book", $"找不到 \"{Name}\" 书籍的封面文件", "Done");
                }
            }
            
            cover = defaultImage;
            _cover = new WeakReference<BitmapImage>(cover);
            return cover;
        }

        public void SetCover(BitmapImage cover)
        {
            _cover = new WeakReference<BitmapImage>(cover);
            HasCover = true;
            CoverChanged?.Invoke(this, null);
        }

        public void Update(FileGroup source)
        {
            if (source.Index != Index)
                throw new Exception("新的文件组跟当前文件组没半毛钱关系");

            Name = source.Name;
            HasCover = source.HasCover;
        }

        public FileGroup ToTableModel()
        {
            return new FileGroup(Index, (ClassItem?.Index) ?? -1, Name, FolderPath, HasCover, CreateTime);
        }

        public bool Equals(FileGroupDTO other)
        {
            if (other is null)
                return false;

            return Index == other.Index;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FileGroupDTO) obj);
        }

        public override int GetHashCode()
        {
            return Index;
        }

        public static async Task InitAssets()
        {
            currentTheme = Application.Current.RequestedTheme;

            var lightFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/CoverIcon.theme-light.png"));
            var darkFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/CoverIcon.theme-dark.png"));

            var lb = new BitmapImage();
            lb.SetSource(await lightFile.OpenAsync(FileAccessMode.Read));
            var db = new BitmapImage();
            db.SetSource(await darkFile.OpenAsync(FileAccessMode.Read));

            lightCover = lb;
            darkCover = db;

            defaultImage = currentTheme == ApplicationTheme.Light ? lightCover : darkCover;
        }

        private void ThemeChangeEvent_ThemeChanged(object sender, ApplicationTheme e)
        {
            if (HasCover)
                return;

            if (currentTheme != e)
            {
                currentTheme = e;
                defaultImage = currentTheme == ApplicationTheme.Light ? lightCover : darkCover;
            }

            _cover = new WeakReference<BitmapImage>(defaultImage);
            CoverChanged?.Invoke(this, null);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(this, obj)) return 0;
            if (ReferenceEquals(null, obj)) return 1;
            if (obj.GetType() != typeof(FileGroupDTO)) return 1;

            return CompareTo((FileGroupDTO) obj);
        }

        public int CompareTo(FileGroupDTO other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;

            return SystemStringSorter.Compare(Name, other.Name);
        }
    }
}