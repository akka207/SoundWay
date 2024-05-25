using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace soundway
{
    internal class MediaElementController: INotifyPropertyChanged
    {
        public event EventHandler OnPlay;
        public event EventHandler OnPause;
        public event EventHandler OnStop;
        public event EventHandler OnMediaEnded;


        private double secondsProgress = 0;
        public double SecondsProgress
        {
            get { return secondsProgress; }
            set
            {
                if (secondsProgress != value)
                {
                    secondsProgress = value;
                    OnPropertyChanged(nameof(SecondsProgress));
                }
            }
        }

        private double secondsMaximum = 1;
        public double SecondsMaximum
        {
            get { return secondsMaximum; }
            set
            {
                if (secondsMaximum != value)
                {
                    secondsMaximum = value;
                    OnPropertyChanged(nameof(SecondsMaximum));
                }
            }
        }

        public void Play()
        {
            OnPlay?.Invoke(this, EventArgs.Empty);
        }
        public void Pause()
        {
            OnPause?.Invoke(this, EventArgs.Empty);
        }
        public void Stop()
        {
            OnStop?.Invoke(this, EventArgs.Empty);
        }
        public void MediaEnded()
        {
            OnMediaEnded?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
