using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using mGet.Database;
using mGetLibrary;
using Shared;

namespace mGet
{
    /// <summary>
    /// Interaction logic for WindowAddNewDownload.xaml
    /// </summary>
    public partial class WindowAddNewDownload : Window
    {
        private long? fileSizeInBytes;
        private Window _callerWindow;

        public WindowAddNewDownload()
        {
            InitializeComponent();
        }

        public Window CallerWindow
        {
            get { return _callerWindow; }
            set { _callerWindow = value; }
        }

        private void ButtonGetFileSize_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = TextBoxAddress.Text;
                if (!string.IsNullOrEmpty(url))
                {
                    ButtonGetFileSize.IsEnabled = false;

                    Thread thread=new Thread(() =>
                    {
                        HttpDownloadManager httpDownloadManager = new HttpDownloadManager(url);
                        fileSizeInBytes = httpDownloadManager.GetFileSize();
                        var prettySize = DownloadInfo.PrettySize((long)fileSizeInBytes);

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            TextBlockFileSize.Text = prettySize;
                            ButtonGetFileSize.IsEnabled = true;
                        }));

                    });

                    thread.Start();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                ButtonGetFileSize.IsEnabled = true;    
            }

            
        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            string url = TextBoxAddress.Text;

            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            string fileName = PathInfo.GetFileName(url);

            Downloads downloads=new Downloads();
            downloads.Url = url;
            downloads.FileName = fileName;
            
            if (fileSizeInBytes != null)
            {
                downloads.FileSize = (long) fileSizeInBytes;
            }
            downloads.Status=DownloadStatus.Stopped;
            downloads.AddedDate=DateTime.Now;

            Downloads.Insert(downloads);

            if (downloads.DownloadId > 0)
            {
                if (CallerWindow is MainWindow)
                {
                    MainWindow mainWindow = (MainWindow) CallerWindow;

                    mainWindow.DownloadsObservableCollection.AddItem(downloads);
                }
            }

            this.Close();
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
