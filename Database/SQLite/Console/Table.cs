using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public class Table
    {
        public SQLiteDataAdapter DataAdapter { set; get; }
        public DataTable DataTable { get; set; }
        public SQLiteCommandBuilder CommandBuilder { get; set; }
        public string ConnectionString { get; set; }
        public string TableName { get; set; }

        private int _updateBatchSize = 1;

        private SQLiteConnection _connection;
        private SQLiteTransaction _transaction;
        private bool _isInTransaction = false;

        public SQLiteConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        public SQLiteTransaction Transaction
        {
            get { return _transaction; }
            set { _transaction = value; }
        }

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

        public int UpdateBatchSize
        {
            get { return _updateBatchSize; }
            set
            {
                _updateBatchSize = value;
                DataAdapter.UpdateBatchSize = _updateBatchSize;
            }
        }

        public Table(string connectionString, string tableName)
        {
            this.ConnectionString = connectionString;
            SQLiteConnection connection = new SQLiteConnection(this.ConnectionString);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand();
            command.Connection = connection;
            this.TableName = tableName;
            command.CommandText = string.Format(@"SELECT * FROM {0}", tableName);


            DataAdapter = new SQLiteDataAdapter(command);
            CommandBuilder = new SQLiteCommandBuilder(DataAdapter);
            DataTable = new DataTable(tableName);
            DataAdapter.Fill(DataTable);
            connection.Close();
        }

        public Table(string connectionString, string tableName, SQLiteCommand command)
        {
            this.ConnectionString = connectionString;
            SQLiteConnection connection = new SQLiteConnection(this.ConnectionString);
            connection.Open();
            command.Connection = connection;
            this.TableName = tableName;
            DataAdapter = new SQLiteDataAdapter(command);
            CommandBuilder = new SQLiteCommandBuilder(DataAdapter);
            DataTable = new DataTable(tableName);
            DataAdapter.Fill(DataTable);
            connection.Close();
        }

        public Table(string connectionString, SQLiteTransaction transaction, string tableName)
        {
            this.ConnectionString = connectionString;
            SQLiteConnection connection = transaction.Connection;
            SQLiteCommand command = new SQLiteCommand();
            command.Connection = connection;
            command.Transaction = transaction;
            command.CommandText = string.Format(@"SELECT * FROM {0}", tableName);
            this.TableName = tableName;
            DataAdapter = new SQLiteDataAdapter(command);
            CommandBuilder = new SQLiteCommandBuilder(DataAdapter);
            DataTable = new DataTable(tableName);
            DataAdapter.Fill(DataTable);
        }

        public Table(string connectionString, SQLiteTransaction transaction, string tableName, SQLiteCommand command)
        {
            this.ConnectionString = connectionString;
            SQLiteConnection connection = transaction.Connection;
            command.Connection = connection;
            command.Transaction = transaction;
            this.TableName = tableName;
            DataAdapter = new SQLiteDataAdapter(command);
            CommandBuilder = new SQLiteCommandBuilder(DataAdapter);
            DataTable = new DataTable(tableName);
            DataAdapter.Fill(DataTable);
        }

        public void DropTable()
        {
            if (IsInTransaction)
            {
                SQLiteConnection connection = Transaction.Connection;
                SQLiteCommand command = new SQLiteCommand();

                command.Connection = connection;
                command.Transaction = Transaction;

                string commandText5 = string.Format(
    @"DROP Table {0};", TableName);

                command.CommandText = commandText5;
                command.ExecuteNonQuery();
            }
            else
            {
                SQLiteConnection connection = null;
                SQLiteCommand command = new SQLiteCommand();

                connection = new SQLiteConnection(this.ConnectionString);
                connection.Open();
                command.Connection = connection;

                string commandText5 = string.Format(
    @"DROP Table {0};", TableName);

                command.CommandText = commandText5;
                command.ExecuteNonQuery();

                connection.Close();
            }

        }

        public static void CreateDatabase(string path)
        {
            SQLiteConnection.CreateFile(path);
        }

        public static void CreateTable(string connectionString, string tableName, SQLiteCommand command)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();
            command.Connection = connection;
            command.ExecuteNonQuery();
            connection.Clone();
        }

        public static void CreateTable(string connectionString,SQLiteTransaction transaction, string tableName, SQLiteCommand command)
        {
            SQLiteConnection connection = transaction.Connection;
            connection.Open();
            command.Connection = connection;
            command.Transaction = transaction;
            command.ExecuteNonQuery();
        }
    }
}
