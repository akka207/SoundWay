using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace soundway
{
    internal class SongItem:INotifyPropertyChanged
    {
        private bool isPlayed = false;
        public bool IsPlayed
        {
            get { return isPlayed; }
            set
            {
                if (isPlayed != value)
                {
                    isPlayed = value;
                    OnPropertyChanged(nameof(IsPlayed));
                }
            }
        }

        private bool isPause = false;
        public bool IsPause
        {
            get { return isPause; }
            set
            {
                if (isPause != value)
                {
                    isPause = value;
                    OnPropertyChanged(nameof(IsPause));
                }
            }
        }

        private bool isLiked = false;
        public bool IsLiked
        {
            get { return isLiked; }
            set
            {
                if (isLiked != value)
                {
                    isLiked = value;
                    OnPropertyChanged(nameof(IsLiked));
                }
            }
        }

        private int number = 0;
        public int Number
        {
            get { return number; }
            set
            {
                if (number != value)
                {
                    number = value;
                    OnPropertyChanged(nameof(Number));
                }
            }
        }

        public string FilePath { get; set; }
        public string Title { get; set; }
        public string Band { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
        public string Duration { get; set; }
        public BitmapImage Image { get; set; }

        public SongItem()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
