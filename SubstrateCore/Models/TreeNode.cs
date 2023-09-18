using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstrateCore.Models
{
    public class TreeNode
    {
        public TreeNode Parent { get; set; }

        public string NodeValue { get; set; }

        public List<TreeNode> Children { get; set; } = new List<TreeNode>();
    }
}
