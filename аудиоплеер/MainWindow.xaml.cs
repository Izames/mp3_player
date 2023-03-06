using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace аудиоплеер
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            var result =dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {

                string[] treks = Directory.GetFiles(dialog.FileName);
                List<String> list = new List<String>();
                foreach(string s in treks)
                {
                    string name = s[s.Length - 3].ToString() + s[s.Length - 2].ToString() + s[s.Length - 1].ToString();
                    if (name == "mp3" || name == "mp4" || name == "wav")
                    {
                        list.Add(s);
                    }
                }
                playlist.Items.Clear();
                playlist.ItemsSource = list;
                media.Source = new Uri(list[0]);
                media.Play();
            }
        }
        private void playlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            media.Source = new Uri(playlist.SelectedItem.ToString());
            media.Play();
            media.Volume = 0.9;
            
        }
        private void change_playlist_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                string[] treks = Directory.GetFiles(dialog.FileName);
                playlist.ItemsSource = treks;
                media.Source = new Uri(treks[0]);
                media.Play();
            }
        }
        private void media_MediaOpened_1(object sender, RoutedEventArgs e)
        {
            music_slider.Maximum = media.NaturalDuration.TimeSpan.Ticks;
            int x = playlist.SelectedIndex;
            bool f = true;
            Thread t = new Thread(_ => {
                while (f==true)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            music_slider.Value = media.Position.Ticks;
                            time_gone.Text = TimeConvert(music_slider.Value);
                            time_will.Text = TimeConvert(music_slider.Maximum - music_slider.Value);

                            if (music_slider.Value == music_slider.Maximum)
                            {
                                if (again.Content == "active")
                                {
                                    f = false;
                                    playlist.SelectedIndex += 0;
                                    music_slider.Value = 0;
                                }
                                else
                                {
                                    if (shuffle.Content == "active")
                                    {
                                        f = false;
                                        Random random = new Random();
                                        int y = random.Next(0, playlist.SelectedItems.Count);
                                        playlist.SelectedIndex = y;
                                        music_slider.Value = 0;
                                    }
                                    else
                                    {
                                        if (x == -1)
                                        {
                                            playlist.SelectedIndex += 2;
                                        }
                                        else if (x == playlist.Items.Count - 1)
                                        {
                                            playlist.SelectedIndex = 0;
                                        }
                                        else
                                        {
                                            playlist.SelectedIndex += 1;
                                        }
                                        music_slider.Value = 0;
                                        f = false;
                                    }
                                }
                            }
                            else
                            {
                                if (x != playlist.SelectedIndex)
                                {
                                    f = false;
                                }
                            }
                            if (change_playlist.ClickMode == ClickMode.Press)
                            {
                                f = false;
                            }
                    }));
                }               
                Thread.Sleep(1000);
            });
            t.Start();
        }
        private void music_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media.Position = new TimeSpan(Convert.ToInt64(music_slider.Value));
        }
        private void pause_Click(object sender, RoutedEventArgs e)
        {
            if (pause.Content=="playing")
            {
                media.Pause();
                pause.Content = "paused";
            }
            else
            {
                media.Play();
                pause.Content = "playing";
            }
        }
        private void back_Click(object sender, RoutedEventArgs e)
        {
            if (shuffle.Content == "active")
            {
                Random random = new Random();
                playlist.SelectedIndex= random.Next(0, playlist.Items.Count);
            }
            else
            {
                if (playlist.SelectedIndex == 0 || playlist.SelectedIndex == -1)
                {
                    playlist.SelectedIndex = playlist.Items.Count - 1;
                }
                else
                {
                    playlist.SelectedIndex -= 1;
                }
            }
        }
        private void next_Click(object sender, RoutedEventArgs e)
        {
            if (shuffle.Content == "active")
            {
                Random random = new Random();
                playlist.SelectedIndex = random.Next(0, playlist.Items.Count);
            }
            else
            {
                if (playlist.SelectedIndex == playlist.Items.Count - 1)
                {
                    playlist.SelectedIndex = 0;
                }
                else if (playlist.SelectedIndex == -1)
                {
                    playlist.SelectedIndex = 1;
                }
                else
                {
                    playlist.SelectedIndex += 1;
                }
            }
            
        }
        private void again_Click(object sender, RoutedEventArgs e)
        {
            if (again.Content == "active")
            {
                again.Content = "unactive";
                again.Background = new SolidColorBrush(Color.FromArgb(37, 37, 37, 1));
            }
            else
            {

                again.Content = "active";
                again.Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));

            }
        }
        private void shuffle_Click(object sender, RoutedEventArgs e)
        {
            if (shuffle.Content == "active")
            {
                shuffle.Content = "unactive";
                shuffle.Background = new SolidColorBrush(Color.FromArgb(37, 37, 37, 1));
            }
            else
            {

                shuffle.Content = "active";
                shuffle.Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));

            }
        }
        private string TimeConvert(double tick)
        {
            double pos = (tick / 10000000) - (tick % 10000000) * 0.0000001;
            String minute = (Convert.ToInt32(pos) / 60).ToString();
            string second = (pos % 60).ToString();
            if (minute == "0" || minute == "1" || minute == "2" || minute == "3" || minute == "4" || minute == "5" || minute == "6" || minute == "7" || minute == "8" || minute == "9")
            {
                minute = "0" + minute;
            }
            if (second == "0" || second == "1" || second == "2" || second == "3" || second == "4" || second == "5" || second == "6" || second == "7" || second == "8" || second == "9")
            {
                second = "0" + second;
            }
            return minute + ":" + second;
        }
    }
}
