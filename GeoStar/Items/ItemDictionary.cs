using System.Collections;
using System.Collections.Generic;

namespace GeoStar.Items
{
    //only one instance of this class is allowed, maybe?
    class ItemDictionary : IDictionary<string, ItemBase>
    {
        Dictionary<string, ItemBase> itemDict;

        public ItemBase this[string key] { get => itemDict[key]; set => itemDict[key] = value; }

        public ICollection<string> Keys => itemDict.Keys;

        public ICollection<ItemBase> Values => itemDict.Values;

        public int Count => itemDict.Count;

        public bool IsReadOnly => true;

        public void Add(string key, ItemBase value)
        {
            itemDict.Add(key, value);
        }

        public void Add(KeyValuePair<string, ItemBase> item)
        {
            itemDict.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            itemDict.Clear();
        }

        public bool Contains(KeyValuePair<string, ItemBase> item)
        {
            return itemDict.ContainsKey(item.Key) && itemDict[item.Key] == item.Value;
        }

        public bool ContainsKey(string key)
        {
            return itemDict.ContainsKey(key);
        }

        //wont do shit
        public void CopyTo(KeyValuePair<string, ItemBase>[] array, int arrayIndex)
        {
            
        }

        public IEnumerator<KeyValuePair<string, ItemBase>> GetEnumerator()
        {
            return itemDict.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return itemDict.Remove(key);
        }

        public bool Remove(KeyValuePair<string, ItemBase> item)
        {
            return itemDict.Remove(item.Key);
        }

        public bool TryGetValue(string key, out ItemBase value)
        {
            if (itemDict.ContainsKey(key))
            {
                value = itemDict[key];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
