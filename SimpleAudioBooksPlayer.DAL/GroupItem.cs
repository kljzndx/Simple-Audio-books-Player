using System.ComponentModel.DataAnnotations;

namespace SimpleAudioBooksPlayer.DAL
{
    public class GroupItem
    {
        public GroupItem()
        {
        }

        public GroupItem(string name)
        {
            Name = name;
        }

        [Key]
        public int Index { get; set; }
        public string Name { get; set; }
        public bool HasCover { get; set; }
    }
}