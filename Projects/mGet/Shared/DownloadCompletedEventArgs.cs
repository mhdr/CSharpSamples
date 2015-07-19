using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class DownloadCompletedEventArgs:EventArgs
    {
        private string _outputFilePath;
        private long _downloadId;

        public DownloadCompletedEventArgs(string outputFilePath)
        {
            this.OutputFilePath = outputFilePath;
        }

        public DownloadCompletedEventArgs(long downloadId,string outputFilePath)
        {
            this.DownloadId = downloadId;
            this.OutputFilePath = outputFilePath;
        }

        public string OutputFilePath
        {
            get { return _outputFilePath; }
            set { _outputFilePath = value; }
        }

        public long DownloadId
        {
            get { return _downloadId; }
            set { _downloadId = value; }
        }
    }
}
