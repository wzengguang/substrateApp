using System;

using Microsoft.EntityFrameworkCore;

namespace SubstrateCore.Datas
{
    public class SQLServerDb : DbContext, IDataSource
    {
        private string _connectionString = null;

        public SQLServerDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public DbSet<DbVersion> DbVersion { get; set; }

    }
}
