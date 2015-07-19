using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class DownloadInfo
    {
        public static string PrettySize(long sizeInByte, bool showInBytes = false)
        {
            string fileSizeMsg = "";
            double baseSize = 1024.0;

            if (showInBytes)
            {
                fileSizeMsg = String.Format("{0:N0} B", sizeInByte);
            }
            else
            {
                if (sizeInByte < 1024)
                {
                    fileSizeMsg = String.Format("{0:N0} B", sizeInByte);
                }
                else if (sizeInByte >= 1024 && sizeInByte < 1024 * 1024)
                {
                    double size = sizeInByte / baseSize;
                    fileSizeMsg = String.Format("{0:F2} KB", size);
                }
                else if (sizeInByte >= 1024 * 1024 && sizeInByte < 1024 * 1024 * 1024)
                {
                    double size = sizeInByte / (baseSize * baseSize);
                    fileSizeMsg = String.Format("{0:F2} MB", size);
                }
                else if (sizeInByte >= 1024 * 1024 * 1024)
                {
                    double size = sizeInByte / (baseSize * baseSize * baseSize);
                    fileSizeMsg = String.Format("{0:F2} GB", size);
                }
            }

            return fileSizeMsg;
        }
    }
}
