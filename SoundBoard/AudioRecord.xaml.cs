using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Coding4Fun.Toolkit.Audio;
using Coding4Fun.Toolkit.Audio.Helpers;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using SoundBoard.Resources;
using SoundBoard.ViewModels;

namespace SoundBoard
{
    public partial class AudioRecord : PhoneApplicationPage
    {

        MicrophoneRecorder _recorder = new MicrophoneRecorder();
        IsolatedStorageFileStream _audioStram;
        string _tempFileName = "tempwav.wav";
       
        public AudioRecord()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton recordAudioAppBar = new ApplicationBarIconButton(new Uri("/Assets/AppBar/save.png", UriKind.Relative));
            recordAudioAppBar.Text = AppResources.AppBarSave;
            recordAudioAppBar.Click += recordAudioAppBar_Click;
            
            ApplicationBar.Buttons.Add(recordAudioAppBar);
            ApplicationBar.IsVisible = false;
        
        }

        void recordAudioAppBar_Click(object sender, EventArgs e)
        {
            InputPrompt fileName = new InputPrompt();
            fileName.Title = "Sound Name";
            fileName.Message = "What should we call it";

            fileName.Completed += fileName_Completed;
            fileName.Show();

        }

        void fileName_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult == PopUpResult.Ok)
            {
                SoundData data = new SoundData();
                data.FilePath = string.Format("/customAudio/{0}.wav",DateTime.Now.ToFileTime());
                data.Title = e.Result;

                using (IsolatedStorageFile istoreage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!istoreage.DirectoryExists("/customAudio/"))
                    {
                        istoreage.CreateDirectory("/customAudio/");
                    }
                    istoreage.MoveFile(_tempFileName, data.FilePath);
                }

                App.ViewModel.CustomSounds.Items.Add(data);

                var JsonData = JsonConvert.SerializeObject(App.ViewModel.CustomSounds);

                IsolatedStorageSettings.ApplicationSettings[SoundModel.CustomSoundKey] = JsonData;
                IsolatedStorageSettings.ApplicationSettings.Save();

                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            PlayButton.IsEnabled = false;
            ApplicationBar.IsVisible = false;
            _recorder.Start();
            RotateCircle.Begin();
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            _recorder.Stop();
            SaveAudio(_recorder.Buffer);
            PlayButton.IsEnabled = true;
            ApplicationBar.IsVisible = true;
            RotateCircle.Stop();
        }

        private void SaveAudio(MemoryStream buffer)
        {
            if (buffer == null)
                throw new ArgumentException("Attempting to save and empty sound buffer");

            if (_audioStram != null)
            {
                AudioPlayer.Stop();
                AudioPlayer.Source = null;
                _audioStram.Dispose();
            }

            using (IsolatedStorageFile istoreage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (istoreage.FileExists(_tempFileName))
                {
                    istoreage.DeleteFile(_tempFileName);
                }

                _tempFileName = string.Format("{0}.wav", DateTime.Now.ToFileTime());

                var bytes = buffer.GetWavAsByteArray(_recorder.SampleRate);

                _audioStram = istoreage.CreateFile(_tempFileName);
                _audioStram.Write(bytes, 0, bytes.Length);

                AudioPlayer.SetSource(_audioStram);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AudioPlayer.Play();
        }


    }
}