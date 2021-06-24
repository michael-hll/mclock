using Foundation;
using mClock.iOS;
using mClock.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppVersionAndBuild))]
namespace mClock.iOS
{
    public class AppVersionAndBuild : IAppVersionAndBuild
    {

        public string GetShortVersion()
        {
            return NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
        }
        public string GetAppVersion()
        {
            return NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString();
        }
    }
}
