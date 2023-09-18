namespace SubstrateCore.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public static class XElementExtension
    {
        /// <summary>
        /// Clones a specified XElement.
        /// </summary>
        public static XElement Clone(this XElement element)
        {
            return new XElement(element);
        }

        /// <summary>
        /// Removes all the namespaces of this element and its descendants.
        /// </summary>
        public static void RemoveNamespace(this XElement element)
        {
            foreach (var child in element.DescendantsAndSelf())
            {
                child.Name = child.Name.LocalName;
            }
        }

        /// <summary>
        /// Determines whether the element is this tag.
        /// The comparison process is case-insensitive.
        /// </summary>
        public static bool Is(this XElement element, string tag)
        {
            return element.Name.LocalName.EqualsIgnoreCase(tag);
        }

        /// <summary>
        /// Attempts to remove this element from its parent element.
        /// </summary>
        public static bool TryRemove(this XElement element)
        {
            if (element.Parent != null)
            {
                element.Remove();
            }

            return element.Parent == null;
        }

        /// <summary>
        /// Removes this element from its parent if it has no child element.
        /// </summary>
        public static void RemoveIfEmpty(this XElement element)
        {
            if (!element.Elements().Any())
            {
                element.TryRemove();
            }
        }

        /// <summary>
        /// Gets the first descendant that matches the tag name.
        /// The comparison process is case-insensitive.
        /// </summary>
        public static XElement GetFirst(this XElement element, string tag)
        {
            return element?.GetAll(tag).FirstOrDefault();
        }

        /// <summary>
        /// Gets all descendants that match the tag name.
        /// The comparison process is case-insensitive.
        /// </summary>
        public static IEnumerable<XElement> GetAll(this XElement element, string tag)
        {
            return element?.Elements().Where(e => e.Name.LocalName.EqualsIgnoreCase(tag));
        }

        /// <summary>
        /// Gets the last sibling of this element.
        /// </summary>
        public static XElement LastSibling(this XElement element)
        {
            return element.ElementsBeforeSelf().LastOrDefault();
        }

        /// <summary>
        /// Gets the next sibling of this element.
        /// </summary>
        public static XElement NextSibling(this XElement element)
        {
            return element.ElementsAfterSelf().FirstOrDefault();
        }

        /// <summary>
        /// Gets the first attribute that matchs the attribute name.
        /// The comparison process is case-insensitive.
        /// </summary>
        public static XAttribute GetAttribute(this XElement element, string attrName)
        {
            return element.Attributes().Where(a => a.Name.LocalName.EqualsIgnoreCase(attrName)).FirstOrDefault();
        }

        /// <summary>
        /// Determines whether this element has a specific attribute.
        /// The comparison process is case-insensitive.
        /// </summary>
        public static bool HasAttribute(this XElement element, string attrName)
        {
            var attr = element.GetAttribute(attrName);
            return attr != null;
        }

        /// <summary>
        /// Determines whether this element has a specific attribute.
        /// The comparison process is case-insensitive.
        /// </summary>
        public static bool HasAttribute(this XElement element, string attrName, string attrValue)
        {
            var actualValue = element.GetAttribute(attrName)?.Value;
            if (!string.IsNullOrEmpty(actualValue))
            {
                return actualValue.Trim().EqualsIgnoreCase(attrValue.Trim());
            }
            return false;
        }

        /// <summary>
        /// Sorts the children elements with a specific attribute.
        /// The comparison process is case-insensitive.
        /// </summary>
        public static void SortByAttribute(this XElement parent, string attribute)
        {
            var children = parent.Elements().ToList();
            children.Sort((x, y) =>
                string.Compare(x.GetAttribute(attribute)?.Value,
                               y.GetAttribute(attribute)?.Value,
                               StringComparison.OrdinalIgnoreCase)
            );
            parent.RemoveNodes();
            foreach (var child in children) parent.Add(child);
        }

        public static XElement FirstChild(this XElement element, string name)
        {
            if (element == null)
                return null;
            return element.Elements().FirstOrDefault(a => a.Name.LocalName == name);
        }

        public static IEnumerable<XElement> AllChildren(this XElement element, string name)
        {
            if (element == null)
                return null;
            return element.Elements().Where(a => a.Name.LocalName == name);
        }

        public static XElement FirstDescendent(this XElement element, string name)
        {
            if (element == null)
                return null;
            return element.Descendants().FirstOrDefault(a => a.Name.LocalName == name);
        }

        public static IEnumerable<XElement> AllDescendent(this XElement element, string name)
        {
            if (element == null)
                return null;
            return element.Descendants().Where(a => a.Name.LocalName == name);
        }

        public static XElement FirstChildByAttribute(this XElement element, string elementName, string name, bool and = true, params string[] values)
        {
            if (element != null)
            {
                foreach (var child in element.AllChildren(elementName))
                {
                    var result = child.CheckAttribute(name, and, values);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        public static IEnumerable<XElement> AllChildByAttribute(this XElement element, string elementName, string name, bool and = true, params string[] values)
        {
            if (element != null)
            {
                foreach (var child in element.AllChildren(elementName))
                {
                    var result = child.CheckAttribute(name, and, values);
                    if (result != null)
                    {
                        yield return result;
                    }
                }
            }
        }

        public static IEnumerable<XElement> AllDescendentsByAttribute(this XElement element, string elementName, string name, bool and = true, params string[] values)
        {
            if (element != null)
            {
                foreach (var child in element.AllDescendent(elementName))
                {
                    var result = child.CheckAttribute(name, and, values);
                    if (result != null)
                    {
                        yield return result;
                    }
                }
            }
        }

        public static XElement FirstDescendentByAttribute(this XElement element, string elementName, string name, bool and = true, params string[] values)
        {
            if (element != null)
            {
                foreach (var child in element.AllDescendent(elementName))
                {
                    var result = child.CheckAttribute(name, and, values);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        public static XElement CheckAttribute(this XElement element, string name, bool and = true, params string[] values)
        {
            foreach (var attr in element.Attributes())
            {
                if (attr.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    if (values.Length > 0)
                    {
                        if (and)
                        {
                            foreach (var v in values)
                            {
                                if (!attr.Value.Contains(v, StringComparison.OrdinalIgnoreCase))
                                {
                                    return null;
                                }
                            }

                            return element;
                        }
                        else
                        {
                            foreach (var v in values)
                            {
                                if (attr.Value.Contains(v, StringComparison.OrdinalIgnoreCase))
                                {
                                    return element;
                                }
                            }
                        }

                    }
                    else
                    {
                        return element;
                    }
                }
            }

            return null;
        }
    }
}
