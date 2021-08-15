using System;
using Xamarin.Forms;

namespace mClock.Models
{
    public class Countdown : BindableObject
    {
        TimeSpan _remainTime;
        CountdownState _countdownState = CountdownState.Stopped;

        public event Action Completed;
        public event Action Ticked;
        public event Action Paused;
        public event Action Stopped;

        public DateTime EndDate { get; set; }

        public TimeSpan RemainTime
        {
            get { return _remainTime; }

            private set
            {
                _remainTime = value;
                OnPropertyChanged();
            }
        }

        public CountdownState State
        {
            get { return _countdownState; }
            set
            {
                _countdownState = value;
                OnPropertyChanged();
            }
        }

        public void Start(double seconds = 1)
        {
            Device.StartTimer(TimeSpan.FromSeconds(seconds), () =>
            {
                if (State == CountdownState.Running)
                {
                    RemainTime = (EndDate - DateTime.Now);

                    var ticked = RemainTime.TotalSeconds > 1;

                    if (ticked)
                    {
                        Ticked?.Invoke();
                    }
                    else
                    {
                        Completed?.Invoke();
                    }

                    return ticked;
                }
                if (State == CountdownState.Paused)
                {
                    EndDate = DateTime.Now.Add(RemainTime);
                    Paused?.Invoke();
                    return true;
                }
                if (State == CountdownState.Stopped)
                {
                    Stopped?.Invoke();
                    return false;
                }
                return false;
            });
        }
    }
}
