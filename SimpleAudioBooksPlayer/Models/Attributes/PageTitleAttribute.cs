using System;
using Windows.ApplicationModel.Resources;

namespace SimpleAudioBooksPlayer.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PageTitleAttribute : Attribute
    {
        public PageTitleAttribute(string resourceFile)
        {
            ResourceFile = resourceFile;
        }

        public PageTitleAttribute(string resourceFile, string resourceKey)
        {
            ResourceFile = resourceFile;
            ResourceKey = resourceKey;
        }

        public string ResourceFile { get; set; }
        public string ResourceKey { get; set; }

        public string GetTitle()
        {
            return ResourceLoader.GetForCurrentView(ResourceFile).GetString(ResourceKey ?? "Title");
        }
    }
}