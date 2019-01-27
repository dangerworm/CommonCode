using System.Collections.Generic;
using System.Linq;

namespace CommonCode.BusinessLayer.Helpers
{
    public class CypherHelper
    {
        public static string CreateNode(string nodeType, string nodeName,
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

        public static string CreateNode(string nodeType,
            string nodeName = null, string parameterName = null)
        {
            Verify.NotNull(nodeType, nameof(nodeType));

            var node = nodeType;

            if (!string.IsNullOrWhiteSpace(nodeName))
                node = $"{nodeName}:{node}";

            if (!string.IsNullOrWhiteSpace(parameterName))
                node += $" {{{parameterName}}}";

            return $"({node})";
        }

        public static string CreateRelationship(string relationshipType,
            string relationshipName = null, string parameterName = null)
        {
            Verify.NotNull(relationshipType, nameof(relationshipType));

            var relationship = relationshipType;

            if (!string.IsNullOrWhiteSpace(relationshipName))
                relationship = $"{relationshipName}:{relationship}";

            if (!string.IsNullOrWhiteSpace(parameterName))
                relationship += $" {{{parameterName}}}";

            return $"[{relationship}]";
        }
    }
}
