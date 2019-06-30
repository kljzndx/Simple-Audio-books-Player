﻿using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.DAL;

namespace SimpleAudioBooksPlayer.Models.DTO
{
    public class FileGroupDTO : ObservableObject, IEquatable<FileGroupDTO>
    {
        private const string DefaultCoverUri = "ms-appx:///Assets/CoverIcon.png";
        private const string CustomCoverUri = "ms-appdata:///local/cover/";

        private WeakReference<BitmapImage> _cover;

        private string _name;
        private bool _hasCover;

        public FileGroupDTO(FileGroup source)
        {
            Index = source.Index;
            _name = source.Name;
            FolderPath = source.FolderPath;
            _hasCover = source.HasCover;
            CreateTime = source.CreateTime;
        }

        public int Index { get; }
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

        public async Task<BitmapImage> GetCover()
        {
            BitmapImage cover = null;
            _cover?.TryGetTarget(out cover);
            if (cover is null)
            {
                if (HasCover)
                {
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(CustomCoverUri+$"{Index}.png"));
                    cover = new BitmapImage();
                    cover.SetSource(await file.OpenAsync(FileAccessMode.Read));
                    _cover = new WeakReference<BitmapImage>(cover);
                }
                else
                {
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(DefaultCoverUri));
                    cover = new BitmapImage();
                    cover.SetSource(await file.OpenAsync(FileAccessMode.Read));
                    _cover = new WeakReference<BitmapImage>(cover);
                }
            }
            return cover;
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
    }
}