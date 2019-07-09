using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SimpleAudioBooksPlayer.Models.Sorters
{
    public static class SystemStringSorter
    {
        [DllImport("Shlwapi.dll", SetLastError = false, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string psz1, string psz2);

        public static int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }
    }
}