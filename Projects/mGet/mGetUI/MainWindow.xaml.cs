using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using mGet.Database;
using mGetLibrary;

namespace mGet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DownloadsCollection _downloadsObservableCollection=new DownloadsCollection();

        public MainWindow()
        {
            InitializeComponent();
        }

        public DownloadsCollection DownloadsObservableCollection
        {
            get { return _downloadsObservableCollection; }
            set { _downloadsObservableCollection = value; }
        }

        private void MenuItemAddNewDownload_OnClick(object sender, RoutedEventArgs e)
        {
            WindowAddNewDownload windowAddNewDownload=new WindowAddNewDownload();
            windowAddNewDownload.CallerWindow = this;
            windowAddNewDownload.Show();
        }

        private void MenuItemExitApplication_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadListViewDownload();
        }

        private void LoadListViewDownload()
        {
            var downloads = Downloads.GetDownloads();

            foreach (Downloads download in downloads)
            {
                DownloadsObservableCollection.AddItem(download);
            }

            ListViewDownload.ItemsSource = DownloadsObservableCollection;
        }

        private void MenuItemDownload_OnSubmenuOpened(object sender, RoutedEventArgs e)
        {
            if (ListViewDownload.SelectedIndex >= 0)
            {
                MenuItemResumeDownload.IsEnabled = true;
                MenuItemPauseDownload.IsEnabled = true;
            }
        }

        private void MenuItemDownload_OnSubmenuClosed(object sender, RoutedEventArgs e)
        {
            MenuItemResumeDownload.IsEnabled = false;
            MenuItemPauseDownload.IsEnabled = false;
        }

        private void MenuItemResumeDownload_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListViewDownload.SelectedIndex >= 0)
            {
                vmDownload selected = (vmDownload) ListViewDownload.SelectedItem;

                if (selected.Status == DownloadStatus.Stopped)
                {
                    HttpDownloadManager httpDownloadManager = new HttpDownloadManager(selected.Url);
                    httpDownloadManager.DownloadId = selected.DownloadId;
                    httpDownloadManager.DownloadProgress += httpDownloadManager_DownloadProgress;
                    httpDownloadManager.DownloadCompleted += httpDownloadManager_DownloadCompleted;
                    selected.Status=DownloadStatus.Downloading;
        
                    Thread thread=new Thread(() =>
                    {
                        httpDownloadManager.StartDownload();
                    });

                    thread.Start();
                }
            }
        }

        void httpDownloadManager_DownloadCompleted(object sender, Shared.DownloadCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                
            }));
        }

        void httpDownloadManager_DownloadProgress(object sender, Shared.DownloadProgressEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                vmDownload selected = this.DownloadsObservableCollection.First(x => x.DownloadId == e.DownloadId);
                selected.ProgressValue = e.Value;
                selected.Speed = e.Speed;
            }));
        }
    }
}
