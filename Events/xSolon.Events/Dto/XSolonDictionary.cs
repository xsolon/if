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
    public class XSolonDictionary : List<NameValuePair> //, IDictionary
    {
        public XSolonDictionary()
        {
        }

        #region IDictionary
        ///// <summary>
        ///// Determines whether the <see cref="T:System.Collections.IDictionary" />
        ///// object contains an element with the specified key.
        ///// </summary>
        ///// <param name="key">The key to locate in the <see cref="T:System.Collections.IDictionary" />
        ///// object.</param>
        ///// <exception cref="T:System.ArgumentNullException">
        ///// <paramref name="key" /> is null. </exception>
        ///// <returns>
        ///// true if the <see cref="T:System.Collections.IDictionary" /> contains
        ///// an element with the key; otherwise, false.
        ///// </returns>
        //public bool Contains(object key)
        //{
        //    return this.ContainsKey(key.ToString());
        //}

        ///// <summary>
        ///// Adds an element with the provided key and value to the <see cref="T:System.Collections.IDictionary" />
        ///// object.
        ///// </summary>
        ///// <param name="key">The <see cref="T:System.Object" /> to use as the key of the
        ///// element to add.</param>
        ///// <param name="value">The <see cref="T:System.Object" /> to use as the value of
        ///// the element to add.</param>
        ///// <exception cref="T:System.ArgumentNullException">
        ///// <paramref name="key" /> is null. </exception>
        ///// <exception cref="T:System.ArgumentException">An element with the same key already
        ///// exists in the <see cref="T:System.Collections.IDictionary" /> object. </exception>
        ///// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary" />
        ///// is read-only.-or- The <see cref="T:System.Collections.IDictionary" /> has a fixed
        ///// size. </exception>
        //public void Add(object key, object value)
        //{
        //    if (Contains(key))
        //    {
        //        throw new ArgumentOutOfRangeException("Key already in dictionary");
        //    }

        //    Add(key.ToString(), value);
        //}

        ///// <summary>
        ///// Returns an <see cref="T:System.Collections.IDictionaryEnumerator" />
        ///// object for the <see cref="T:System.Collections.IDictionary" /> object.
        ///// </summary>
        ///// <returns>
        ///// An <see cref="T:System.Collections.IDictionaryEnumerator" /> object for
        ///// the <see cref="T:System.Collections.IDictionary" /> object.
        ///// </returns>
        //public new IDictionaryEnumerator GetEnumerator()
        //{
        //    return this.HashTable.GetEnumerator();
        //}

        ///// <summary>
        ///// Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary" />
        ///// object.
        ///// </summary>
        ///// <param name="key">The key of the element to remove.</param>
        ///// <exception cref="T:System.ArgumentNullException">
        ///// <paramref name="key" /> is null. </exception>
        ///// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary" />
        ///// object is read-only.-or- The <see cref="T:System.Collections.IDictionary" /> has
        ///// a fixed size. </exception>
        //public void Remove(object key)
        //{
        //    var elem = this.Find(i => object.Equals(i.Name, Values));

        //    if (elem != null)
        //    {

        //        this.Remove(elem);
        //    }
        //}

        ///// <summary>
        ///// Gets an <see cref="T:System.Collections.ICollection" /> object containing
        ///// the values in the <see cref="T:System.Collections.IDictionary" /> object.
        ///// </summary>
        ///// <returns>An <see cref="T:System.Collections.ICollection" /> object containing
        ///// the values in the <see cref="T:System.Collections.IDictionary" /> object.</returns>
        ///// <value></value>
        //public ICollection Values
        //{
        //    get
        //    {
        //        return this.Select(x => x.Value).ToList();
        //    }
        //}

        ///// <summary>
        ///// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary" />
        ///// object is read-only.
        ///// </summary>
        ///// <returns>true if the <see cref="T:System.Collections.IDictionary" /> object is
        ///// read-only; otherwise, false.</returns>
        ///// <value></value>
        //public bool IsReadOnly
        //{
        //    get
        //    {
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the <see cref="object" /> with the specified key.
        ///// </summary>
        ///// <value></value>
        //public object this[object key]
        //{
        //    get
        //    {
        //        return this[key.ToString()];
        //    }
        //    set
        //    {
        //        this[key.ToString()] = value;
        //    }
        //}

        ///// <summary>
        ///// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary" />
        ///// object has a fixed size.
        ///// </summary>
        ///// <returns>true if the <see cref="T:System.Collections.IDictionary" /> object has
        ///// a fixed size; otherwise, false.</returns>
        ///// <value></value>
        //public bool IsFixedSize
        //{
        //    get
        //    {
        //        // TODO: Implement this property getter
        //        throw new NotImplementedException();
        //    }
        //}

        ///// <summary>
        ///// Gets an <see cref="T:System.Collections.ICollection" /> object containing
        ///// the keys of the <see cref="T:System.Collections.IDictionary" /> object.
        ///// </summary>
        ///// <returns>An <see cref="T:System.Collections.ICollection" /> object containing
        ///// the keys of the <see cref="T:System.Collections.IDictionary" /> object.</returns>
        ///// <value></value>
        //public ICollection Keys
        //{
        //    get
        //    {
        //        return this.Select(x => x.Name).ToList();
        //    }
        //}

        #endregion
        public void Add(string name, object value)
        {

            if (this.ContainsKey(name))
            {
                throw new ArgumentException("Key already in dictionary {0}", name);
            }

            this.Add(new NameValuePair(name) { Value = value });
        }

        /// <summary>
        /// If there is an entry it will override its value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SafeAdd(string name, object value)
        {

            this[name] = value;

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
                else
                {
                    base.Add(new NameValuePair(key,value));
                }
            }
        }

        public bool ContainsKey(string name)
        {
            return this.FirstOrDefault(i => i.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) != null;
        }

        public Hashtable GetHashTable()
        {
            Hashtable table = new Hashtable();

            foreach (NameValuePair pair in this)

                if (!table.ContainsKey(pair.Name))

                    table.Add(pair.Name, pair.Value);

            return table;
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

        public override bool Equals(object obj)
        {
            var res = false;

            var dic = obj as XSolonDictionary;

            if (dic != null)
            {

                if (dic.Count == this.Count)
                {
                    foreach (var item in dic)
                    {

                        if (!this.ContainsKey(item.Name))
                        {

                            res = false; break;
                        }
                        else
                        {
                            var oValue = this[item.Name];

                            if (!object.Equals(item.Value, oValue))
                            {

                                res = false; break;
                            }
                        }

                    }

                    res = true;

                }

            }

            return res;

        }

    }
}