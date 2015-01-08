using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    class People
    {
        public string TableName { get; private set; }
        public SQLiteConnection Connection { get; private set; }
        public SQLiteTransaction Transaction { get; private set; }
        private bool _isInTransaction = false;

        public bool IsInTransaction
        {
            get
            {
                if (this.Connection == null || this.Transaction == null)
                {
                    _isInTransaction = false;
                }
                else
                {
                    _isInTransaction = true;
                }

                return _isInTransaction;
            }
            set { _isInTransaction = value; }
        }

        private string tableName = "People";
        public People()
        {
            
        }

        public static void CreateTable(string connectionString)
        {
            string tableName = "People";
            string command = string.Format(@"
DROP TABLE IF EXISTS {0};
CREATE TABLE {0} (
'PersonId'  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
'FirstName'  TEXT,
'Age'  INTEGER NOT NULL,
'BulkInsertSessionId' TEXT NULL,
'TimeStamp' TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
);", tableName);

            Table.CreateTable(connectionString, new SQLiteCommand(command));
        }

        public List<Person> GetPeople()
        {
            Table table=new Table(Statics.GetConnectionString(),this.tableName);

            List<Person> result=new List<Person>();

            foreach (DataRow row in table.DataTable.Rows)
            {
                Person person=new Person();
                person.PersonId = (int) row["PersonId"];
                person.FirstName = row["FirstName"] as string;
                person.Age =  (int) row["Age"];
                result.Add(person);
            }

            return result;
        }

        public List<Person> GetPeople(Func<Person,bool> predicate)
        {
            Table table = new Table(Statics.GetConnectionString(), this.tableName);

            List<Person> result = new List<Person>();

            foreach (DataRow row in table.DataTable.Rows)
            {
                Person person = new Person();
                person.PersonId = (int)row["PersonId"];
                person.FirstName = row["FirstName"] as string;
                person.Age = (int)row["Age"];

                if (predicate(person))
                {
                    result.Add(person);    
                }
            }

            return result;
        }

        public void Insert(Person value)
        {
            if (string.IsNullOrEmpty(this.TableName))
            {
                throw new NullReferenceException("TableName is Null");
            }

            Table table = null;
            if (this.IsInTransaction)
            {
                table = new Table(Statics.ConnectionString, this.Transaction, this.TableName);
            }
            else
            {
                table = new Table(Statics.ConnectionString, this.TableName);
            }

            SQLiteCommand insertCommand = table.CommandBuilder.GetInsertCommand().Clone() as SQLiteCommand;
            insertCommand.CommandText += ";SET @LastId= SCOPE_IDENTITY();";

            SQLiteParameter param = new SQLiteParameter();
            param.Direction = ParameterDirection.Output;
            param.DbType = DbType.Int32;
            param.ParameterName = "@LastId";

            insertCommand.Parameters.Add(param);
            table.DataAdapter.InsertCommand = insertCommand;

            DataRow newRow = table.DataTable.NewRow();
            newRow["ExcelName"] = value.FirstName;
            newRow["ExcelLabel"] = value.Age;
            newRow["BulkInsertSessionId"] = value.BulkInsertSessionId;
            table.DataTable.Rows.Add(newRow);
            table.DataAdapter.Update(table.DataTable);

            value.PersonId = (int)param.Value;
        }
    }
}
