using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using mGetUI.Annotations;
using Shared;

namespace mGet
{
    public class vmDownload : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long DownloadId
        {
            get { return _downloadId; }
            set
            {
                _downloadId = value;
                OnPropertyChanged();
            }
        }

        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                OnPropertyChanged();
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }

        public long FileSize
        {
            get { return _fileSize; }
            set
            {
                _fileSize = value;

                if (_fileSize > 0)
                {
                    _prettyFileSize = DownloadInfo.PrettySize((long)_fileSize);    
                }
                else
                {
                    _prettyFileSize = "";
                }
                
                OnPropertyChanged();
            }
        }

        public DownloadStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public double ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;

                if (_progressValue > 0)
                {
                    _prettyProgressValue = String.Format("{0:P2}", _progressValue);
                }
                else
                {
                    _prettyProgressValue = "";
                }

                OnPropertyChanged();
            }
        }

        public DateTime AddedDate
        {
            get { return _addedDate; }
            set
            {
                _addedDate = value;
                OnPropertyChanged();
            }
        }

        public long Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                _prettySpeed = DownloadInfo.PrettySize(_speed);
                OnPropertyChanged();
            }
        }

        public string PrettySpeed
        {
            get { return _prettySpeed; }
        }

        public string TimeRemaining
        {
            get { return _timeRemaining; }
            set
            {
                _timeRemaining = value;
                OnPropertyChanged();
            }
        }

        public string PrettyFileSize
        {
            get { return _prettyFileSize; }
        }

        public string PrettyProgressValue
        {
            get { return _prettyProgressValue; }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private long _downloadId;
        private string _url;
        private string _fileName;
        private long _fileSize;
        private string _prettyFileSize;
        private DownloadStatus _status;
        private double _progressValue;
        private string _prettyProgressValue;
        private DateTime _addedDate;

        private long _speed;
        private string _prettySpeed;
        private string _timeRemaining;
    }
}
