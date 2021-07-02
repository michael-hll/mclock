using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using mClock.Views;
using System.IO;
using System.Threading;
using System.Globalization;
using mClock.Resources;

namespace mClock
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            // Load localizations
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
            AppResources.Culture = Thread.CurrentThread.CurrentUICulture;

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
