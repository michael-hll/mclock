using System;
using AudioToolbox;
using mClock.iOS;
using mClock.Services;

[assembly: Xamarin.Forms.Dependency(typeof(PlaySoundService))]
namespace mClock.iOS
{
    public class PlaySoundService : IPlaySoundService
    {
        public void PlaySystemSound(uint soundID)
        {
            // https://iphonedev.wiki/index.php/AudioServices
            var sound = new SystemSound(soundID);
            sound.PlaySystemSound();
        }
    }
}
