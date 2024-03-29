﻿using Microsoft.Data.Sqlite;
using SubstrateCore.Repositories;
using System;
using System.IO;
using Windows.Storage;

namespace SubstrateCore.Repository
{
    public class RepositoryBase : IDisposable
    {

        public SqliteConnection DbConnection { get; }
        public RepositoryBase()
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, SQLiteDb.DatabaseName);
            DbConnection = new SqliteConnection($"Filename={dbpath}");
            DbConnection.Open();
        }

        public void Dispose()
        {
            if (DbConnection != null)
            {
                DbConnection.Close();
                DbConnection.Dispose();
            }
        }
    }
}
