using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SimpleAudioBooksPlayer.Models.Sorters
{
    public static class SystemStringSorter
    {
        public static int Compare(string x, string y)
        {
            return String.Compare(x, y, StringComparison.CurrentCulture);
        }
    }
}