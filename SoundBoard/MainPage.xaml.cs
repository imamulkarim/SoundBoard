using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SoundBoard.Resources;
using SoundBoard.ViewModels;

namespace SoundBoard
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;

            // Sample code to localize the ApplicationBar
            BuildLocalizedApplicationBar();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;

            if (selector == null)
            {
                return;
            }

            SoundData data = selector.SelectedItem as SoundData;

            if (data == null)
            {
                return;
            }

            if (File.Exists(data.FilePath))
            {
                AudioPlayer.Source = new Uri(data.FilePath, UriKind.RelativeOrAbsolute);

            }
            else
            {
                using (IsolatedStorageFile istoreage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(data.FilePath, FileMode.Open, istoreage))
                    {
                        AudioPlayer.SetSource(stream);
                    }
                }
            }


            selector.SelectedItem = null;
        }

        // Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/microphone.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarRecord;
            ApplicationBar.Buttons.Add(appBarButton);
            appBarButton.Click += appBarButton_Click;

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem aboutAppBar = new ApplicationBarMenuItem(AppResources.AppBarAbout);
            ApplicationBar.MenuItems.Add(aboutAppBar);
            aboutAppBar.Click += aboutAppBar_Click;
        }

        void aboutAppBar_Click(object sender, EventArgs e)
        {
            AboutPrompt aboutMe = new AboutPrompt();
            aboutMe.Show("Tonmoy", "@littleprograming.blogspot", "imamulkarim@hotmail.com", "www.littleprograming.blogspot.com");
        }

        void appBarButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AudioRecord.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}