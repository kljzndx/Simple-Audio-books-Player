﻿using System;
using SimpleAudioBooksPlayer.DAL;

namespace SimpleAudioBooksPlayer.Models.DTO
{
    public class MusicFileDTO
    {
        public MusicFileDTO(MusicFile source)
        {
            Group = new MusicGroupDTO(source.Group);
            Title = source.Title;
            Duration = source.Duration;
            FileName = source.FileName;
            FilePath = source.FilePath;
            ModifyTime = source.ModifyTime;
        }

        public MusicGroupDTO Group { get; set; }

        public string Title { get; set; }
        public TimeSpan Duration { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime ModifyTime { get; set; }

    }
}