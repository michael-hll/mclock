using System;
using System.Threading.Tasks;
using mClock.Models;
using mClock.Utility;
using mClock.ViewModels;
using Xamarin.Forms;

namespace mClock.Views
{
    public partial class MTimerPage : ContentPage
    {
        MTimerViewModel viewModel;
        bool isTotalMinutesTripleTapped = false;
        public double width;
        public double height;

        public int TimeFormatIndex
        {
            get => MClockPage.AppSettings.GetValueOrDefault(nameof(TimeFormatIndex), 0);
            set => MClockPage.AppSettings.AddOrUpdateValue(nameof(TimeFormatIndex), value);
        }

        public static readonly string[] TimeFormats = {
            "HH:mm",
            "hh:mm tt",
            "HH:mm:ss",
            "hh:mm:ss tt",
            "MMM dd, HH:mm:ss",
            "MMM dd, hh:mm:ss tt",
            "ddd, MMM dd, HH:mm:ss",
            "ddd, MMM dd, hh:mm:ss tt",
        };

        public MTimerPage()
        {
            InitializeComponent();

            viewModel = new MTimerViewModel();
            BindingContext = viewModel;
            viewModel.CurrentTime = DateTime.Now.ToString(TimeFormats[TimeFormatIndex]);

            // update date/time in timer page
            Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
            {
                viewModel.CurrentTime = DateTime.Now.ToString(TimeFormats[TimeFormatIndex]);
                return true;
            });
        }

        async void OnLableSwiped(System.Object sender, SwipedEventArgs e)
        {
            switch (e.Direction)
            {
                case SwipeDirection.Left:

                    break;
                case SwipeDirection.Right:
                    if (Navigation.NavigationStack.Count > 1)
                        await Navigation.PopAsync();
                    break;
                case SwipeDirection.Up:
                    if (viewModel.Countdown.State == CountdownState.Stopped)
                    {
                        CalculateNewMinutes(1);
                    }
                    break;
                case SwipeDirection.Down:
                    if (viewModel.Countdown.State == CountdownState.Stopped)
                    {
                        CalculateNewMinutes(-1);
                    }
                    break;
            }
        }

        void CalculateNewMinutes(int direction)
        {
            int current = viewModel.DefaultMinutes;
            int steps = 0;
            if (direction > 0)
            {
                if (current >= 1 && current < 5)
                    steps = 1;
                else if (current >= 5 && current < 60)
                {
                    steps = 5;
                }
                else if (current >= 60 && current < 120)
                    steps = 10;
            }
            else
            {
                if (current > 1 && current <= 5)
                    steps = 1;
                else if (current > 5 && current <= 60)
                    steps = 5;
                else if (current > 60)
                    steps = 10;
            }
            viewModel.DefaultMinutes = current + steps * direction;
            UpdateLableFontSizes(Application.Current.MainPage.Width);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await viewModel?.LoadAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        async void OnTotalMinutesDoubleTapped(System.Object sender, System.EventArgs e)
        {
            await Task.Delay(250);
            if (isTotalMinutesTripleTapped) return;

            // Begin double click
            // Triger stop/pause events

            if (viewModel.Countdown.State == CountdownState.Stopped)
            {
                viewModel.Countdown.State = CountdownState.Running;
                viewModel?.StartAsync();
            }
            else if (viewModel.Countdown.State == CountdownState.Running)
            {
                viewModel.Countdown.State = CountdownState.Paused;
            }
            else if (viewModel.Countdown.State == CountdownState.Paused)
            {
                viewModel.Countdown.State = CountdownState.Running;
            }
        }

        async void OnTotalMinutesTripleTapped(System.Object sender, System.EventArgs e)
        {
            isTotalMinutesTripleTapped = true;
            await Task.Delay(500);

            // Begin triple click
            // Triger stop event

            if (viewModel.Countdown.State == CountdownState.Stopped)
            {
                // do nothing
            }
            else if (viewModel.Countdown.State == CountdownState.Running)
            {
                viewModel.Countdown.State = CountdownState.Stopped;
            }
            else if (viewModel.Countdown.State == CountdownState.Paused)
            {
                viewModel.Countdown.State = CountdownState.Stopped;
            }

            isTotalMinutesTripleTapped = false;
        }

        void OnTimeSwiped(System.Object sender, SwipedEventArgs e)
        {
            switch (e.Direction)
            {
                case SwipeDirection.Left:
                    TimeFormatIndex--;
                    if (TimeFormatIndex == -1) TimeFormatIndex = TimeFormats.Length - 1;
                    break;
                case SwipeDirection.Right:
                    TimeFormatIndex++;
                    if (TimeFormatIndex == TimeFormats.Length) TimeFormatIndex = 0;
                    break;
                case SwipeDirection.Up:
                    // Handle the swipe
                    break;
                case SwipeDirection.Down:
                    // Handle the swipe
                    break;
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width != this.width || height != this.height)
            {
                this.width = width;
                this.height = height;

                UpdateLableFontSizes(this.width);
            }
        }

        protected void UpdateLableFontSizes(double width)
        {
            double fontSizeDivisor = 6;
            if (Device.Idiom == TargetIdiom.Tablet)
            {
                // iPad
                fontSizeDivisor = 5;
            }
            if (viewModel.DefaultMinutes > 99)
                fontSizeDivisor += 1.5;
            if (UtilityService.IsScreenPortrait)
                viewModel.TimerFontSize = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width / fontSizeDivisor;
            else
                viewModel.TimerFontSize = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Height / fontSizeDivisor;
        }
    }
}
