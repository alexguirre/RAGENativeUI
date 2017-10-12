namespace RAGENativeUI
{
    using System;
    using System.Dynamic;
    using System.Collections.Generic;

    public sealed class Metadata : DynamicObject
    {
        Dictionary<string, object> dictionary;

        public bool ContainsKey(string key)
        {
            Throw.IfNull(key, nameof(key));

            return dictionary != null && dictionary.ContainsKey(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            Throw.IfNull(key, nameof(key));

            value = null;
            return dictionary != null && dictionary.TryGetValue(key, out value);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            Throw.IfNull(key, nameof(key));
            
            if(TryGetValue(key, out object v))
            {
                if(v is T convertedValue)
                {
                    value = convertedValue;
                    return true;
                }
                else
                {
                    throw new InvalidOperationException($"Invalid cast from {v.GetType().Name} to {typeof(T).Name}");
                }
            }

            value = default;
            return false;
        }

        public Dictionary<string, object> ToDictionary()
        {
            return dictionary != null ? new Dictionary<string, object>(dictionary) : new Dictionary<string, object>(0);
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if(indexes.Length == 1)
            {
                if (indexes[0] is string name)
                {
                    GetValue(name, out result);
                    return true;
                }
            }

            result = null;
            return false;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Length == 1)
            {
                if (indexes[0] is string name)
                {
                    SetValue(name, value);
                    return true;
                }
            }
            
            return false;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            GetValue(binder.Name, out result);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetValue(binder.Name, value);
            return true;
        }


        private void GetValue(string name, out object result)
        {
            EnsureDictionary();
            if(!dictionary.TryGetValue(name, out result))
            {
                throw new KeyNotFoundException();
            }
        }

        private void SetValue(string name, object value)
        {
            EnsureDictionary();
            dictionary[name] = value;
        }

        private void EnsureDictionary()
        {
            if (dictionary == null)
                dictionary = new Dictionary<string, object>();
        }
    }
}

