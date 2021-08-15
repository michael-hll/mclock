using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using mClock.Models;
using mClock.Services;
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
        private double _timerFontSize;
        private string _currTime;
        private Color _stateColor;
        private bool isLoaded = false;

        public MTimerViewModel()
        {
            _countdown = new Countdown();
            _countdown.PropertyChanged += Countdown_PropertyChanged;
            _defaultMinutes = MClockPage.MainInstance.MTimerDefaultMins;
            _totalMinutes = MClockPage.MainInstance.MTimerDefaultMins;
            _timerFontSize = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Height / 6.8;
            _currTime = DateTime.Now.ToString("h:mm tt");
            _stateColor = Color.Red;
        }

        private void Countdown_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State")
            {
                if (Countdown.State == CountdownState.Stopped)
                    StateColor = Color.Red;
                else if (Countdown.State == CountdownState.Paused)
                    StateColor = Color.Yellow;
                else if (Countdown.State == CountdownState.Running)
                    StateColor = Color.Lime;
            }
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

        public string CurrentTime
        {
            get => _currTime;
            set => SetProperty(ref _currTime, value);
        }

        public Color StateColor
        {
            get => _stateColor;
            set => SetProperty(ref _stateColor, value);
        }

        public Double TimerFontSize
        {
            get => _timerFontSize;
            set => SetProperty(ref _timerFontSize, value);
        }

        public override Task StartAsync()
        {
            CreateMTimer();

            _countdown.EndDate = MTimer.Date;
            _countdown.Start(1 / 60);

            return base.StartAsync();
        }

        public override Task LoadAsync()
        {
            if (!isLoaded)
            {
                _countdown.Ticked += OnCountdownTicked;
                _countdown.Completed += OnCountdownCompleted;
                _countdown.Paused += OnCountdownPaused;
                _countdown.Stopped += OnCountdownStopped;
                isLoaded = true;
            }

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
            Countdown.State = CountdownState.Stopped;
            DependencyService.Get<IPlaySoundService>().PlaySystemSound(1005);
            ResetParameters();
        }

        void OnCountdownPaused()
        {

        }

        void OnCountdownStopped()
        {
            ResetParameters();
        }

        void ResetParameters()
        {
            Days = 0;
            Hours = 0;
            Minutes = 0;
            Progress = 0;
            ProgressMin = 0;
            TotalMinutes = MClockPage.MainInstance.MTimerDefaultMins;
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
