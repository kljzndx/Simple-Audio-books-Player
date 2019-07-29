using System;
using Windows.UI;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.ViewModels.Extensions;

namespace SimpleAudioBooksPlayer.Models.DTO
{
    public class ClassItemDTO : ObservableObject
    {
        private bool _isSelected;
        private string _name;
        private Color _backgroundColor;

        public ClassItemDTO(ClassItem source)
        {
            Index = source.Index;
            Name = source.Name;
            CreateDate = source.CreateDate;

            if (String.IsNullOrWhiteSpace(source.BackgroundColor))
                _backgroundColor = Colors.Transparent;
            else
            {
                var argb = source.BackgroundColor.Split(',');
                _backgroundColor = Color.FromArgb(Byte.Parse(argb[0]), Byte.Parse(argb[1]), Byte.Parse(argb[2]), Byte.Parse(argb[3]));
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public int Index { get; }

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public DateTime CreateDate { get; }

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => Set(ref _backgroundColor, value);
        }

        public ClassItem ToTableModel()
        {
            return new ClassItem(Index, Name, BackgroundColor.ToArgbString());
        }
    }
}