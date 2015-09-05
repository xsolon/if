// ----------------------
//  xSolon.Instructions.DTO - ReflectionExtensions.cs 
//  administrator - 4/25/2013 
// ----------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace xSolon.Events
{
    /// <summary>
    /// Reflection related Extension Methods
    /// </summary>
    public static partial class SerializationExtensions
    {
        public static FieldInfo[] GetFullFields(this Type type, BindingFlags flags)
        {
            if (type.IsInterface)
            {
                var FieldInfos = new List<FieldInfo>();

                var considered = new List<Type>();

                var queue = new Queue<Type>();

                considered.Add(type);

                queue.Enqueue(type);

                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();

                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface))
                        {
                            continue;
                        }

                        considered.Add(subInterface);

                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetFields(flags);

                    var newFieldInfos = typeProperties.Where(x => !FieldInfos.Contains(x));

                    FieldInfos.InsertRange(0, newFieldInfos);
                }

                return FieldInfos.ToArray();
            }

            var temp = new List<FieldInfo>();

            if (type.BaseType != null)
            {
                var ff = GetFullFields(type.BaseType, flags);

                temp = new List<FieldInfo>(ff);
            }

            temp.AddRange(type.GetFields(flags));

            return temp.ToArray();
        }

        /// <summary>
        /// Get all Publice Properties for the type (incuding interfaces)
        /// </summary>
        /// <param name="type">Type to inspect</param>
        /// <returns>Array of PropertyInfo for public properties of the type</returns>
        public static PropertyInfo[] GetFullProperties(this Type type, BindingFlags flags)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();

                var queue = new Queue<Type>();

                considered.Add(type);

                queue.Enqueue(type);

                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();

                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface))
                        {
                            continue;
                        }

                        considered.Add(subInterface);

                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(flags);

                    var newPropertyInfos = typeProperties
                                                         .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            var temp = new List<PropertyInfo>();

            if (type.BaseType != null)
            {
                temp = new List<PropertyInfo>(GetFullProperties(type.BaseType, flags));
            }

            temp.AddRange(type.GetProperties(flags));

            return temp.ToArray();
        }

        /// <summary>
        /// Get all Publice Properties for the type (incuding interfaces)
        /// </summary>
        /// <param name="type">Type to inspect</param>
        /// <returns>Array of PropertyInfo for public properties of the type</returns>
        public static PropertyInfo[] GetPublicProperties(this Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();

                var queue = new Queue<Type>();

                considered.Add(type);

                queue.Enqueue(type);

                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();

                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface))
                        {
                            continue;
                        }

                        considered.Add(subInterface);

                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(
                                                               BindingFlags.FlattenHierarchy |
                                                               BindingFlags.Public |
                                                               BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                                                         .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy |
                                      BindingFlags.Public | BindingFlags.Instance);
        }

        public static Dictionary<string, string> GetValuesDictionary(this object thiz, List<string> propertyNames)
        {
            var res = new Dictionary<string, string>();

            Type inType = thiz.GetType();

            string dd = string.Empty;

            var propertyInfos = inType.GetFullProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var info in propertyInfos)
            {
                if (!res.ContainsKey(info.Name) && propertyNames.Contains(info.Name))
                {
                    res.Add(info.Name, info.Xsol_GetValue(thiz));
                }
            }

            var fields = inType.GetFullFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var info in fields)
            {
                if (!res.ContainsKey(info.Name) && propertyNames.Contains(info.Name))
                {
                    res.Add(info.Name, info.Xsol_GetValue(thiz));
                }
            }

            return res;
        }

        public static List<Dictionary<string, string>> GetSerializedList(this IEnumerable thiz)
        {

            var result = new List<Dictionary<string, string>>();
            var enumer = thiz.GetEnumerator();

            while (enumer.MoveNext())
            {
                try
                {

                    result.Add(enumer.Current.GetValuesDictionary());

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            return result;

        }

        public static Dictionary<string, string> GetValuesDictionary(this object thiz)
        {
            var res = new Dictionary<string, string>();

            Type inType = thiz.GetType();

            string dd = string.Empty;

            var propertyInfos = inType.GetFullProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var info in propertyInfos)
            {
                if (!res.ContainsKey(info.Name))
                {
                    res.Add(info.Name, info.Xsol_GetValue(thiz));
                }
            }

            var fields = inType.GetFullFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var info in fields)
            {
                if (!res.ContainsKey(info.Name))
                {
                    res.Add(info.Name, info.Xsol_GetValue(thiz));
                }
            }

            return res;
        }

        public static Dictionary<string, string> GetXMLValuesDictionary(this object thiz, List<string> propertyNames)
        {
            var res = new Dictionary<string, string>();

            string template = "{0}: {1}\r\n";

            Type inType = thiz.GetType();

            string dd = string.Empty;

            var propertyInfos = inType.GetFullProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var info in propertyInfos)
            {
                if (!res.ContainsKey(info.Name) && propertyNames.Contains(info.Name))
                {
                    res.Add(info.Name, info.Xsol_GetValueAsXml(thiz));
                }
            }

            var fields = inType.GetFullFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var info in fields)
            {
                if (!res.ContainsKey(info.Name) && propertyNames.Contains(info.Name))
                {
                    res.Add(info.Name, info.Xsol_GetValueAsXML(thiz));
                }
            }

            return res;
        }

        public static Dictionary<string, string> GetXMLValuesDictionary(this object thiz)
        {
            var res = new Dictionary<string, string>();

            string template = "{0}: {1}\r\n";

            Type inType = thiz.GetType();

            string dd = string.Empty;

            var propertyInfos = inType.GetFullProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var info in propertyInfos)
            {
                if (!res.ContainsKey(info.Name))
                {
                    res.Add(info.Name, info.Xsol_GetValueAsXml(thiz));
                }
            }

            var fields = inType.GetFullFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var info in fields)
            {
                if (!res.ContainsKey(info.Name))
                {
                    res.Add(info.Name, info.Xsol_GetValueAsXML(thiz));
                }
            }

            return res;
        }

        public static string RawPrint(this object thiz)
        {
            return RawPrint(thiz, BindingFlags.Public | BindingFlags.Instance);
        }

        public static string RawPrint(this object thiz, BindingFlags flags)
        {
            if (thiz == null)
            {
                return "NULL";
            }

            if (thiz.GetType().IsSerializable)
            {
                return thiz.Serialize();
            }

            string template = "{0}: {1}\r\n";

            Type inType = thiz.GetType();

            string dd = string.Empty;

            var propertyInfos = inType.GetFullProperties(flags);

            foreach (var info in propertyInfos)
            {
                dd += string.Format(template, info.Name, info.Xsol_GetValueAsXml(thiz));
            }

            var fields = inType.GetFullFields(flags);

            foreach (var info in fields)
            {
                dd += string.Format(template, info.Name, info.Xsol_GetValueAsXML(thiz));
            }

            return dd;
        }

        //Core recursion function
        //Implemented based on interface, not part of algorithm

        /// <summary>
        /// Get value (as string) of a field from the passed object
        /// </summary>
        /// <param name="inProp">FieldInfo of the field</param>
        /// <param name="inObject">object that should contain the FieldInfo</param>
        /// <returns>Value as a string. If serialization fails the exception will be passed as result</returns>
        public static string Xsol_GetValue(this FieldInfo inProp, object inObject)
        {
            string value = string.Empty;

            try
            {
                Type propertyType = inProp.FieldType;

                var rawVal = inProp.GetValue(inObject);

                if (rawVal == null)
                {
                    value = "NULL";
                }
                else
                {
                    value = rawVal.ToString();
                }
            }
            catch (Exception ex)
            {
                value = ex.ToString();
            }

            return value;
        }

        /// <summary>
        /// Get value (as string) of a property from the passed object
        /// </summary>
        /// <param name="inProp">PropertyInfo of the field</param>
        /// <param name="inObject">object that should contain the Property</param>
        /// <returns>Value as a string. If serialization fails the exception will be passed as result</returns>
        public static string Xsol_GetValue(this PropertyInfo inProp, object inObject)
        {
            string value = string.Empty;

            try
            {
                Type propertyType = inProp.PropertyType;

                var rawVal = inProp.GetValue(inObject, null);

                if (rawVal == null)
                {
                    value = "NULL";
                }
                else
                {
                    value = rawVal.ToString();
                }
            }
            catch (Exception ex)
            {
                value = ex.ToString();
            }

            return value;
        }

        /// <summary>
        /// Get value (as string) of a property from the passed object
        /// </summary>
        /// <param name="inProp">PropertyInfo of the field</param>
        /// <param name="inObject">object that should contain the Property</param>
        /// <returns>Value as a string. If serialization fails the exception will be passed as result</returns>
        public static string Xsol_GetValueAsXml(this PropertyInfo inProp, object inObject)
        {
            string value = string.Empty;

            try
            {
                Type propertyType = inProp.PropertyType;

                var rawVal = inProp.GetValue(inObject, null);

                value = SerializeObject(rawVal, propertyType);
            }
            catch (Exception ex)
            {
                value = ex.ToString();
            }

            return value;
        }

        /// <summary>
        /// Get value (as string) of a field from the passed object
        /// </summary>
        /// <param name="inProp">FieldInfo of the field</param>
        /// <param name="inObject">object that should contain the FieldInfo</param>
        /// <returns>Value as a string. If serialization fails the exception will be passed as result</returns>
        public static string Xsol_GetValueAsXML(this FieldInfo inProp, object inObject)
        {
            string value = string.Empty;

            try
            {
                Type propertyType = inProp.FieldType;

                var rawVal = inProp.GetValue(inObject);

                value = SerializeObject(rawVal, propertyType);
            }
            catch (Exception ex)
            {
                value = ex.ToString();
            }

            return value;
        }
    }

    public static class EnumExt
    {
        /// <summary>
        /// Check to see if a flags enumeration has a specific flag set.
        /// </summary>
        /// <param name="variable">Flags enumeration to check</param>
        /// <param name="value">Flag to check for</param>
        /// <returns></returns>
        public static bool HasFlag(this Enum variable, Enum value)
        {
            if (variable == null)
            {
                return false;
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            // Not as good as the .NET 4 version of this function, but should be good enough

            if (!Enum.IsDefined(variable.GetType(), value))
            {
                throw new ArgumentException(string.Format(
                    "Enumeration type mismatch.  The flag is of type '{0}', was expecting '{1}'.",
                    value.GetType(), variable.GetType()));
            }

            ulong num = Convert.ToUInt64(value);

            return ((Convert.ToUInt64(variable) & num) == num);
        }
    }
}