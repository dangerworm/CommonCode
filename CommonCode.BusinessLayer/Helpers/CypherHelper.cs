using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommonCode.BusinessLayer.Helpers
{
    public class CypherHelper
    {
        public static string CreateNode(string nodeType, string nodeName = null, string parameterName = null)
        {
            return Create(nodeType, nodeName, parameterName, "(", ")");
        }
        public static string CreateNode(string nodeType, string nodeName, Dictionary<string, string> properties, bool includeValues = false)
        {
            return Create(nodeType, nodeName, properties, includeValues, "(", ")");
        }

        public static string CreateNode<T>(string nodeType, string nodeName, T node, bool includeValues = false)
        {
            return Create(nodeType, nodeName, node, includeValues, "(", ")");
        }

        public static string CreateRelationship(string relationshipType, string relationshipName = null, string parameterName = null)
        {
            return Create(relationshipType, relationshipName, parameterName, "[", "]");
        }

        public static string CreateRelationship(string relationshipType, string relationshipName, Dictionary<string, string> properties, bool includeValues = false)
        {
            return Create(relationshipType, relationshipName, properties, includeValues, "[", "]");
        }

        public static string CreateRelationship<T>(string relationshipType, 
            string relationshipName, T relationship, bool includeValues = false)
        {
            return Create(relationshipType, relationshipName, relationship, includeValues, "[", "]");
        }

        private static string Create(string type, string name, string parameterName, 
            string prefix, string suffix)
        {
            Verify.NotNull(type, nameof(type));

            var body = type;

            if (!string.IsNullOrWhiteSpace(name))
                body = $"{name}:{body}";

            if (!string.IsNullOrWhiteSpace(parameterName))
                body += $" {{{parameterName}}}";

            return $"{prefix}{body}{suffix}";
        }

        private static string Create(string type, string name, 
            Dictionary<string, string> properties, bool includeValues, string prefix, string suffix)
        {
            Verify.NotNull(type, nameof(type));

            if (!properties.Any())
                return "";

            var objectValues = GetObjectValues(properties, includeValues);
            return $"{prefix}{name}:{type} {{{objectValues}}}{suffix}";
        }

        private static string Create<T>(string type, string name, T node, 
            bool includeValues, string prefix, string suffix)
        {
            var properties = node.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(prop => prop.GetValue(node, null) != null)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(node, null).ToString());

            return Create(type, name, properties, includeValues, prefix, suffix);
        }

        private static string GetObjectValues(Dictionary<string, string> properties, bool includeValues)
        {
            var delimiter = ", ";
            var elements = properties.Keys.Select(x => $"{x}: {{{x}}}");
            var objectValues = string.Join(delimiter, elements);

            if (includeValues)
                objectValues = objectValues.FormatFromDictionary(properties, !properties.Values.Where(x => !string.IsNullOrWhiteSpace(x)).All(x => x.StartsWith("\"")));

            return objectValues;
        }
    }
}
