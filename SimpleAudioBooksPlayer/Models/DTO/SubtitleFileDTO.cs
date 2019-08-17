using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using HappyStudio.Parsing.Subtitle;
using HappyStudio.Parsing.Subtitle.Interfaces;
using HappyStudio.UwpToolsLibrary.Auxiliarys;
using SimpleAudioBooksPlayer.DAL;
using ObservableObject = GalaSoft.MvvmLight.ObservableObject;

namespace SimpleAudioBooksPlayer.Models.DTO
{
    public class SubtitleFileDTO : ObservableObject
    {
        private WeakReference<List<ISubtitleLine>> _linesReference;

        public SubtitleFileDTO(SubtitleFile source)
        {
            DisplayName = source.DisplayName;
            FileName = source.FileName;
            FilePath = source.FilePath;
            ModifyDate = source.ModifyTime;
        }

        public string DisplayName { get; }
        public string FileName { get; }
        public string FilePath { get; }
        public DateTime ModifyDate { get; private set; }

        public async Task<List<ISubtitleLine>> GetLines()
        {
            List<ISubtitleLine> lines = null;
            _linesReference?.TryGetTarget(out lines);
            if (lines != null)
                return lines;

            var file = await StorageFile.GetFileFromPathAsync(FilePath);
            var subtitle = SubtitleParser.Parse(await FileReader.ReadText(file, "GBK"));
            lines = subtitle.Lines.ToList();
            _linesReference = new WeakReference<List<ISubtitleLine>>(lines);

            return lines;
        }

        public void Update(SubtitleFile newSource)
        {
            if (newSource.FilePath != this.FilePath)
                throw new Exception("不是同一个文件");

            ModifyDate = newSource.ModifyTime;

            _linesReference = null;
        }
    }
}