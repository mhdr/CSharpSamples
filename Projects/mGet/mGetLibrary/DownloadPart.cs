using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Http
{
    public class DownloadPart
    {
        private string _threadName;
        private long _startByte;
        private long _endByte;
        private long _downloadedBytes;
        private long _speed;
        private string _temporaryFileName;
        private DownloadStatus _status=DownloadStatus.NoStarted;

        public string ThreadName
        {
            get { return _threadName; }
            set { _threadName = value; }
        }

        public long StartByte
        {
            get { return _startByte; }
            set { _startByte = value; }
        }

        public long EndByte
        {
            get { return _endByte; }
            set { _endByte = value; }
        }

        public long DownloadedBytes
        {
            get { return _downloadedBytes; }
            set { _downloadedBytes = value; }
        }

        public long Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public string TemporaryFileName
        {
            get { return _temporaryFileName; }
            set { _temporaryFileName = value; }
        }

        public DownloadStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}
