using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstrateCore.Utils
{
    public static class PathUtils
    {
        /// <summary>
        /// Gets the application root directory.
        /// </summary>
        public static string ApplicationRoot()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Directory.GetParent(baseDirectory).ToString();
        }

        /// <summary>
        /// Gets the folder application root directory.
        /// </summary>
        public static string ApplicationFolder(string folder)
        {
            var root = PathUtils.ApplicationRoot();
            return Path.GetFullPath(Path.Combine(root, folder));
        }

        /// <summary>
        /// Gets the absolute path for the specified path with its parent.
        /// </summary>
        public static string GetAbsolutePath(string relativeTo, string path)
        {
            path = path.StartsWith("\\") ? path.Substring(1) : path;
            return Path.GetFullPath(Path.Combine(relativeTo, path));
        }

        /// <summary>
        /// Gets the relative path from one path to another.
        /// </summary>
        public static string GetRelativePath(string relativeTo, string path)
        {
            return Path.GetRelativePath(relativeTo, path);
        }
    }
}
