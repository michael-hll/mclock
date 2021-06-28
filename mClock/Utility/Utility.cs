using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace mClock.Utility
{
    public class Utility
    {
        public static ResourceDictionary GetResourceDictionary(ICollection<ResourceDictionary> mergedDictionaries)
        {
            foreach (ResourceDictionary dict in mergedDictionaries)
                return dict;
            return null;
        }
    }
}
