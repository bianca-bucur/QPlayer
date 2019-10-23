using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Web;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Windows.Shell;

namespace QPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool mediaPlayerIsPlaying = false;
        public static bool timeSliderIsDragged = false;
        public static bool volumeSliderIsDragged = false;
        public static bool playlistInstanciated = false;
        public static string playIcon;
        public string PlayIcon { get { return @"D:\6. Coding\WPF\QPlayer\QPlayer\Resources\Play.ico"; } }

        public static Playlist playlist;

        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();            
            playlist = new Playlist(videoPlayer);
            playlist.ShowInTaskbar = false;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if ((videoPlayer.Source != null) && (videoPlayer.NaturalDuration.HasTimeSpan) && (!timeSliderIsDragged))
            {
                TimerSlider.Minimum = 0;
                TimerSlider.Maximum = videoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                TimerSlider.Value = videoPlayer.Position.TotalSeconds;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            WindowControlls.Close_Window(this);
            WindowControlls.Close_Window(playlist);
        }

        private void OpenPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (!playlist.playlistIsMinimized)
            {
                playlist.Show();
            }
            else
            {
                playlist.Visibility = Visibility.Visible;
            }
        }

        private void TimeSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            timeSliderIsDragged = true;
        }

        private void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimerStatus.Text = TimeSpan.FromSeconds(TimerSlider.Value).ToString(@"hh\:mm\:ss");
        }

        private void TimerSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            timeSliderIsDragged = false;
            videoPlayer.Position = TimeSpan.FromSeconds(TimerSlider.Value);
        }

        private void Drag_Window(object sender, MouseButtonEventArgs e)
        {
            WindowControlls.Drag_Window(this, e);
        }

        private void Volume_Change(object sender, MouseWheelEventArgs e)
        {
            if (videoPlayer.Volume >= 0 && e.Delta < 0)
            {
                videoPlayer.Volume -= 0.1;
            }
            if (videoPlayer.Volume <= 1 && e.Delta > 0)
            {
                videoPlayer.Volume += 0.1;
            }
            VolumeSlider.Value = videoPlayer.Volume;
        }

        private void Play_Media(object sender, RoutedEventArgs e)
        {
            MediaController.Play_Media(videoPlayer, play);
            VolumeSlider.Value = videoPlayer.Volume;
        }

        private void Pause_Media(object sender, RoutedEventArgs e)
        {
            videoPlayer.Pause();
        }

        private void Stop_Media(object sender, RoutedEventArgs e)
        {
            videoPlayer.Stop();
        }

        private void Mute_Media(object sender, RoutedEventArgs e)
        {
            videoPlayer.IsMuted = !videoPlayer.IsMuted;
        }
    }
}
