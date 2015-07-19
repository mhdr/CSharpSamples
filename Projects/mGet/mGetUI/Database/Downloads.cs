using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations.Builders;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mGet.Database
{
    public class Downloads
    {
        public long DownloadId { get; set; }
        public string Url { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DownloadStatus Status { get; set; }
        public double Progress { get; set; }
        public DateTime AddedDate { get; set; }

        public static List<Downloads> GetDownloads()
        {
            SQLiteConnection connection=new SQLiteConnection(Statics.ConnectionString);
            connection.Open();

            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Downloads",connection);
            SQLiteDataReader reader = command.ExecuteReader();

            List<Downloads> result = new List<Downloads>();
            while (reader.Read())
            {
                Downloads download = new Downloads();

                dynamic fileSize = reader["FileSize"];
                dynamic status = reader["Status"];
                dynamic progress = reader["Progress"];
                dynamic addedDate = reader["AddedDate"];

                download.DownloadId = (long)reader["DownloadId"];
                download.Url = reader["Url"] as string;
                download.FileName = reader["FileName"] as string;

                if (fileSize > 0)
                {
                    download.FileSize = Convert.ToInt64(fileSize);
                }
                
                download.Status = (DownloadStatus)status;

                if (progress >= 0)
                {
                    download.Progress = (double)progress;
                }

                download.AddedDate = Convert.ToDateTime(addedDate);

                result.Add(download);
            }

            reader.Close();
            connection.Close();

            return result;
        }

        public static List<Downloads> GetDownloads(Func<Downloads,bool> predicate)
        {
            SQLiteConnection connection = new SQLiteConnection(Statics.ConnectionString);
            connection.Open();

            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Downloads", connection);
            SQLiteDataReader reader = command.ExecuteReader();

            List<Downloads> result = new List<Downloads>();
            while (reader.Read())
            {
                Downloads download = new Downloads();

                dynamic fileSize = reader["FileSize"];
                dynamic status = reader["Status"];
                dynamic progress = reader["Progress"];
                dynamic addedDate = reader["AddedDate"];

                download.DownloadId = (long)reader["DownloadId"];
                download.Url = reader["Url"] as string;
                download.FileName = reader["FileName"] as string;

                if (fileSize > 0)
                {
                    download.FileSize = Convert.ToInt64(fileSize);
                }

                download.Status = (DownloadStatus)status;

                if (progress >= 0)
                {
                    download.Progress = (double)progress;
                }

                download.AddedDate = Convert.ToDateTime(addedDate);

                if (predicate(download))
                {
                    result.Add(download);    
                }
            }

            reader.Close();
            connection.Close();

            return result;
        }

        public static Downloads GetDownload(Func<Downloads, bool> predicate)
        {
            SQLiteConnection connection = new SQLiteConnection(Statics.ConnectionString);
            connection.Open();

            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Downloads", connection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Downloads download = new Downloads();

                dynamic fileSize = reader["FileSize"];
                dynamic status = reader["Status"];
                dynamic progress = reader["Progress"];
                dynamic addedDate = reader["AddedDate"];

                download.DownloadId = (long)reader["DownloadId"];
                download.Url = reader["Url"] as string;
                download.FileName = reader["FileName"] as string;

                if (fileSize > 0)
                {
                    download.FileSize = Convert.ToInt64(fileSize);
                }

                download.Status = (DownloadStatus)status;

                if (progress >= 0)
                {
                    download.Progress = (double)progress;
                }

                download.AddedDate = Convert.ToDateTime(addedDate);

                if (predicate(download))
                {
                    reader.Close();
                    connection.Close();
                    return download;
                }
            }

            reader.Close();
            connection.Close();

            return null;
        }

        public static void Insert(Downloads value)
        {
            SQLiteConnection connection=new SQLiteConnection(Statics.ConnectionString);

            connection.Open();
            string commandText =
@"INSERT INTO Downloads(Url,FileName,FileSize,Status,Progress,AddedDate) VALUES(@Url,@FileName,@FileSize,@Status,@Progress,@AddedDate);
SELECT last_insert_rowid();";

            SQLiteCommand insertCommand = new SQLiteCommand(commandText,connection);
            insertCommand.Parameters.AddWithValue("@Url",value.Url);
            insertCommand.Parameters.AddWithValue("@FileName", value.FileName);
            insertCommand.Parameters.AddWithValue("@FileSize", value.FileSize);
            insertCommand.Parameters.AddWithValue("@Status", value.Status);
            insertCommand.Parameters.AddWithValue("@Progress", value.Progress);
            insertCommand.Parameters.AddWithValue("@AddedDate", value.AddedDate);

            var result = insertCommand.ExecuteScalar();

            if (result != null)
            {
                var id = Convert.ToInt64(result);
                value.DownloadId = id;
            }

            connection.Close();
        }
    }
}
