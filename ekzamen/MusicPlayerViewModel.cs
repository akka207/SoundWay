using soundway.CustomControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TagLib.Ogg;

namespace soundway
{
    internal class MusicPlayerViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<SongItem> songItems = new ObservableCollection<SongItem>();
        public ObservableCollection<SongItem> SongItems
        {
            get { return FiltredCollection(); }
            set
            {
                if (songItems != value)
                {
                    songItems = value;
                    OnPropertyChanged(nameof(SongItems));
                }
            }
        }

        public ObservableCollection<SongItem> LikedItems
        {
            get { return GetLikedSongs(); }
        }


        private string searchField = string.Empty;
        public string SearchField
        {
            get { return searchField; }
            set
            {
                if (searchField != value)
                {
                    searchField = value;
                    OnPropertyChanged(nameof(SearchField));
                    OnPropertyChanged(nameof(SongItems));
                }
            }
        }


        private static SongItem playedSong  = new SongItem() { Title = "Назва", Band = "Виконавець", IsPlayed = false, IsPause = true };
        public SongItem PlayedSong
        {
            get { return playedSong; }
            set
            {
                if (playedSong != value)
                {
                    if (value == null)
                    {
                        playedSong = new SongItem() { Title = "Title", Band = "Band", IsPlayed = false, IsPause = true };
                    }
                    else
                    {
                        playedSong = value;
                    }
                    OnPropertyChanged(nameof(PlayedSong));
                }
            }
        }

        private bool currentPauseState = true;
        public bool CurrentPauseState
        {
            get { return currentPauseState; }
            set
            {
                if (currentPauseState != value)
                {
                    currentPauseState = value;
                    OnPropertyChanged(nameof(CurrentPauseState));
                }
            }
        }


        private List<string> originLikedSongs;


        public MediaElementController controller = new MediaElementController();
        public MediaElementController Controller
        {
            get { return controller; }
            set
            {
                if (controller != value)
                {
                    controller = value;
                    OnPropertyChanged(nameof(Controller));
                    Controller.OnMediaEnded += Controller_OnMediaEnded;
                }
            }
        }


        private bool doRepeat = false;
        public bool DoRepeat
        {
            get { return doRepeat; }
            set
            {
                if (doRepeat != value)
                {
                    doRepeat = value;
                    OnPropertyChanged(nameof(DoRepeat));
                }
            }
        }

        private bool shuffleState = false;
        public bool ShuffleState
        {
            get { return shuffleState; }
            set
            {
                if (shuffleState != value)
                {
                    shuffleState = value;
                    OnPropertyChanged(nameof(ShuffleState));
                }
            }
        }

        private List<int> playQueue;


        public ICommand LoadCommand { get; }
        public ICommand LikeCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand PlayPauseCommand { get; }
        public ICommand RepeatCommand { get; }
        public ICommand ShuffleCommand { get; }
        public ICommand PrevCommand { get; }
        public ICommand NextCommand { get; }


        public MusicPlayerViewModel()
        {
            LoadCommand = new RelayCommand<bool>(CreateMusicItemsSubVoid);
            LikeCommand = new RelayCommand<SwitchButton>(LikeSong);
            PlayCommand = new RelayCommand<SwitchButton>(PlaySong);
            RemoveCommand = new RelayCommand<ImageButton>(RemoveSong);
            PlayPauseCommand = new RelayCommand<SwitchButton>(PlayPauseSwitch);
            RepeatCommand = new RelayCommand(Repeat);
            ShuffleCommand = new RelayCommand(Shuffle);
            PrevCommand = new RelayCommand(PlayPrev);
            NextCommand = new RelayCommand(PlayNext);

            LoadLikedSongs();
        }


        private void Controller_OnMediaEnded(object sender, EventArgs e)
        {
            Stop();
            if (DoRepeat)
            {
                Play();
            }
            else
            {
                PlayNext();
            }
        }

        private void CreateMusicItemsSubVoid(bool value)
        {
            CreateMusicItems(value);
        }

        private void CreateMusicItems(bool clear, List<string> pathList = null)
        {
            if (pathList == null)
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
                {
                    FileName = "Music",
                    DefaultExt = ".mp3",
                    Filter = "Music files (.mp3)|*.mp3",
                    Multiselect = true
                };

                bool? result = dialog.ShowDialog();
                if (result == true)
                {
                    List<string> filesPath = dialog.FileNames.ToList();
                    if (clear)
                    {
                        if (PlayedSong != null)
                        {
                            PlayedSong.IsPlayed = false;
                            PlayedSong = null;
                            Stop();
                        }
                        SongItems.Clear();
                        Controller.SecondsProgress = 0; 
                        Controller.SecondsMaximum = 1; 
                    }

                    foreach (string path in filesPath)
                    {
                        if (!SongItems.Any(s => s.FilePath == path))
                        {
                            SongItems.Add(new SongItem() { FilePath = path });
                        }
                    }
                    SetIndexes();
                    ExtractTags();
                    SetLikedSongs();
                    OnPropertyChanged(nameof(LikedItems));
                }
            }
            else
            {
                foreach (string path in pathList)
                {
                    if (!SongItems.Any(s => s.FilePath == path) && System.IO.File.Exists(path))
                    {
                        SongItems.Add(new SongItem() { FilePath = path });
                    }
                }
                SetIndexes();
                ExtractTags();
                SetLikedSongs();
                OnPropertyChanged(nameof(LikedItems));
            }
        }

        private void ExtractTags()
        {
            string unknownTag = "Unknown";

            foreach (SongItem musicItem in SongItems)
            {
                try
                {
                    TagLib.File file = TagLib.File.Create(musicItem.FilePath);
                    musicItem.Title = !string.IsNullOrEmpty(file.Tag.Title) ? file.Tag.Title.ToString() : unknownTag;
                    musicItem.Band = !string.IsNullOrEmpty(file.Tag.FirstPerformer) ? file.Tag.FirstPerformer.ToString() : unknownTag;
                    musicItem.Album = !string.IsNullOrEmpty(file.Tag.Album) ? file.Tag.Album.ToString() : unknownTag;
                    musicItem.Genre = !string.IsNullOrEmpty(file.Tag.FirstGenre) ? file.Tag.FirstGenre.ToString() : unknownTag;
                    double time = file.Properties.Duration.TotalSeconds;
                    musicItem.Duration = string.Format("{0:D2}:{1:D2}", (int)(time / 60), (int)(time % 60));
                    var picture = file.Tag.Pictures.FirstOrDefault();
                    if (picture != null)
                    {
                        byte[] data = picture.Data.Data;
                        BitmapImage image = new BitmapImage();
                        using (var ms = new MemoryStream(data))
                        {
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = ms;
                            image.EndInit();
                        }
                        musicItem.Image = image;
                    }
                }
                catch
                {
                    MessageBox.Show("Importing error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LikeSong(SwitchButton button)
        {
            button.ActiveState = !button.ActiveState;
            (button.DataContext as SongItem).IsLiked = button.ActiveState;
            OnPropertyChanged(nameof(LikedItems));
            SaveLikedSongs();
        }
        private void PlaySong(SwitchButton button)
        {
            if (!button.ActiveState)
            {
                if (PlayedSong != null)
                {
                    Stop();
                }
                PlayedSong = button.DataContext as SongItem;
                Play();
            }
            else
            {
                Stop();
                PlayedSong = null;
                Controller.SecondsProgress = 0;
                Controller.SecondsMaximum = 1;
            }
        }
        private void RemoveSong(ImageButton button)
        {
            if (MessageBox.Show("Підтведити видалення?", "Підтвердження", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (PlayedSong != null && PlayedSong == button.DataContext)
                {
                    Stop();
                    PlayedSong = null;
                }
                SongItems.Remove(button.DataContext as SongItem);
                OnPropertyChanged(nameof(LikedItems));
                SetIndexes();
            }
        }
        private void PlayPauseSwitch(SwitchButton button)
        {
            if (PlayedSong.IsPlayed)
            {
                if (PlayedSong.IsPause)
                {
                    Play();
                }
                else
                {
                    Pause();
                }
                CurrentPauseState = PlayedSong.IsPause;
            }
        }
        private void Repeat()
        {
            DoRepeat = !DoRepeat;
        }
        private void Shuffle()
        {
            ShuffleState = !ShuffleState;
            playQueue = CreateShuffleList();
        }

        private void SetIndexes()
        {
            for (int i = 0; i < SongItems.Count; i++)
            {
                SongItems[i].Number = i + 1;
            }
        }
        private ObservableCollection<SongItem> FiltredCollection()
        {
            if (string.Compare(SearchField, string.Empty) != 0 && songItems.Count != 0)
            {
                ObservableCollection<SongItem> temp = new ObservableCollection<SongItem>();
                foreach (var item in songItems)
                {
                    if (item.Title.Contains(SearchField))
                    {
                        temp.Add(item);
                    }
                }
                return temp;
            }
            else
            {
                return songItems;
            }
        }
        private ObservableCollection<SongItem> GetLikedSongs()
        {
            if (songItems.Count != 0)
            {
                ObservableCollection<SongItem> temp = new ObservableCollection<SongItem>();
                foreach (var item in songItems)
                {
                    if (item.IsLiked)
                    {
                        temp.Add(item);
                    }
                }
                return temp;
            }
            return null;
        }


        private void SaveLikedSongs()
        {
            System.IO.File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "LikedSongs.txt"), LikedSongsJson());
        }
        private void LoadLikedSongs()
        {
            if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "LikedSongs.txt")))
            {
                originLikedSongs = JsonConvert.DeserializeObject<List<string>>(System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "LikedSongs.txt")));
                if (originLikedSongs == null)
                {
                    originLikedSongs = new List<string>();
                }
                CreateMusicItems(false, originLikedSongs);
            }
            else
            {
                System.IO.File.Create(Path.Combine(Directory.GetCurrentDirectory(), "LikedSongs.txt"));
            }
        }
        private void SetLikedSongs()
        {
            foreach (var item in SongItems)
            {
                if (originLikedSongs.Any(t => string.Compare(t, item.FilePath) == 0))
                {
                    item.IsLiked = true;
                }
            }
        }
        private string LikedSongsJson()
        {
            foreach (var item in songItems)
            {
                if (item.IsLiked && !originLikedSongs.Contains(item.FilePath))
                {
                    originLikedSongs.Add(item.FilePath);
                }
                else if (!item.IsLiked && originLikedSongs.Contains(item.FilePath))
                {
                    originLikedSongs.Remove(item.FilePath);
                }
            }
            return JsonConvert.SerializeObject(originLikedSongs, Formatting.Indented);
        }


        private void Play()
        {
            Controller.Play();
            PlayedSong.IsPlayed = true;
            PlayedSong.IsPause = false;
            CurrentPauseState = false;
        }
        private void Pause()
        {
            Controller.Pause();
            PlayedSong.IsPlayed = true;
            PlayedSong.IsPause = true;
            CurrentPauseState = true;
        }
        private void Stop()
        {
            Controller.Stop();
            PlayedSong.IsPlayed = false;
            PlayedSong.IsPause = true;
            CurrentPauseState = true;
        }


        private void PlayPrev()
        {
            if (songItems.Count == 0)
                return;
            Stop();
            if (ShuffleState)
            {
                SongItem songItem = PlayedSong;
                int i = 0;
                for (; i < playQueue.Count; i++)
                {
                    if (songItem.Number == playQueue[i])
                    {
                        break;
                    }
                }
                if (i>0)
                {
                    PlayedSong = songItems.First(s => s.Number == playQueue[i - 1]);
                }
                else
                {
                    PlayedSong = songItems.First(s => s.Number == playQueue[playQueue.Count-1]);
                }
            }
            else
            {
                SongItem songItem = PlayedSong;
                if (songItem.Number > 1)
                {
                    PlayedSong = songItems[songItem.Number - 2];
                }
                else
                {
                    PlayedSong = songItems[songItems.Count - 1];
                }
            }
            Play();
        }
        private void PlayNext()
        {
            if (songItems.Count == 0)
                return;
            Stop();
            if (ShuffleState)
            {
                SongItem songItem = PlayedSong;
                int i = 0;
                for (; i < playQueue.Count; i++)
                {
                    if (songItem.Number == playQueue[i])
                    {
                        break;
                    }
                }
                if (i+1 < playQueue.Count)
                {
                    PlayedSong = songItems.First(s=>s.Number == playQueue[i+1]);
                }
                else
                {
                    PlayedSong = songItems.First(s => s.Number == playQueue[0]);
                }
            }
            else
            {
                SongItem songItem = PlayedSong;
                if (songItem.Number < songItems.Count)
                {
                    PlayedSong = songItems[songItem.Number];
                }
                else
                {
                    PlayedSong = songItems[0];
                }
            }
            Play();
        }
        private List<int> CreateShuffleList()
        {
            List<int> list = new List<int>();
            for(int i = 0; i< SongItems.Count; i++)
            {
                list.Add(i + 1);
            }
            Random rnd = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
