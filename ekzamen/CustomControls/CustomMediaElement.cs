using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace soundway.CustomControls
{
    internal class CustomMediaElement:MediaElement
    {
        #region Attached properties area
        public MediaElementController Controller
        {
            get { return (MediaElementController)GetValue(ControllerProperty); }
            set 
            { 
                SetValue(ControllerProperty, value);
                value.OnPlay += Controller_OnPlay;
                value.OnPause += Controller_OnPause;
                value.OnStop += Controller_OnStop;
            }
        }

        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register("Controller", typeof(MediaElementController), typeof(CustomMediaElement), new UIPropertyMetadata(new MediaElementController()));
        #endregion


        private DispatcherTimer timer = new DispatcherTimer();

        private void Controller_OnPlay(object sender, EventArgs e)
        {
            this.Play();
            timer.Start();
        }
        private void Controller_OnPause(object sender, EventArgs e)
        {
            this.Pause();
            timer.Stop();
        }
        private void Controller_OnStop(object sender, EventArgs e)
        {
            this.Stop();
            timer.Stop();
        }

        public CustomMediaElement()
        {
            Controller.OnPlay += Controller_OnPlay;
            Controller.OnPause += Controller_OnPause;
            Controller.OnStop += Controller_OnStop;

            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += new EventHandler(OnTimerTick);

            MediaOpened += CustomMediaElement_MediaOpened;
            MediaEnded += CustomMediaElement_MediaEnded;
        }

        private void CustomMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            Controller.MediaEnded();
        }

        private void CustomMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            Controller.SecondsMaximum = NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            Controller.SecondsProgress = Position.TotalSeconds;
        }
    }
}
