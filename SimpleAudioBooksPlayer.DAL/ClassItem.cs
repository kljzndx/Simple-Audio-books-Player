using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace SimpleAudioBooksPlayer.DAL
{
    public class ClassItem
    {
        public ClassItem()
        {
        }

        public ClassItem(string name, DateTime date, string backgroundColor)
        {
            Name = name;
            CreateDate = date;
            BackgroundColor = backgroundColor;
        }

        public ClassItem(int index, string name, DateTime date, string backgroundColor) : this(name, date, backgroundColor)
        {
            Index = index;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Index { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string BackgroundColor { get; set; }
    }
}