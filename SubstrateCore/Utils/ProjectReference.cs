using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SubstrateCore.Models;

namespace SubstrateCore.Utils
{
    public class ProjectReference
    {
        public void Run(string path)
        {
            string physicPath = SubstratePath.Combine(path);

            XDocument xDocument = XDocument.Load(physicPath);
            TreeNode treeNode = new TreeNode { NodeValue = path };
            FindNotNet6ProjectReference(xDocument, treeNode, 0);

            File.WriteAllLines("../../../notNet6", treeNode.ToList());

            var result2 = NotNet6ProjectReference(xDocument);

            xDocument.Save(physicPath);
        }

        public List<XElement> NotNet6ProjectReference(XDocument xDocument)
        {
            List<XElement> notnet6 = new List<XElement>();
            var pReferences = xDocument.Root.AllDescendent("ProjectReference").ToList();

            foreach (var pReference in pReferences)
            {
                string includePath = pReference.Attribute("Include")?.Value;
                if (includePath != null)
                {
                    string fullPath = SubstratePath.Combine(includePath.Replace("$(INETROOT)\\", string.Empty, StringComparison.OrdinalIgnoreCase));

                    var root = XDocument.Load(fullPath).Root;
                    var el = root.FirstChild("PropertyGroup");
                    string pg = el.Value;
                    if (pg != null && !pg.Contains("net6"))
                    {
                        notnet6.Add(pReference);
                        pReference.Remove();
                    }

                    if (root.CheckAttribute("Sdk") != null)
                    {
                        pReference.RemoveNodes();
                    }
                }
            }

            var ig = xDocument.Root.FirstDescendentByAttribute("ItemGroup", "Condition", true, "==", "net472");

            if (ig == null)
            {
                ig = new XElement("ItemGroup");
                ig.SetAttributeValue("Condition", @"'$(TargetFramework)' == 'net472'");
                xDocument.Root.AllDescendent("ItemGroup").Last().AddAfterSelf(ig);
            }


            ig.Add(notnet6);


            return notnet6;
        }

        public void FindNotNet6ProjectReference(XDocument xDocument, TreeNode treeNode, int deep, int maxDeep = 10)
        {
            deep++;

            var projectReferenceElements = xDocument.Root.AllDescendent("ProjectReference");

            foreach (var projectReferenceElement in projectReferenceElements)
            {
                string includePath = projectReferenceElement.Attribute("Include")?.Value;
                if (includePath != null)
                {
                    string fullPath = SubstratePath.Combine(includePath.ReplaceIgnoreCase("$(INETROOT)\\", string.Empty));

                    var propertyGroupElement = XDocument.Load(fullPath).Root.FirstChild("PropertyGroup");
                    string propertyGroupElementValue = propertyGroupElement?.Value;
                    if (propertyGroupElementValue != null && !propertyGroupElementValue.ContainIgnoreCase("net6"))
                    {
                        TreeNode childTree = new TreeNode { Parent = treeNode, NodeValue = includePath };

                        treeNode.Children.Add(childTree);
                        if (deep <= maxDeep)
                        {
                            XDocument xd = XDocument.Load(fullPath);

                            FindNotNet6ProjectReference(xd, childTree, deep);
                        }
                    }
                }
            }
        }
    }
}
