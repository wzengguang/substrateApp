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
    public class ProjectRepository : RepositoryBase
    {
        private SqliteCommand Command()
        {
            return new SqliteCommand() { Connection = DbConnection };
        }

        public async Task<List<Project>> GetProjects()
        {
            var result = new List<Project>();
            try
            {
                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT ProjectType,Framework,Name,RelativePath from Project", DbConnection);

                SqliteDataReader query = await selectCommand.ExecuteReaderAsync();

                while (await query.ReadAsync())
                {
                    var pinfo = new Project()
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

        public async Task<List<Project>> GetProjectByName(string name)
        {
            var result = new List<Project>();
            try
            {
                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT ProjectType,Framework,Name,RelativePath,Content from Project WHERE Name=@name", DbConnection);
                selectCommand.Parameters.AddWithValue("@name", name);

                SqliteDataReader query = await selectCommand.ExecuteReaderAsync();

                while (await query.ReadAsync())
                {
                    var pinfo = new Project()
                    {
                        ProjectType = (ProjectTypeEnum)query.GetInt32(0),
                        Framework = query.GetString(1),
                        Name = query.GetString(2),
                        RelativePath = query.GetString(3),
                        Content = query.GetString(4),
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

        public async Task<Project> GetProjectByPath(string path)
        {
            var result = new List<Project>();
            try
            {
                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT ProjectType,Framework,Name,RelativePath,Content from Project WHERE RelativePath=@path", DbConnection);
                selectCommand.Parameters.AddWithValue("@path", path);

                SqliteDataReader query = await selectCommand.ExecuteReaderAsync();

                while (await query.ReadAsync())
                {
                    var pinfo = new Project()
                    {
                        ProjectType = (ProjectTypeEnum)query.GetInt32(0),
                        Framework = query.GetString(1),
                        Name = query.GetString(2),
                        RelativePath = query.GetString(3),
                        Content = query.GetString(4),
                    };

                    return pinfo;
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }


        public async Task AddProjects(List<Project> projects)
        {
            foreach (var item in projects)
            {
                await AddProject(item);
            }
        }

        public async Task AddProject(Project project)
        {
            try
            {
                SqliteCommand insertCommand = new SqliteCommand() { Connection = DbConnection };

                insertCommand.CommandText = "INSERT OR REPLACE INTO Project (ProjectType,Framework,Name,RelativePath,Content) " +
                                            "VALUES (@ProjectType, @Framework,@Name,@RelativePath,@Content);";

                insertCommand.Parameters.AddWithValue("@ProjectType", (int)project.ProjectType);
                insertCommand.Parameters.AddWithValue("@Framework", project.Framework);
                insertCommand.Parameters.AddWithValue("@Name", project.Name);
                insertCommand.Parameters.AddWithValue("@RelativePath", project.RelativePath);
                insertCommand.Parameters.AddWithValue("@Content", project.Content);

                await insertCommand.ExecuteReaderAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + e.StackTrace);
            }
        }

        public async Task<int> CountProjectInfo()
        {
            SqliteCommand insertCommand = Command();
            insertCommand.CommandText = "SELECT COUNT(Id) FROM Project";
            var count = await insertCommand.ExecuteScalarAsync();
            return Convert.ToInt32(count);
        }
    }
}
