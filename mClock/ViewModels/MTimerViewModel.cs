using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using mClock.Models;
using mClock.ViewModels.Base;
using mClock.Views;
using Xamarin.Forms;

namespace mClock.ViewModels
{
    public class MTimerViewModel : BaseViewModel
    {
        private MTimer _mtimer;
        private Countdown _countdown;
        private int _days;
        private int _hours;
        private int _minutes;
        private double _progress;
        private double _progress_min;
        private int _defaultMinutes;

        public MTimerViewModel()
        {
            _countdown = new Countdown();
            _defaultMinutes = MClockPage.MainInstance.MTimerMinutes;
        }

        public MTimer MTimer
        {
            get => _mtimer;
            set => SetProperty(ref _mtimer, value);
        }

        public int Days
        {
            get => _days;
            set => SetProperty(ref _days, value);
        }

        public int Hours
        {
            get => _hours;
            set => SetProperty(ref _hours, value);
        }

        public int Minutes
        {
            get => _minutes;
            set => SetProperty(ref _minutes, value);
        }

        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public double ProgressMin
        {
            get => _progress_min;
            set => SetProperty(ref _progress_min, value);
        }

        public int DefaultMinutes
        {
            get => _defaultMinutes;
            set
            {
                SetProperty(ref _defaultMinutes, value);
                MClockPage.MainInstance.MTimerMinutes = value;
            }
        }

        public ICommand RestartCommand => new Command(Restart);

        public override Task LoadAsync()
        {
            LoadMTimer();

            _countdown.EndDate = MTimer.Date;
            _countdown.Start();

            _countdown.Ticked += OnCountdownTicked;
            _countdown.Completed += OnCountdownCompleted;

            return base.LoadAsync();
        }

        public override Task UnloadAsync()
        {
            _countdown.Ticked -= OnCountdownTicked;
            _countdown.Completed -= OnCountdownCompleted;

            return base.UnloadAsync();
        }

        void OnCountdownTicked()
        {
            Days = _countdown.RemainTime.Days;
            Hours = _countdown.RemainTime.Hours;
            Minutes = _countdown.RemainTime.Minutes;

            var totalSeconds = (MTimer.Date - MTimer.Creation).TotalSeconds;
            var remainSeconds = _countdown.RemainTime.TotalSeconds;
            Progress = remainSeconds / totalSeconds;
            ProgressMin = (double)(remainSeconds - Minutes * 60) / 60d;
        }

        void OnCountdownCompleted()
        {
            Days = 0;
            Hours = 0;
            Minutes = 0;

            Progress = 0;
        }

        void LoadMTimer()
        {
            var mtimer = new MTimer()
            {
                Picture = "mtimer_bg",
                Date = DateTime.Now + new TimeSpan(0, 0, MClockPage.MainInstance.MTimerMinutes, 0),
                Creation = DateTime.Now
            };

            MTimer = mtimer;
        }

        void Restart()
        {
            Debug.WriteLine("Restart");
        }
    }
}
