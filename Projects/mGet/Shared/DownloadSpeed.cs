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
        private long _fileSize = 0;
        private long _lastFileSizeWhileCalculating = 0;
        private Timer timer;
        private long _speed = 0;

        public DownloadSpeed()
        {
            timer = new Timer(TimerTicks, null, 0, 1000);
        }

        public long FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }

        public long LastFileSizeWhileCalculating
        {
            get { return _lastFileSizeWhileCalculating; }
            set { _lastFileSizeWhileCalculating = value; }
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
            Speed = FileSize - LastFileSizeWhileCalculating;
            LastFileSizeWhileCalculating = FileSize;
        }

        public void IncreaseFileSize(long size)
        {
            FileSize = FileSize + size;
        }
    }
}
