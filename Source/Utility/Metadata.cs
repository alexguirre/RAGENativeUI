namespace RAGENativeUI.Utility
{
    using System;
    using System.Dynamic;
    using System.Collections.Generic;

    public class Metadata : DynamicObject
    {
        Dictionary<string, object> dictionary;
        
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            EnsureDictionary();
            
            return dictionary.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            EnsureDictionary();

            dictionary[binder.Name] = value;

            return true;
        }

        private void EnsureDictionary()
        {
            if (dictionary == null)
                dictionary = new Dictionary<string, object>();
        }
    }
}

