using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstrateApp.Database
{
    public static class SearchService
    {
        public static List<string> GetRecentSearchFilePath()
        {
            List<string> list = new List<string>();
            SqliteCommand selectCommand = new SqliteCommand
             ("SELECT path from FilePathSearchLog ORDER BY datetime DESC", SqliteDb.SqliteConnection);

            SqliteDataReader query = selectCommand.ExecuteReader();

            while (query.Read())
            {
                list.Add(query.GetString(0));
            }

            return list;
        }

        public static void InsertRecentSearchFilePath(string path)
        {
            SqliteCommand insertCommand = new SqliteCommand();
            insertCommand.Connection = SqliteDb.SqliteConnection;

            insertCommand.CommandText = "INSERT INTO FilePathSearchLog [(path)] VALUES (NULL, @path);";
            insertCommand.Parameters.AddWithValue("@path", path);

            insertCommand.ExecuteReader();

        }
    }

}
