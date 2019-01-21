using System.Collections.Generic;
using System.Linq;

namespace CommonCode.BusinessLayer.Helpers
{
    public class Neo4JHelper
    {
        public static string CreateNode(string nodeName, string nodeType,
            Dictionary<string, string> properties, bool includeValues = false)
        {
            if (!properties.Any())
                return "";

            var delimiter = ", ";
            var elements = properties.Keys.Select(x => $"{x}: {{{x}}}");

            var objectValues = string.Join(delimiter, elements);

            if (includeValues)
            {
                objectValues = objectValues.FormatFromDictionary(properties, true);
            }

            return $"({nodeName}:{nodeType} {{{objectValues}}})";
        }
    }
}
