using System;
using System.Collections.Generic;
using mClock.ViewModels;
using mClock.ViewModels.Base;
using Xamarin.Forms;

namespace mClock.Views
{
    public partial class MTimerPage : ContentPage
    {
        MTimerViewModel viewModel;
        public MTimerPage()
        {
            InitializeComponent();

            viewModel = new MTimerViewModel();
            BindingContext = viewModel;
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
                    if (viewModel.DefaultMinutes >= 5)
                        viewModel.DefaultMinutes += 5;
                    else
                        viewModel.DefaultMinutes += 1;
                    break;
                case SwipeDirection.Down:
                    if (viewModel.DefaultMinutes > 5)
                        viewModel.DefaultMinutes -= 5;
                    else if (viewModel.DefaultMinutes > 1)
                        viewModel.DefaultMinutes -= 1;
                    break;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var vm = BindingContext as BaseViewModel;
            await vm?.LoadAsync();
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            var vm = BindingContext as BaseViewModel;
            await vm?.UnloadAsync();
        }
    }
}
