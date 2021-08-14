using System;
using System.Collections.Generic;
using mClock.Views;
using Xamarin.Forms;
using mClock.Utility;

namespace mClock
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;
        }


        public string MClockVersion
        {
            get
            {
                return Utility.UtilityService.GetShortVersion() + " (Build " + Utility.UtilityService.GetBuildNumber() + ")";
            }
        }
    }


}
