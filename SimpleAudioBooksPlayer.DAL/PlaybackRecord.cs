using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleAudioBooksPlayer.DAL
{
    public class PlaybackRecord
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int GroupId { get; set; }
        public int TrackId { get; set; }

        public string SortMethod { get; set; }
        public bool IsReverse { get; set; }
    }
}