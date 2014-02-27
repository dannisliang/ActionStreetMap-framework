namespace Mercraft.Infrastructure.Config
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Contains functionality to merge configs
    /// </summary>
    public class ConfigMerger
    {
        static readonly XAttributeEqualityComparer AttrComparer = new XAttributeEqualityComparer();
        static readonly XNameComparer NameComparer = new XNameComparer();

        /// <summary>
        /// Merges two XElement instances
        /// </summary>
        public static XElement Merge(XElement e1, XElement e2)
        {
            //cannot merge null values
            if (e1 == null)
                return e2;
            if (e2 == null)
                return e1;

            var attributes = e2.Attributes().Union(e1.Attributes(), AttrComparer);

            var elements1 = e1.Elements().OrderBy(e => e.Name, NameComparer).ToArray();
            var elements2 = e2.Elements().OrderBy(e => e.Name, NameComparer).ToArray();
            var elements = new List<XNode>();
            int i1 = 0, i2 = 0;
            while (i1 < elements1.Length && i2 < elements2.Length)
            {
                XElement e = null;
                int compResult = NameComparer.Compare(elements1[i1].Name, elements2[i2].Name);
                if (compResult < 0)
                {
                    e = elements1[i1];
                    i1++;
                }
                else if (compResult > 0)
                {
                    e = elements2[i2];
                    i2++;
                }
                else
                {
                    e = Merge(elements1[i1], elements2[i2]);
                    i1++;
                    i2++;
                }
                elements.Add(e);
            }
            while (i1 < elements1.Length)
            {
                elements.Add(elements1[i1]);
                i1++;
            }
            while (i2 < elements2.Length)
            {
                elements.Add(elements2[i2]);
                i2++;
            }

            string value = null;
            if (elements.Count == 0)
            {
                if (!string.IsNullOrEmpty(e1.Value))
                    value = e1.Value;
                if (!string.IsNullOrEmpty(e2.Value))
                    value = e2.Value;
            }

            return value != null ? 
                new XElement(e1.Name, attributes, elements, value) : 
                new XElement(e1.Name, attributes, elements);
        }

        #region Nested classes

        class XNameComparer : IComparer<XName>
        {
            public int Compare(XName x, XName y)
            {
                int result = string.Compare(x.Namespace.NamespaceName, y.Namespace.NamespaceName);
                if (result == 0)
                    result = string.Compare(x.LocalName, y.LocalName);
                return result;
            }
        }

        class XAttributeEqualityComparer : IEqualityComparer<XAttribute>
        {
            public bool Equals(XAttribute x, XAttribute y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(XAttribute x)
            {
                return x.Name.GetHashCode();
            }
        }

        #endregion
    }
}
