using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "test.sqlite";
            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string path = Path.Combine(myDocumentsPath, fileName);

            string connectionString = string.Format("Data Source={0};Version=3;", path);

            Table.CreateDatabase(path);
            CreateTable(connectionString);
        }

        private static void CreateTable(string connectionString)
        {
            string tableName = "People";
            string command = string.Format(@"
DROP TABLE IF EXISTS {0};
CREATE TABLE {0} (
'PersonId'  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
'FirstName'  TEXT,
'Age'  INTEGER NOT NULL
);", tableName);

            Table.CreateTable(connectionString,new SQLiteCommand(command));
        }
    }
}
