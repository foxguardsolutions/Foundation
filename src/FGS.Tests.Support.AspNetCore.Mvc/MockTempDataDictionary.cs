using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FGS.Tests.Support.AspNetCore.Mvc
{
    internal class MockTempDataDictionary : ITempDataDictionary
    {
        private readonly IDictionary<string, object> _contents = new Dictionary<string, object>();

        public bool Remove(KeyValuePair<string, object> item) => _contents.Remove(item.Key);

        public int Count => _contents.Count;

        public void Add(KeyValuePair<string, object> item) => _contents.Add(item.Key, item.Value);

        public void Clear() => _contents.Clear();

        public bool Contains(KeyValuePair<string, object> item) => _contents.Any(i => i.Key == item.Key && i.Value == item.Value);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => _contents.ToArray().CopyTo(array, arrayIndex);

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => _contents.ToList().GetEnumerator();

        public IEnumerator GetEnumerator() => _contents.GetEnumerator();

        public bool IsReadOnly => _contents.IsReadOnly;

        public void Add(string key, object value) => _contents.Add(key, value);

        public bool ContainsKey(string key) => _contents.ContainsKey(key);

        public bool Remove(string key) => _contents.Remove(key);

        public bool TryGetValue(string key, out object value) => _contents.TryGetValue(key, out value);

        public object this[string key]
        {
            get => _contents[key];
            set => _contents[key] = value;
        }

        public ICollection<string> Keys => _contents.Keys;

        public ICollection<object> Values => _contents.Values;

        public void Load() => throw new NotSupportedException();

        public void Save() => throw new NotSupportedException();

        public void Keep() => throw new NotSupportedException();

        public void Keep(string key) => throw new NotSupportedException();

        public object Peek(string key) => throw new NotSupportedException();
    }
}
