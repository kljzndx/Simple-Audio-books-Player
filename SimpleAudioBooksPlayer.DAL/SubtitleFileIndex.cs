using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleAudioBooksPlayer.DAL
{
    public class SubtitleFileIndex
    {
        public SubtitleFileIndex()
        {
        }

        public SubtitleFileIndex(string musicFilePath, string subtitleFilePath)
        {
            MusicFilePath = musicFilePath;
            SubtitleFilePath = subtitleFilePath;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Index { get; set; }
        public string MusicFilePath { get; set; }
        public string SubtitleFilePath { get; set; }
    }
}