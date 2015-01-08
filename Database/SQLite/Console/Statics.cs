using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    class Statics
    {
        public static string GetDatabasePath()
        {
            string fileName = "test.sqlite";
            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string path = Path.Combine(myDocumentsPath, fileName);

            return path;
        }

        public static string ConnectionString
        {
            get { return Statics.GetConnectionString(); }
        }

        public static string GetConnectionString()
        {
            string connectionString = string.Format("Data Source={0};Version=3;", Statics.GetDatabasePath());
            return connectionString;
        }
    }
}
