using System;
using SimpleAudioBooksPlayer.DAL;

namespace SimpleAudioBooksPlayer.Models.DTO
{
    public class MusicGroupDTO
    {
        public MusicGroupDTO(MusicGroup source)
        {
            Index = source.Index;
            Name = source.Name;
            HasCover = source.HasCover;
            CreateTime = source.CreateTime;
        }

        public int Index { get; }
        public string Name { get; }
        public bool HasCover { get; }
        public DateTime CreateTime { get; set; }
    }
}