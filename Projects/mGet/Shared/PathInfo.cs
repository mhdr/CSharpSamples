using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class PathInfo
    {
        public static string GetNextVacantNameForFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return filePath;
            }

            string path = filePath;
            int counter = 1;

            while (File.Exists(path))
            {
                counter++;
                string filePathWithoutExt = GetFilePathWithoutExtension(filePath);
                string ext = GetExtension(filePath);
                path = String.Format("{0}-{1}.{2}", filePathWithoutExt, counter,ext);
            }

            return path;
        }

        public static string GetNextVacantNameForDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return directoryPath;
            }

            string path = directoryPath;
            int counter = 1;

            while (Directory.Exists(path))
            {
                counter++;
                path = String.Format("{0}{1}", directoryPath, counter);
            }

            return path;
        }

        public static string GetFileName(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            return fileName;
        }

        public static string GetFilePathWithoutExtension(string filePath)
        {
            //string result = filePath.Substring(0, filePath.LastIndexOf(".", StringComparison.Ordinal));
            string root = Directory.GetParent(filePath).FullName;
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            string result = Path.Combine(root, fileNameWithoutExt);
            return result;
        }

        public static string GetExtension(string filePath)
        {
            string result = Path.GetExtension(filePath);
            return result;
        }
    }
}
