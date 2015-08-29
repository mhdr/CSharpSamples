using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Shared
{
    [Serializable]
    public class DownloadInfoDb
    {
        private string _url;
        private string _referUrl;
        private long _fileSize;

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public long FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }

        public string ReferUrl
        {
            get { return _referUrl; }
            set { _referUrl = value; }
        }

        public static void SaveInfoToJSON(string fileDirectory,string url,string referUrl,long fileSize)
        {
            string infoFile = Path.Combine(fileDirectory, "DownloadInfoDb.json");

            if (File.Exists(infoFile))
            {
                return;
                // TODO check if file size is changed after adding to queue
            }

            DownloadInfoDb downloadInfoDb = new DownloadInfoDb();
            downloadInfoDb.Url = url;
            downloadInfoDb.FileSize = fileSize;
            downloadInfoDb.ReferUrl = referUrl;

            var json = JsonConvert.SerializeObject(downloadInfoDb, Formatting.Indented);
            FileStream fileStream = new FileStream(infoFile, FileMode.OpenOrCreate);
            StreamWriter streamWriter = new StreamWriter(fileStream);

            streamWriter.Write(json);

            streamWriter.Flush();
            streamWriter.Close();
            fileStream.Close();
        }

        public static DownloadInfoDb ReadInfoFromJSON(string fileDirectory)
        {
            string infoFile = Path.Combine(fileDirectory, "DownloadInfoDb.json");

            if (!File.Exists(infoFile))
            {
                return null;
                // TODO check 
            }

            FileStream fileStream = new FileStream(infoFile, FileMode.Open);
            StreamReader streamReader = new StreamReader(fileStream);
            var jsonStr = streamReader.ReadToEnd();
            DownloadInfoDb downloadInfoDb = JsonConvert.DeserializeObject<DownloadInfoDb>(jsonStr);


            streamReader.Close();
            fileStream.Close();

            return downloadInfoDb;
        }
    }
}
