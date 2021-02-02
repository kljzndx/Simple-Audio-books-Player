using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HappyStudio.Parsing.Subtitle;
using HappyStudio.Parsing.Subtitle.Interfaces;
using HappyStudio.UwpToolsLibrary.Auxiliarys;
using Newtonsoft.Json;
using SimpleAudioBooksPlayer.Models.DTO;

namespace SimpleAudioBooksPlayer.Models.FileModels
{
    public class SubtitleFile : LibraryFileBase
    {
        public SubtitleFile(FileGroupDTO groupDto, string fileName) : base(groupDto, fileName)
        {
        }

        [JsonConstructor]
        public SubtitleFile(string fileName) : this(null, fileName)
        {
        }

        public async Task<List<ISubtitleLine>> GetSubtitleLines(Action<IFile> notFoundErrorCallback = null)
        {
            var file = await base.GetFileAsync(notFoundErrorCallback);
            if (file == null)
                return null;

            string content = await FileReader.ReadText(file, "GBK");
            var lineUiList = SubtitleParser.Parse(content).Lines.ToList();

            return lineUiList;
        }
    }
}