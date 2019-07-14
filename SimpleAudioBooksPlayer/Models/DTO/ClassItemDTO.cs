using System;
using Windows.UI;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.DAL;

namespace SimpleAudioBooksPlayer.Models.DTO
{
    public class ClassItemDTO : ObservableObject
    {
        private string _name;
        private Color _backgroundColor;

        public ClassItemDTO(ClassItem source)
        {
            Index = source.Index;
            Name = source.Name;
            CreateDate = source.CreateDate;
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
            return new ClassItem(Index, Name, CreateDate, String.Empty);
        }
    }
}