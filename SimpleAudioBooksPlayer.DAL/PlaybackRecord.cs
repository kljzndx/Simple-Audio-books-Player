using System.ComponentModel.DataAnnotations;

namespace SimpleAudioBooksPlayer.DAL
{
    public class PlaybackRecord
    {
        [Key]
        public int Index { get; set; }

        public int GroupId { get; set; }
        public int TrackId { get; set; }

        public string SortMethod { get; set; }
        public bool IsReverse { get; set; }
    }
}