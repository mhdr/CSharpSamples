using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class DownloadProgressEventArgs : EventArgs
    {
        private double _value;
        private long _speed;
        private long _downloadId;

        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public long Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public long DownloadId
        {
            get { return _downloadId; }
            set { _downloadId = value; }
        }

        public DownloadProgressEventArgs(long downloadId,double value, long speed)
        {
            this.DownloadId = downloadId;
            this.Value = value;
            this.Speed = speed;
        }

        public DownloadProgressEventArgs(double value,long speed)
        {
            this.Value = value;
            this.Speed = speed;
        }
    }
}
