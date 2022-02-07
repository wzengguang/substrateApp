namespace SubstrateCore.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public static class XDocumentExtension
    {
        /// <summary>
        /// Removes all the namespaces of this document and its descendants.
        /// </summary>
        public static void RemoveNamespace(this XDocument document)
        {
            document.Root.DescendantsAndSelf().ToList().ForEach(e => e.Name = e.Name.LocalName);
        }

        /// <summary>
        /// Gets the first descendant that matches the tag name.
        /// The comparison process is case-insensitive.
        /// </summary>
        public static XElement GetFirst(this XDocument document, string tag)
        {
            return document.GetDescendants(tag).FirstOrDefault();
        }

        /// <summary>
        /// Gets the last descendant that matches the tag name.
        /// The comparison process is case-insensitive.
        /// </summary>
        public static XElement GetLast(this XDocument document, string tag)
        {
            return document.GetDescendants(tag).LastOrDefault();
        }

        /// <summary>
        /// Gets all descendants that match the tag name.
        /// The comparison process is case-insensitive.
        /// </summary>
        public static IEnumerable<XElement> GetDescendants(this XDocument document, string tag)
        {
            return document.Descendants().Where(e => e.Name.LocalName.EqualsIgnoreCase(tag));
        }

        public static IEnumerable<XElement> GetDescendants(this XDocument document, params string[] tags)
        {
            return document.Descendants().Where(e => tags.Contains(e.Name.LocalName, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets all descendants that match the tags name.
        /// The comparison process is case-insensitive.
        /// </summary>
        public static IEnumerable<XElement> GetAll(this XDocument document, params string[] tags)
        {
            var result = new List<XElement>();
            tags.ToList().ForEach(tag => result.AddRange(document.GetDescendants(tag)));
            return result;
        }
    }
}
