using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubstrateCore.Models;

namespace SubstrateCore
{
    public static class TreeNodeExtension
    {
        public static List<string> ToList(this TreeNode node)
        {
            List<string> strings = new List<string>();
            TraverseTree(node, ref strings);
            return strings;
        }

        private static void TraverseTree(this TreeNode node, ref List<string> strings, int deep = 0)
        {
            if (node != null)
            {
                string span = new string(' ', deep);
                strings.Add(span + node.NodeValue);

                foreach (var child in node.Children)
                {
                    TraverseTree(child, ref strings, deep++);
                }
            }
        }
    }
}
