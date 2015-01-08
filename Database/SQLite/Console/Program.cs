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
            Table.CreateDatabase(Statics.GetDatabasePath());
            People.CreateTable(Statics.GetConnectionString());
        }


    }
}
