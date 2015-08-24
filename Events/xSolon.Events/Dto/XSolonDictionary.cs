using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Xml.Serialization;

namespace xSolon.Events.Dto
{
    /// <summary>
    /// Serializable dictionary implemented as a List of <see cref="NameValuePairs"/>
    /// </summary>
    [Serializable]
    public class XSolonDictionary : List<NameValuePair>
    {
        public XSolonDictionary()
        {
        }

        public void Add(string name, object value)
        {
            this.Add(new NameValuePair(name) { Value = value });
        }

        public XSolonDictionary(IDictionary<string, object> table)
        {
            var enumer = table.GetEnumerator();

            while (enumer.MoveNext())
            {
                this.Add(enumer.Current.Key, enumer.Current.Value);
            }
        }

        public XSolonDictionary(IDictionary<string, string> table)
        {
            var enumer = table.GetEnumerator();

            while (enumer.MoveNext())
            {
                this.Add(enumer.Current.Key, enumer.Current.Value);
            }
        }

        public XSolonDictionary(IDictionary table)
        {
            IDictionaryEnumerator enumerator = table.GetEnumerator();

            while (enumerator.MoveNext())
                this.Add(enumerator.Key.ToString(), enumerator.Value);
        }

        public object this[string key]
        {
            get
            {
                object valueObject = null;

                NameValuePair pair = this.Find(
                    delegate(NameValuePair nvp)
                    {
                        return nvp.Name == key;
                    });

                if (pair != null)

                    valueObject = pair.Value;

                return valueObject;
            }

            set
            {
                NameValuePair pair = this.Find(
                    delegate(NameValuePair nvp)
                    {
                        return nvp.Name == key;
                    });

                if (pair != null)

                    pair.Value = value;
            }
        }

        public bool ContainsKey(string name)
        {
            return this.FirstOrDefault(i => i.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) != null;
        }

        [XmlIgnore()]
        public Hashtable HashTable
        {
            get
            {
                Hashtable table = new Hashtable();

                foreach (NameValuePair pair in this)

                    if (!table.ContainsKey(pair.Name))

                        table.Add(pair.Name, pair.Value);

                return table;
            }
        }

        /// <summary>
        /// Returns a Dictionary where the values are casted as strings
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetDictionary()
        {

            var table = new Dictionary<string, string>();

            foreach (NameValuePair pair in this)
                if (!table.ContainsKey(pair.Name))
                    table.Add(pair.Name, pair.Value as string);

            return table;
        }
    }
}