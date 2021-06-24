using System;
namespace mClock.Services
{
    public interface IAppVersionAndBuild
    {
        string GetShortVersion();
        string GetAppVersion();
    }
}
