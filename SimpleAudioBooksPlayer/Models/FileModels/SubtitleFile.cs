using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HappyStudio.Parsing.Subtitle;
using HappyStudio.Parsing.Subtitle.Interfaces;
using HappyStudio.UwpToolsLibrary.Auxiliarys;
using SimpleAudioBooksPlayer.Models.DTO;

namespace SimpleAudioBooksPlayer.Models.FileModels
{
    public class SubtitleFile : LibraryFileBase
    {
        public SubtitleFile(FileGroupDTO groupDto, string fileName) : base(groupDto, fileName)
        {
        }

        public async Task<List<ISubtitleLine>> GetSubtitleLines()
        {
            string content = await FileReader.ReadText(await base.GetFileAsync(), "GBK");
            var lineUiList = SubtitleParser.Parse(content).Lines.ToList();

            return lineUiList;
        }
    }
}