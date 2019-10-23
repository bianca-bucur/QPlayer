using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using System.Windows.Shell;
using Microsoft.WindowsAPICodePack.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;

namespace QPlayer
{
    /// <summary>
    /// Interaction logic for Playlist.xaml
    /// </summary>
    public partial class Playlist : Window
    {
        public ObservableCollection<Media> newFiles = new ObservableCollection<Media>();
        public ObservableCollection<Media> playlist_Items = new ObservableCollection<Media>();

        //public List<Media> newFiles = new List<Media>();
        //public List<Media> playlist_Items = new List<Media>();
        public MediaElement videoPlayer;
        public static int id { get; set; }
        public bool playlistIsMinimized = false;

        public Playlist(MediaElement videoPlayer)
        {
            InitializeComponent();
            //newFiles = new List<Media>();
            this.videoPlayer = videoPlayer;
            id = 1;
            playList.ItemsSource = playlist_Items;
            //playlist_Items=new List<Media>();
        }

        private void addFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();   
            
            openFileDialog.Filter = "Media files (*.mp3;*.mpg;*.mpeg;*.mkv)|*.mp3;*.mpg;*.mpeg;*.mkv|All files (*.*)|*.*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var media in openFileDialog.FileNames)
                {
                    newFiles.Add(new Media() { source= new Uri(media),  m_Title = media.Split('\\').Last() });
                    id++;
                }
            }

            foreach (var media in newFiles)
            {
                if (!playList.Items.Contains(media.m_Title))
                {
                    playlist_Items.Add(media);
                    //playList.Items.Add(media);
                }
            }
            newFiles.Clear();            
        }

        private void addFolder_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog() { IsFolderPicker = true };
            FileInfo[] files;
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok) 
            {
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(System.IO.Path.GetFullPath(openFileDialog.FileName));
                    files = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);

                    string[] allowedExtensions = new string[] { ".mp3", ".mpg", ".mpeg", ".mkv", ".avi" };

                    foreach (var media in files)
                    {
                        if (allowedExtensions.Contains(System.IO.Path.GetExtension(media.FullName)))
                        {
                            //newFiles.Add();
                            Media newMedia = new Media() { source = new Uri(media.FullName), m_Title = media.FullName.Split('\\').Last() };
                            if(!ContainsMedia(playlist_Items,newMedia))
                            playlist_Items.Add(newMedia);
                            //playList.Items.Add(new Media() { source = new Uri(media.FullName), m_ID = id, m_Title = media.FullName.Split('\\').Last() });
                            id++;
                            
                        }
                    }
                }
            }
            newFiles.Clear();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            WindowControlls.Close_Window(this);
        }

        private void Drag_Window(object sender, MouseButtonEventArgs e)
        {
            WindowControlls.Drag_Window(this, e);
        }

        private void Minimize_Window(object sender, RoutedEventArgs e)
        {
            WindowControlls.Minimize_Window(this);
            playlistIsMinimized = true;
        }

        private void SelectedMedia_DBClick(object sender, MouseButtonEventArgs e)
        {
            Media selItem = (Media)playList.SelectedItem;
            videoPlayer.Source = selItem.source;
            videoPlayer.Play();
        }

        private void ContextMenu_Open(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            ContextMenu ctxtMenu = btn.ContextMenu;
            ctxtMenu.PlacementTarget = btn;
            ctxtMenu.IsOpen = true;
        }

        private void SortByLabel(object sender, RoutedEventArgs e)
        {
            playList.Items.SortDescriptions.Clear();
            playList.Items.SortDescriptions.Add(new SortDescription("m_Title", ListSortDirection.Ascending));
        }

        private void RestoreSort(object sender, RoutedEventArgs e)
        {
            playList.Items.SortDescriptions.Clear();
            playList.Items.SortDescriptions.Add(new SortDescription("m_ID", ListSortDirection.Ascending));
        }

        private void RandomizeSort(object sender, RoutedEventArgs e)
        {
            playList.Items.SortDescriptions.Clear();
            ObservableCollection<Media> tempList = playlist_Items;
            tempList = Shuffle(tempList);
            playlist_Items.Clear();
            foreach (var media in tempList)
            {
                playlist_Items.Add(media);
            }
        }

        private ObservableCollection<Media> Shuffle(ObservableCollection<Media> theItems)
        {
            Random rnd = new Random();
            ObservableCollection<Media> tempList = new ObservableCollection<Media>();
            while (theItems.Count > 0)
            {
                int randomIndex = rnd.Next(0, theItems.Count - 1);
                tempList.Add(theItems[randomIndex]);
                theItems.RemoveAt(randomIndex);
            }
            return tempList; 
        }

        private bool ContainsMedia(ObservableCollection<Media> items, Media media)
        {
            foreach(var m in items)
            {
                if (m.m_Title.Equals(media.m_Title)) return true;
            }
            return false;
        }

        private void ClearPlaylist(object sender, RoutedEventArgs e)
        {
            playlist_Items.Clear();
        }

        private void RemoveMedia(object sender, RoutedEventArgs e)
        {
            while (playList.SelectedItem != null)
            {
            playlist_Items.Remove(playList.SelectedItem as Media);
            }
            CollectionViewSource.GetDefaultView(playlist_Items).Refresh();
        }

        void s_PreviewMouseMoveEvent(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem && e.LeftButton == MouseButtonState.Pressed)
            {
                ListBoxItem draggedItem = sender as ListBoxItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                draggedItem.IsSelected = true;
                CollectionViewSource.GetDefaultView(playlist_Items).Refresh();
            }
        }

        void listbox1_Drop(object sender, DragEventArgs e)
        {
            Media droppedData = e.Data.GetData(typeof(Media)) as Media;
            Media target = ((ListBoxItem)(sender)).DataContext as Media;

            int removedIdx = playList.Items.IndexOf(droppedData);
            int targetIdx = playList.Items.IndexOf(target);

            if (removedIdx < targetIdx)
            {
                playlist_Items.Insert(targetIdx + 1, droppedData);
                playlist_Items.RemoveAt(removedIdx);
            }
            else
            {
                int remIdx = removedIdx + 1;
                if (playlist_Items.Count + 1 > remIdx)
                {
                    playlist_Items.Insert(targetIdx, droppedData);
                    playlist_Items.RemoveAt(remIdx);
                }
            }
        }
    }

    

}

