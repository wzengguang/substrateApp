using Microsoft.Data.Sqlite;
using SubstrateCore.Models;
using SubstrateCore.Repository;
using SubstrateCore.Services;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstrateCore.Repositories
{
    public class SQLiteDataRepository : RepositoryBase
    {
        public async Task<List<ProjectInfo>> GetProjects()
        {
            var result = new List<ProjectInfo>();
            try
            {
                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT ProjectType,Framework,Name,RelativePath from ProjectInfo", DbConnection);

                SqliteDataReader query = await selectCommand.ExecuteReaderAsync();

                while (await query.ReadAsync())
                {
                    var pinfo = new ProjectInfo()
                    {
                        ProjectType = (ProjectTypeEnum)query.GetInt32(0),
                        Framework = query.GetString(1),
                        Name = query.GetString(2),
                        RelativePath = query.GetString(3),
                    };

                    result.Add(pinfo);
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return result;
        }

        public async Task AddProjects(List<ProjectInfo> projects)
        {
            foreach (var item in projects)
            {
                await AddProject(item);
            }
        }

        public async Task AddProject(ProjectInfo project)
        {
            try
            {
                SqliteCommand insertCommand = new SqliteCommand() { Connection = DbConnection };

                insertCommand.CommandText = "INSERT OR REPLACE INTO ProjectInfo (ProjectType,Framework,Name,RelativePath) VALUES (@ProjectType, @Framework,@Name,@RelativePath);";
                insertCommand.Parameters.AddWithValue("@ProjectType", (int)project.ProjectType);
                insertCommand.Parameters.AddWithValue("@Framework", project.Framework);
                insertCommand.Parameters.AddWithValue("@Name", project.Name);
                insertCommand.Parameters.AddWithValue("@RelativePath", project.RelativePath);

                await insertCommand.ExecuteReaderAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
