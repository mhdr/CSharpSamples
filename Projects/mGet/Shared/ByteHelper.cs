using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class ByteHelper
    {
        public static int IndexOf(this byte[] searchIn, byte[] searchBytes, int start = 0)
        {
            int found = -1;
            bool matched = false;
            //only look at this if we have a populated search array and search bytes with a sensible start
            if (searchIn.Length > 0 && searchBytes.Length > 0 && start <= (searchIn.Length - searchBytes.Length) && searchIn.Length >= searchBytes.Length)
            {
                //iterate through the array to be searched
                for (int i = start; i <= searchIn.Length - searchBytes.Length; i++)
                {
                    //if the start bytes match we will start comparing all other bytes
                    if (searchIn[i] == searchBytes[0])
                    {
                        if (searchIn.Length > 1)
                        {
                            //multiple bytes to be searched we have to compare byte by byte
                            matched = true;
                            for (int y = 1; y <= searchBytes.Length - 1; y++)
                            {
                                if (searchIn[i + y] != searchBytes[y])
                                {
                                    matched = false;
                                    break;
                                }
                            }
                            //everything matched up
                            if (matched)
                            {
                                found = i;
                                break;
                            }

                        }
                        else
                        {
                            //search byte is only one bit nothing else to do
                            found = i;
                            break; //stop the loop
                        }

                    }
                }

            }
            return found;
        }

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

        public static List<long> SplitFileSize(long size, int split)
        {
            List<long> result=new List<long>();

            if (split == 1)
            {
                result.Add(size);
            }
            else
            {
                long size1 = Convert.ToInt32(size / split);
                long remainder = Convert.ToInt32(size % split);

                if (remainder == 0)
                {
                    for (int i = 1; i <= split; i++)
                    {
                        result.Add(size1);
                    }
                }
                else
                {
                    long size2 = 0;

                    for (int i = 1; i <= split - 1; i++)
                    {
                        result.Add(size1);
                        size2 += size1;
                    }

                    long lastSize = size - size2;
                    result.Add(lastSize);                    
                }

            }

            return result;
        }
    }
}
