using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mGet;
using mGet.Database;

namespace mGet
{
    public class DownloadsCollection : ObservableCollection<vmDownload>
    {
        public void AddItem(vmDownload vmDownload)
        {
            this.Add(vmDownload);
        }

        public void AddItem(Downloads download)
        {
            vmDownload newVmDownload = new vmDownload();
            newVmDownload.Url = download.Url;
            newVmDownload.DownloadId = download.DownloadId;
            newVmDownload.FileName = download.FileName;
            if (download.FileSize >= 0)
            {
                newVmDownload.FileSize =download.FileSize;
            }
            
            newVmDownload.Status = download.Status;

            if (download.Progress >= 0)
            {
                newVmDownload.ProgressValue = download.Progress;    
            }
            
            newVmDownload.AddedDate = Convert.ToDateTime(download.AddedDate);

            this.Add(newVmDownload);
        }

    }
}
