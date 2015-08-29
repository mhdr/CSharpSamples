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

        public DownloadCompletedEventArgs(string outputFilePath)
        {
            this.OutputFilePath = outputFilePath;
        }

        public string OutputFilePath
        {
            get { return _outputFilePath; }
            set { _outputFilePath = value; }
        }
    }
}
