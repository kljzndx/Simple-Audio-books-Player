using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleAudioBooksPlayer.DAL
{
    public class MusicGroup
    {
        public MusicGroup()
        {
        }

        public MusicGroup(string name)
        {
            Name = name;
            CreateTime = DateTime.Now;
        }

        [Key]
        public int Index { get; set; }
        public string Name { get; set; }
        public bool HasCover { get; set; }
        public DateTime CreateTime { get; set; }
    }
}