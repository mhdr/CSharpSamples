using System;
using System.Runtime.InteropServices;

namespace Shared
{
    public static class KnownFolders
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out string pszPath);

        public static readonly Guid Downloads = new Guid("374DE290-123F-4565-9164-39C4925E467B");

        public static string GetPath(Guid knownFolder)
        {
            string path;

            SHGetKnownFolderPath(knownFolder, 0, IntPtr.Zero, out path);

            return path;
        }
    }
}