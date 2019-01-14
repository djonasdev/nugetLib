using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NuGetLib
{
    public static class XLinqExtension
    {
        public static IEnumerable<XElement> ElementsAnyNamespace(this IEnumerable<XContainer> source, string localName)
        {
            return source.Elements().Where(e => e.Name.LocalName == localName);
        }

        public static XElement ElementAnyNamespace(this XContainer source, string localName)
        {
            return source.Elements().FirstOrDefault(e => e.Name.LocalName == localName);
        }

        public static IEnumerable<XElement> ElementsAnyNamespace(this XElement source, string localName)
        {
            return source.Elements().Where(e => e.Name.LocalName == localName);
        }

        public static XElement ElementAnyNamespace(this XElement source, string localName)
        {
            return source.Elements().FirstOrDefault(e => e.Name.LocalName == localName);
        }
    }
}