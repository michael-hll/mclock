using Xamarin.Forms;
using UIKit;
using mClock.Services;
using System;
using mClock.iOS;

[assembly: Dependency(typeof(BrightnessService))]
namespace mClock.iOS
{
    public class BrightnessService : IBrightnessService
    {
        public void SetBrightness(float brightness)
        {
            UIScreen.MainScreen.Brightness = brightness;
        }

        public float GetBrightness()
        {
            return (float)UIScreen.MainScreen.Brightness;
        }
    }
}
