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
        private int _totalMinutes;
        private double _progress;
        private double _progress_min;
        private int _defaultMinutes;

        public MTimerViewModel()
        {
            _countdown = new Countdown();
            _defaultMinutes = MClockPage.MainInstance.MTimerDefaultMins;
            _totalMinutes = MClockPage.MainInstance.MTimerDefaultMins;
        }

        public MTimer MTimer
        {
            get => _mtimer;
            set => SetProperty(ref _mtimer, value);
        }

        public Countdown Countdown
        {
            get => _countdown;
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

        /**
         * Remaining Minutes
         */
        public int Minutes
        {
            get => _minutes;
            set => SetProperty(ref _minutes, value);
        }

        /**
         * Total Left Minutes
         */
        public int TotalMinutes
        {
            get => _totalMinutes;
            set => SetProperty(ref _totalMinutes, value);
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
                TotalMinutes = DefaultMinutes;
                MClockPage.MainInstance.MTimerDefaultMins = value;
            }
        }

        public override Task StartAsync()
        {
            CreateMTimer();

            _countdown.EndDate = MTimer.Date;
            _countdown.Start();

            return base.StartAsync();
        }

        public override Task LoadAsync()
        {
            _countdown.Ticked += OnCountdownTicked;
            _countdown.Completed += OnCountdownCompleted;
            _countdown.Paused += OnCountdownPaused;
            _countdown.Stopped += OnCountdownStopped;

            return base.LoadAsync();
        }

        public override Task UnloadAsync()
        {
            _countdown.Ticked -= OnCountdownTicked;
            _countdown.Completed -= OnCountdownCompleted;
            _countdown.Paused -= OnCountdownPaused;
            _countdown.Stopped -= OnCountdownStopped;

            return base.UnloadAsync();
        }

        void OnCountdownTicked()
        {
            Days = _countdown.RemainTime.Days;
            Hours = _countdown.RemainTime.Hours;
            Minutes = _countdown.RemainTime.Minutes;
            TotalMinutes = Hours * 60 + Minutes + 1;

            var totalSeconds = (MTimer.Date - MTimer.Creation).TotalSeconds;
            var remainSeconds = _countdown.RemainTime.TotalSeconds;
            Progress = remainSeconds / totalSeconds;
            ProgressMin = (double)(_countdown.RemainTime.Seconds * 1000 + _countdown.RemainTime.Milliseconds) / (double)(60 * 1000);
        }

        void OnCountdownCompleted()
        {
            Days = 0;
            Hours = 0;
            Minutes = 0;
            TotalMinutes = MClockPage.MainInstance.MTimerDefaultMins;

            Progress = 0;
            ProgressMin = 0;
        }

        void OnCountdownPaused()
        {

        }

        void OnCountdownStopped()
        {
            Days = 0;
            Hours = 0;
            Minutes = 0;
            TotalMinutes = MClockPage.MainInstance.MTimerDefaultMins;

            Progress = 0;
            ProgressMin = 0;
        }

        void CreateMTimer()
        {
            int hours = MClockPage.MainInstance.MTimerDefaultMins / 60;
            int mins = MClockPage.MainInstance.MTimerDefaultMins - hours * 60;
            var mtimer = new MTimer()
            {
                Picture = "mtimer_bg",
                Date = DateTime.Now + new TimeSpan(0, hours, mins, 0),

                Creation = DateTime.Now
            };

            MTimer = mtimer;
        }
    }
}
