using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akkatecture.Exceptions;

namespace Akkatecture.Core
{
    public class MetadataContainer : Dictionary<string, string>
    {
        public MetadataContainer()
        {
        }

        public MetadataContainer(IDictionary<string, string> keyValuePairs)
            : base(keyValuePairs)
        {
        }

        public MetadataContainer(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
            : base(keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value))
        {
        }

        public MetadataContainer(params KeyValuePair<string, string>[] keyValuePairs)
            : this((IEnumerable<KeyValuePair<string, string>>)keyValuePairs)
        {
        }

        public void AddRange(params KeyValuePair<string, string>[] keyValuePairs)
        {
            AddRange((IEnumerable<KeyValuePair<string, string>>)keyValuePairs);
        }

        public void AddRange(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            foreach (var keyValuePair in keyValuePairs)
            {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, this.Select(kv => $"{kv.Key}: {kv.Value}"));
        }

        public string GetMetadataValue(string key)
        {
            return GetMetadataValue(key, s => s);
        }

        public T GetMetadataValue<T>(string key, Func<string, T> converter)
        {
            string value;

            if (!TryGetValue(key, out value))
            {
                throw new MetadataKeyNotFoundException(key);
            }

            try
            {
                return converter(value);
            }
            catch (Exception e)
            {
                throw new MetadataParseException(key, value, e);
            }
        }
    }
}
