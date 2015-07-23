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

        public DownloadProgressEventArgs(double value,long speed)
        {
            this.Value = value;
            this.Speed = speed;
        }
    }
}
