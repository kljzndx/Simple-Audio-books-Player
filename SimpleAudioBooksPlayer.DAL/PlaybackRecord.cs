using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleAudioBooksPlayer.DAL
{
    public class PlaybackRecord
    {
        public PlaybackRecord()
        {
            
        }

        public PlaybackRecord(int groupId, uint trackId, string sortMethod, bool isReverse = false)
        {
            GroupId = groupId;
            TrackId = trackId;
            SortMethod = sortMethod;
            IsReverse = isReverse;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int GroupId { get; set; }
        public uint TrackId { get; set; }

        public string SortMethod { get; set; }
        public bool IsReverse { get; set; }
    }
}