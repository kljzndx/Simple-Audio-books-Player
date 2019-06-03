using System;
using System.Linq;

namespace SimpleAudioBooksPlayer.DAL
{
    internal static class StringExtensions
    {
        public static string TakeFileName(this string path)
        {
            var pathParagraphs = path.Split('\\');
            return pathParagraphs[pathParagraphs.Length - 1];
        }

        public static string TakeParentFolderName(this string path)
        {
            var pathParagraphs = path.Split('\\');
            return pathParagraphs[pathParagraphs.Length - 2];
        }

        public static string TakeParentFolderPath(this string path)
        {
            var pathParagraphs = path.Split('\\').ToList();
            pathParagraphs.Remove(pathParagraphs.LastOrDefault());
            return String.Join("\\", pathParagraphs);
        }
    }
}