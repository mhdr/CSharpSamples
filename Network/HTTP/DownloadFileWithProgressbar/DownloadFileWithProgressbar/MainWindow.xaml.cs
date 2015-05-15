using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
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
using System.Windows.Navigation;
using System.Xml;
using Path = System.Windows.Shapes.Path;

namespace DownloadFileWithProgressbar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public MainWindow()
        {
            InitializeComponent();
            AllocConsole();
        }

        private void ButtonDownload_OnClick(object sender, RoutedEventArgs e)
        {
            string fileName = TextBoxFile.Text;
            Thread thread=new Thread(()=>DownloadFile(fileName));
            thread.Start();
        }

        private void DownloadFile(string fileName)
        {   
            var host = "127.0.0.1";
            var port = 8080;

            var uri = string.Format("http://{0}:{1}/{2}", host, port, fileName);

            var request = WebRequest.CreateHttp(uri);
            var response = request.GetResponse();

            var responseStream = response.GetResponseStream();

            var length = response.ContentLength;

            var newFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

            var writer = new FileStream(newFilePath, FileMode.Create);
            writer.Flush();
            writer.Close();

            writer = new FileStream(newFilePath, FileMode.Append);

            var buffer = new byte[1024*1000];
            int bytesRead;
            double tolalRead=0;
            double lengthDouble = length;

            while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                writer.Write(buffer, 0, bytesRead);
                tolalRead = tolalRead + bytesRead;

                double progress = (tolalRead/lengthDouble)*100;
                Console.WriteLine(progress);
                Dispatcher.BeginInvoke(new Action(() => DownloadFile_Progress(progress)));
            }

            writer.Flush();
            writer.Close();
        }

        private void DownloadFile_Progress(double value)
        {
            ProgressBarDownload.Value = value;
        }
    }
}
