namespace RAGENativeUI
{
    using System.Dynamic;
    using System.Collections.Generic;

    public sealed class Metadata : DynamicObject
    {
        Dictionary<string, object> dictionary;

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if(indexes.Length == 1)
            {
                string name = indexes[0] as string;
                if(name != null)
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
                string name = indexes[0] as string;
                if (name != null)
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

