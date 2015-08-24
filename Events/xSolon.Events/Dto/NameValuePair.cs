using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace xSolon.Events.Dto
{
    /// <summary>
    /// Represents a name and value pair. Used to serialize Hashtables and dictionaries
    /// </summary>
    [Serializable]
    public class NameValuePair
    {
        [XmlAttribute]
        public string Name;

        public object Value;

        public NameValuePair()
        {
        }

        public NameValuePair(string name)
            :
            this()
        {
            Name = name;
        }

        public NameValuePair(string name, object value)
            :
            this(name)
        {
            Value = value;
        }


    }
}
