using System;
using SimpleAudioBooksPlayer.DAL;

namespace SimpleAudioBooksPlayer.Models.DTO
{
    public class SubtitleFileDTO
    {
        public SubtitleFileDTO(SubtitleFile source)
        {
            FileName = source.FileName;
            FilePath = source.FilePath;
            ModifyDate = source.ModifyTime;
        }

        public string FileName { get; }
        public string FilePath { get; }
        public DateTime ModifyDate { get; private set; }

        public void Update(SubtitleFile newSource)
        {
            if (newSource.FilePath != this.FilePath)
                throw new Exception("不是同一个文件");

            ModifyDate = newSource.ModifyTime;

            // TODO 更新歌词列表
        }
    }
}