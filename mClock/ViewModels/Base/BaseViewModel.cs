using System;
using System.Threading.Tasks;

namespace mClock.ViewModels.Base
{
    public abstract class BaseViewModel : ExtendedBindableObject
    {
        private bool _isBusy;

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        public virtual Task LoadAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task StartAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task UnloadAsync()
        {
            return Task.CompletedTask;
        }
    }

}
