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

                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS ProjectInfo (Id INT PRIMARY KEY, " +
                    "Name TEXT NOT NULL UNIQUE," +
                    "ProjectType INT," +
                    "Framework NVARCHAR(200) NULL," +
                    "RelativePath NVARCHAR(1024) NULL)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
                db.Close();
            }
        }
    }
}
