using System.Collections.Generic;
using System.Threading.Tasks;
using HappyStudio.Parsing.Subtitle;
using HappyStudio.Subtitle.Control.Interface;
using HappyStudio.Subtitle.Control.Interface.Models.Extensions;
using HappyStudio.UwpToolsLibrary.Auxiliarys;
using SimpleAudioBooksPlayer.Models.DTO;

namespace SimpleAudioBooksPlayer.Models.FileModels
{
    public class SubtitleFile : LibraryFileBase
    {
        public SubtitleFile(FileGroupDTO groupDto, string fileName) : base(groupDto, fileName)
        {
        }

        public async Task<IEnumerable<ISubtitleLineUi>> GetSubtitleLines()
        {
            string content = await FileReader.ReadText(await base.GetFileAsync(), "GBK");
            var lineUiList = SubtitleParser.Parse(content).Lines.ToLineUiList();

            return lineUiList;
        }
    }
}