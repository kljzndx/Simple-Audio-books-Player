using System;
using System.Linq;
using System.Text.RegularExpressions;
using SimpleAudioBooksPlayer.Models.DTO;

namespace SimpleAudioBooksPlayer.Models
{
    public enum MusicListSortMembers
    {
        TrackId,
        Name,
        ModifyTime
    }

    public delegate IComparable MusicListSortSelector<T>(T source);

    public class MusicListSortArgs
    {
        private static readonly Regex NumberRegex = new Regex(@"[0-9]+");

        public MusicListSortArgs(MusicListSortMembers member)
        {
            SortMethod = member.ToString();

            switch (member)
            {
                case MusicListSortMembers.TrackId:
                    KeySelector = s => s.TrackNumber;
                    break;
                case MusicListSortMembers.Name:
                    KeySelector = s =>
                    {
                        var matches = NumberRegex.Matches(s.Title);
                        if (matches.Any(m => m.Success))
                        {
                            string numStr = String.Concat(matches.Select(m => m.Value));
                            if (UInt32.TryParse(numStr, out uint iResult))
                                return iResult;
                            else if (UInt64.TryParse(numStr, out ulong lResult))
                                return lResult;
                        }

                        return s.Title;
                    };
                    break;
                case MusicListSortMembers.ModifyTime:
                    KeySelector = s => s.ModifyTime;
                    break;
            }
        }


        public string SortMethod { get; }
        public MusicListSortSelector<MusicFileDTO> KeySelector { get; }
    }
}