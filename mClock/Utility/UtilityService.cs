using System;
using System.Collections.Generic;
using mClock.Services;
using Xamarin.Forms;

namespace mClock.Utility
{
    public static class UtilityService
    {
        public static ResourceDictionary GetResourceDictionary(ICollection<ResourceDictionary> mergedDictionaries)
        {
            foreach (ResourceDictionary dict in mergedDictionaries)
                return dict;
            return null;
        }

        public static string GetShortVersion()
        {
            // short version
            return DependencyService.Get<IAppVersionAndBuild>().GetShortVersion();
        }

        public static string GetBuildNumber()
        {
            // version
            return DependencyService.Get<IAppVersionAndBuild>().GetAppVersion();
        }

        public static bool IsScreenPortrait
        {
            get
            {
                return Application.Current.MainPage.Width < Application.Current.MainPage.Height;
            }
        }
    }
}
