using System;
using System.Collections.Generic;
using System.Reflection;

namespace SimpleAudioBooksPlayer.Models.Attributes
{
    public static class PageTitleGetter
    {
        private static readonly Dictionary<Type, string> AllTitles = new Dictionary<Type, string>();

        public static string GetTitle(Type pageType)
        {
            if (!AllTitles.ContainsKey(pageType))
            {
                var info = pageType.GetTypeInfo();
                var attribute = info.GetCustomAttribute<PageTitleAttribute>();
                if (attribute is null)
                    AllTitles[pageType] = String.Empty;
                else
                {
                    try
                    {
                        var title = attribute.GetTitle();
                        AllTitles[pageType] = title;
                    }
                    catch
                    {
                        AllTitles[pageType] = String.Empty;
                    }
                }
            }

            return AllTitles[pageType];
        }
    }
}