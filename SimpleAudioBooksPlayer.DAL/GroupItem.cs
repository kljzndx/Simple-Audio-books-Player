using System.ComponentModel.DataAnnotations;

namespace SimpleAudioBooksPlayer.DAL
{
    public class GroupItem
    {
        [Key]
        public int Index { get; set; }
        public string Name { get; set; }
        public bool HasCover { get; set; }
    }
}