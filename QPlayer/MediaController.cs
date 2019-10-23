using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QPlayer
{
    public class MediaController
    {
        public Uri source;

        public static void Play_Media(object sender, Button play)
        {
            MediaElement videoPlayer = sender as MediaElement;
            if (MainWindow.mediaPlayerIsPlaying)
            {
                videoPlayer.Pause();
                MainWindow.mediaPlayerIsPlaying = false;
                play.Content = "Play";
            }
            else
            {
                videoPlayer.Play();
                MainWindow.mediaPlayerIsPlaying = true;
                play.Content = "Pause";
            }
        }
    }
}
