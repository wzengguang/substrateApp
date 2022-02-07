using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstrateCore.Models
{
    public class SearchInput
    {

        public bool IsPath { get; set; }


        public string Content { get; set; }

        public SearchInput(string content, bool ispath)
        {
            Content = content;
            IsPath = ispath;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this == null) return false;

            var other = obj as SearchInput;
            return other.Content == this.Content;
        }

        public override int GetHashCode()
        {
            return Content.GetHashCode();
        }
    }
}
