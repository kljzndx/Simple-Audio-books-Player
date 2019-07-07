using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleAudioBooksPlayer.Models.Sorters
{
    public static class TitleSorter
    {
        private static readonly Regex NumberRegex = new Regex("[0-9]+");

        public static int Compare(string input1, string input2)
        {
            int result = 0;

            var match1 = NumberRegex.Matches(input1);
            var match2 = NumberRegex.Matches(input2);
            var str1 = input1;
            var str2 = input2;

            foreach (Match match in match1)
                str1 = str1.Replace(match.Value, String.Empty);

            foreach (Match match in match2)
                str2 = str2.Replace(match.Value, String.Empty);

            result = String.Compare(str1, str2, StringComparison.Ordinal);
            if (result != 0)
                return result;

            string numStr1 = match1.Any(m => m.Success) ? String.Concat(match1.Select(m => m.Value)) : String.Empty;
            string numStr2 = match2.Any(m => m.Success) ? String.Concat(match2.Select(m => m.Value)) : String.Empty;

            ulong num1 = 0;
            ulong num2 = 0;

            bool b1 = ulong.TryParse(numStr1, out num1);
            bool b2 = ulong.TryParse(numStr2, out num2);

            if (b1 && b2)
                result = num1.CompareTo(num2);
            if (b1 && !b2)
                result = 1;
            if (!b1 && b2)
                result = -1;

            return result;
        }
    }
}