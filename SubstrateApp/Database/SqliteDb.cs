using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SubstrateApp.Database
{
    public static class SqliteDb
    {

        private static string DbPath { get { return Path.Combine(ApplicationData.Current.LocalFolder.Path, "substrateApp.db"); } }

        private static SqliteConnection sqliteConnection;

        public static SqliteConnection SqliteConnection
        {
            get
            {
                _ = sqliteConnection == null ? new SqliteConnection($"Filename={DbPath}") : sqliteConnection;
                sqliteConnection.Open();
                return sqliteConnection;
            }
        }

        public static async void InitializeDatabase()
        {
            String tableCommand = @"CREATE TABLE IF NOT EXISTS FilePathSearchLog (
                id int PRIMARY KEY, 
                path NVARCHAR(2048) NULL
                datetime datetime default current_timestamp) ";

            SqliteCommand createTable = new SqliteCommand(tableCommand, SqliteConnection);

            await createTable.ExecuteReaderAsync();
        }
    }
}
