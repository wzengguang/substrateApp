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
                //string drop = "Drop table ProjectInfo";
                //SqliteCommand dropTable = new SqliteCommand(drop, db);
                //dropTable.ExecuteReader();

                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS ProjectInfo (Id INT PRIMARY KEY, " +
                    "Name VARCHAR(1024) NOT NULL COLLATE NOCASE," +
                    "ProjectType INT," +
                    "Framework VARCHAR(200) NULL," +
                    "RelativePath VARCHAR(1024) NOT NULL UNIQUE COLLATE NOCASE," +
                    "Content TEXT);" +
                    "CREATE INDEX IF NOT EXISTS index_path ON ProjectInfo (RelativePath);";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();

                db.Close();
            }
        }
    }
}
