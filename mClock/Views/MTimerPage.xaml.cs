using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mClock.Models;
using mClock.ViewModels;
using mClock.ViewModels.Base;
using Xamarin.Forms;

namespace mClock.Views
{
    public partial class MTimerPage : ContentPage
    {
        MTimerViewModel viewModel;
        bool isTotalMinutesTripleTapped = false;
        public MTimerPage()
        {
            InitializeComponent();

            viewModel = new MTimerViewModel();
            BindingContext = viewModel;
            timerGrid.SizeChanged += TimerGrid_SizeChanged;
        }

        private void TimerGrid_SizeChanged(object sender, EventArgs e)
        {
            timerGrid.WidthRequest = timerGrid.Height;
        }

        async void OnLableSwiped(System.Object sender, SwipedEventArgs e)
        {
            switch (e.Direction)
            {
                case SwipeDirection.Left:

                    break;
                case SwipeDirection.Right:
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
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await viewModel?.LoadAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //await viewModel?.UnloadAsync();
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
    }
}
