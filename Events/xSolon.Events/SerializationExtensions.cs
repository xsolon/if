// xSolon.Events SerializationExtensions.cs  8/24/2015 martin 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace xSolon.Events
{
    public static partial class SerializationExtensions
    {

        ///TODO: keep salt in obfuscated dll
        // Crypto - defined in SerializationExtensions.partial.cs
        // not stored in project as an addiional measure to protect the salt
        // uncomment if want to compiled
        //private static byte[] _salt = Encoding.ASCII.GetBytes("te806642kbM7c8");

        public static List<Type> KnownTypes = new List<Type>();

        public static List<Type> CleanKnownTypes()
        {
            var types = new List<Type>();

            if (SerializationExtensions.KnownTypes.Count == 0)
            {
                //types.Add(typeof(List));
            }

            SerializationExtensions.KnownTypes.ToArray().Reverse().ToList().ForEach(i =>
            {
                if (types.FirstOrDefault(j => j.Name == i.Name) == null)
                {
                    types.Add(i);
                }
            });

            types.Add(typeof(DBNull));

            return types;
        }

        /// <summary> 
        /// Decrypt the given string.  Assumes the string was encrypted using  
        /// EncryptStringAES(), using an identical sharedSecret. 
        /// </summary> 
        /// <param name="cipherText">The text to decrypt.</param> 
        /// <param name="sharedSecret">A password used to generate a key for decryption.</param> 
        public static string DecryptStringAES(this string cipherText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentNullException("cipherText");
            }

            if (string.IsNullOrEmpty(sharedSecret))
            {
                throw new ArgumentNullException("sharedSecret");
            }

            // Declare the RijndaelManaged object 

            // used to decrypt the data. 

            RijndaelManaged aesAlg = null;

            // Declare the string used to hold 

            // the decrypted text. 

            string plaintext = null;

            try
            {
                // generate the key from the shared secret and the salt 
                var key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Create the streams used for decryption.                 

                byte[] bytes = Convert.FromBase64String(cipherText);

                using (var msDecrypt = new MemoryStream(bytes))
                {
                    // Create a RijndaelManaged object 
                    // with the specified key and IV. 
                    aesAlg = new RijndaelManaged();

                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                    // Get the initialization vector from the encrypted stream 

                    aesAlg.IV = ReadByteArray(msDecrypt);

                    // Create a decrytor to perform the stream transform. 

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string. 
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object. 
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }

            return plaintext;
        }

        /// <summary>
        /// Desearializes a string to the type specified.
        /// </summary>
        /// <param name="type">Type to desearialize to.</param>
        /// <param name="rawValue">string represenation of the object</param>
        /// <returns>Instance of the object.</returns>
        public static object Desearilize(Type type, string rawValue)
        {
            var serializer = new XmlSerializer(type);

            XmlReader reader = System.Xml.XmlReader.Create(new System.IO.StringReader(rawValue));

            return serializer.Deserialize(reader);
        }

        public static T Desearilize<T>(this string rawValue, Type[] additionalTypes) where T : class , new()
        {
            if (additionalTypes == null)
            {
                additionalTypes = CleanKnownTypes().ToArray();
            }

            var serializer = new XmlSerializer(typeof(T), additionalTypes);

            XmlReader reader = System.Xml.XmlReader.Create(new System.IO.StringReader(rawValue));

            return (T)serializer.Deserialize(reader);
        }

        public static object Desearilize<T>(this Stream stream, Type[] additionalTypes) where T : class , new()
        {
            if (additionalTypes == null)
            {
                additionalTypes = CleanKnownTypes().ToArray();
            }

            var serializer = new XmlSerializer(typeof(T), additionalTypes);

            XmlReader reader = System.Xml.XmlReader.Create(stream);

            return serializer.Deserialize(reader);
        }

        public static object DeserializeFromFile(this object obj, Type T, string fileName, Type[] knownTypes)
        {
            System.Xml.Serialization.XmlSerializer x = null;

            if (knownTypes == null)
            {
                x = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            }
            else
            {
                x = new System.Xml.Serialization.XmlSerializer(obj.GetType(), knownTypes);
            }

            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                XmlReader reader = null;

                try
                {
                    reader = new XmlTextReader(fs);

                    // Use the Deserialize method to restore the object's state.

                    object i = x.Deserialize(reader);

                    return i;
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();

                        reader = null;
                    }
                }
            }
        }

        public static T DeserializeFromFile<T>(this T obj, string fileName) where T : class , new()
        {
            return DeserializeFromFile(obj, obj.GetType(), fileName, null) as T;
        }

        public static T DeserializeString<T>(string rawValue) where T : class , new()
        {
            return Desearilize(typeof(T), rawValue) as T;
        }

        public static String DownloadString(this WebClient webClient, String address, Encoding encoding)
        {
            byte[] buffer = webClient.DownloadData(address);
 
            byte[] bom = encoding.GetPreamble();
 
            if ((0 == bom.Length) || (buffer.Length < bom.Length))
            {
                return encoding.GetString(buffer);
            }
 
            for (int i = 0; i < bom.Length; i++)
            {
                if (buffer[i] != bom[i])
                {
                    return encoding.GetString(buffer);
                }
            }
 
            return encoding.GetString(buffer, bom.Length, buffer.Length - bom.Length);
        }

        /// <summary> 
        /// Encrypt the given string using AES.  The string can be decrypted using  
        /// DecryptStringAES().  The sharedSecret parameters must match. 
        /// </summary> 
        /// <param name="plainText">The text to encrypt.</param> 
        /// <param name="sharedSecret">A password used to generate a key for encryption.</param> 
        public static string EncryptStringAES(this string plainText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentNullException("plainText");
            }

            if (string.IsNullOrEmpty(sharedSecret))
            {
                throw new ArgumentNullException("sharedSecret");
            }

            string outStr = null;                       // Encrypted string to return 

            RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data. 

            try
            {
                // generate the key from the shared secret and the salt 
                var key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Create a RijndaelManaged object 

                aesAlg = new RijndaelManaged();

                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Create a decrytor to perform the stream transform. 

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption. 

                using (var msEncrypt = new MemoryStream())
                {
                    // prepend the IV 
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));

                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream. 
                            swEncrypt.Write(plainText);
                        }
                    }

                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Clear the RijndaelManaged object. 
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }

            // Return the encrypted bytes from the memory stream. 

            return outStr;
        }

        public static byte[] ReadFully(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];

            using (var ms = new MemoryStream())
            {
                int read;

                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Serialize object using XmlSerializer
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <returns>String representation of passed object</returns>
        public static string Serialize(this object obj)
        {
            var x = new System.Xml.Serialization.XmlSerializer(obj.GetType(), CleanKnownTypes().ToArray());

            string output = string.Empty;

            using (var memoryStream = new MemoryStream())
            {
                x.Serialize(memoryStream, obj);

                byte[] chars = memoryStream.ToArray();

                output = Encoding.UTF8.GetString(chars);
            }

            return output;
        }

        public static string Serialize(this object obj, Type[] knownTypes)
        {
            System.Xml.Serialization.XmlSerializer x = null;

            if (knownTypes == null)
            {
                x = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            }
            else
            {
                var arrs = new List<Type>(knownTypes);

                //if (obj is XSolonDictionary)
                {
                    //var dic = obj as XSolonDictionary;
                    ///TODO: TypeInsts
                    //dic.ForEach(k =>
                    //{
                    //    if (k.Value is List<TypeInsts>){
                    //        var list = k.Value as List<TypeInsts>;
                    //        list.ForEach(i => arrs.Add(i.Type));
                    //    }
                    //});
                }

                x = new System.Xml.Serialization.XmlSerializer(obj.GetType(), arrs.ToArray());
            }

            string output = string.Empty;

            using (var memoryStream = new MemoryStream())
            {
                x.Serialize(memoryStream, obj);

                byte[] chars = memoryStream.ToArray();

                output = Encoding.UTF8.GetString(chars);
            }

            return output;
        }

        /// <summary>
        /// Transforms object to json notation
        /// </summary>
        /// <param name="obj">object to serialize with JSON</param>
        /// <returns>JSON string </returns>
        /// <exception cref="System.Exception">Will Fail if object is not JSON serializable</exception>
        public static string ToJson(this object obj)
        {
            var oSerializer = new JavaScriptSerializer();

            oSerializer.MaxJsonLength = int.MaxValue;

            string sJSON = oSerializer.Serialize(obj);

            return sJSON;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];

            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];

            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }

        private static string SerializeObject(Object rawVal, Type propertyType)
        {
            string value = string.Empty;

            if (rawVal == null)
            {
                value = "NULL";
            }
            else if (propertyType.IsGenericType && propertyType.Name.TrimEnd("`1".ToCharArray()).Equals("EnumProp"))
            {
                Type[] types = propertyType.GetGenericArguments();

                Type enumType = types[0];

                PropertyInfo prop = propertyType.GetProperty("Value");

                rawVal = prop.GetValue(rawVal, null);

                value = (rawVal == null) ? "NULL" : rawVal.ToString();
            }
            else
            {
                value = Serialize(rawVal);

                var doc = new XmlDocument();

                doc.LoadXml(value);
                //value = RemoveAllNamespaces(doc.DocumentElement.OuterXml);
            }

            //else if (rawVal.GetType() == typeof(String))

            //{

            //    value = rawVal.ToString();

            //}

            //else if (rawVal is IDictionary)

            //{

            //    IDictionaryEnumerator enumerator = (rawVal as IDictionary).GetEnumerator();

            //    while (enumerator.MoveNext())

            //    {

            //        value += string.Format("\t{0}:\t\t{1}{2}", Convert.ToString(enumerator.Key), enumerator.Value.ToString(), Environment.NewLine);

            //    }

            //}

            //else if (rawVal is IEnumerable)

            //{

            //    foreach (object o in (rawVal as IEnumerable))

            //    {

            //        value += string.Format("\t{0}{1}", Convert.ToString(o), Environment.NewLine);

            //    }

            //}

            //else

            //    value = rawVal.ToString();

            return value;
        }
    }
}