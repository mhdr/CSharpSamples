using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class mGetEnvironment
    {
        private static string _applicationDirectory;
        private static string _applicationOutputDirectory;

        public static string ApplicationDirectory
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string folderPtah = Path.Combine(path, "mGet");

                if (!Directory.Exists(folderPtah))
                {
                    Directory.CreateDirectory(folderPtah);
                }

                _applicationDirectory = folderPtah;
                return _applicationDirectory;
            }
        }

        public static string ApplicationOutputDirectory
        {
            get
            {
                _applicationOutputDirectory = KnownFolders.GetPath(KnownFolders.Downloads);
                return _applicationOutputDirectory;
            }
        }
    }
}
