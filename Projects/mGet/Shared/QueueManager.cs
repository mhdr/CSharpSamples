using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class QueueManager
    {
        public List<Tuple<string, string>> GetQueueList()
        {
            // directoryName,url
            List<Tuple<string, string>> result = new List<Tuple<string, string>>();

            var dirs = Directory.GetDirectories(mGetEnvironment.ApplicationDirectory);

            foreach (string d in dirs)
            {
                DownloadInfoDb infoDb = DownloadInfoDb.ReadInfoFromJSON(d);
                Tuple<string,string> q=new Tuple<string,string>(d,infoDb.Url);
                result.Add(q);
            }

            return result;
        }
    }
}
