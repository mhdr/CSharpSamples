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
        private int _numberOfConnections;

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

        public int NumberOfConnections
        {
            get { return _numberOfConnections; }
            set { _numberOfConnections = value; }
        }

        public DownloadProgressEventArgs(double value,long speed,int numberOfConnections)
        {
            this.Value = value;
            this.Speed = speed;
            this.NumberOfConnections = numberOfConnections;
        }
    }
}
