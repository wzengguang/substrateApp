using Microsoft.Data.Sqlite;
using SubstrateCore.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SubstrateCore.Repositories
{
    public class SQLiteDb
    {
        public async static void InitializeDb()
        {

            await ApplicationData.Current.LocalFolder.CreateFileAsync(AppSettings.DatabaseName, CreationCollisionOption.OpenIfExists);
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, AppSettings.DatabaseName);

            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                //string drop = "Drop table IF EXISTS ProjectInfo";
                //SqliteCommand dropTable = new SqliteCommand(drop, db);
                //dropTable.ExecuteReader();

                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Project (" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "Name VARCHAR(1024) NOT NULL COLLATE NOCASE," +
                    "ProjectType INT NOT NULL," +
                    "Framework VARCHAR(200) NOT NULL," +
                    "RelativePath VARCHAR(1024) NOT NULL UNIQUE COLLATE NOCASE," +
                    "Content TEXT NOT NULL);" +
                    "CREATE INDEX IF NOT EXISTS index_path ON Project (RelativePath);";
                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();

                db.Close();
            }
        }
    }
}
