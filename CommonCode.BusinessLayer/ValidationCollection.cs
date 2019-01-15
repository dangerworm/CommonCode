using CommonCode.BusinessLayer.Helpers;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace CommonCode.BusinessLayer
{
    public class ValidationCollection : NameValueCollection
    {
        public bool HasAny => Count > 0;

        public void Add(string key, string value, string prefix)
        {
            var keyName = string.IsNullOrWhiteSpace(prefix)
                ? key
                : $"{prefix}.{key}";

            Add(keyName, value);
        }

        public void Add(NameValueCollection collection, string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                Add(collection);
            }
            else
            {
                foreach (var key in collection.AllKeys)
                {
                    Add(key, collection[key], prefix);
                }
            }
        }

        public void AddRange(ValidationCollection validationCollection)
        {
            Add(validationCollection);
        }

        public bool ContainsKey(string key)
        {
            Verify.NotNull(key, nameof(key));

            return AllKeys.Any(x => KeyMatches(x, key));
        }

        public bool IsValidField(string key)
        {
            return !ContainsKey(key);
        }

        public new void Remove(string key)
        {
            var keys = AllKeys.Where(x => KeyMatches(x, key));

            foreach (var keyName in keys)
            {
                base.Remove(keyName);
            }
        }

        public IEnumerable<string> GetErrors()
        {
            return AllKeys.Select(key => this[key]);
        }

        private static bool KeyMatches(string key, string value)
        {
            return key == value || key.EndsWith($".{value}");
        }
    }
}
