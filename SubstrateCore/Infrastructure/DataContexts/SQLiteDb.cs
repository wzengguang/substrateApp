using System;

using Microsoft.EntityFrameworkCore;
using SubstrateCore.Models;

namespace SubstrateCore.Datas
{
    public class SQLiteDb : DbContext, IDataSource
    {
        private string _connectionString = null;

        public SQLiteDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public DbSet<DbVersion> DbVersion { get; set; }

        public DbSet<ProjectInfo> Customers { get; set; }

    }
}
