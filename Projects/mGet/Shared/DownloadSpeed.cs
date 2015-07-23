using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mGetLibrary
{
    public class DownloadSpeed
    {
        private long _downloadedSize = 0;
        private long _lastdownloadedSizeWhileCalculating = 0;
        private Timer timer;
        private long _speed = 0;

        public DownloadSpeed()
        {
            timer = new Timer(TimerTicks, null, 0, 1000);
        }

        public long DownloadedSize
        {
            get { return _downloadedSize; }
            set { _downloadedSize = value; }
        }

        public long LastdownloadedSizeWhileCalculating
        {
            get { return _lastdownloadedSizeWhileCalculating; }
            set { _lastdownloadedSizeWhileCalculating = value; }
        }

        /// <summary>
        /// Speed in Bytes
        /// </summary>
        public long Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        private void TimerTicks(object state)
        {
            Speed = DownloadedSize - LastdownloadedSizeWhileCalculating;
            LastdownloadedSizeWhileCalculating = DownloadedSize;
        }

        public void IncreaseDownloadedSize(long size)
        {
            lock (this)
            {
                DownloadedSize = DownloadedSize + size;
            }
        }
    }
}
