using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public interface INetClient
    {
        long GetFileSize();
        void StartDownloadAsyn();
        bool SupportResume();

        event EventHandler<DownloadCompletedEventArgs> DownloadCompleted;
        event EventHandler<DownloadProgressEventArgs> DownloadProgress;
    }
}
