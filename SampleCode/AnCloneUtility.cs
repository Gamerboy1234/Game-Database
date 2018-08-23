
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using ActiveNet.Core.Log;
using Newtonsoft.Json;

namespace ActiveNet.Types.Base
{
    public static class AnCloneUtility
    {
        #region Classes

        public class Schema
        {
            public string FieldName { get; set; }
            public string ClassName { get; set; }
            public string ClassFullName { get; set; }
            public Type Type { get; set; }
            public List<Schema> Columns { get; set; }

            public Schema()
            {
                FieldName = "";
                ClassName = "";
                ClassFullName = "";
                Type = null;
                Columns = new List<Schema>();
            }
        }

        #endregion Classes


        #region Private Fields

        private const string ExceptionError = "The type must be serializable.";

        #endregion Private Fields


        #region Binary Methods

        public static T BinaryClone<T>(T source)
        {
            try
            {
                if (ReferenceEquals(source, null))
                {
                    return default(T);
                }

                var type = AnTypes.GetType(source, null);

                if ((type == null) || !type.IsSerializable)
                {
                    throw new ArgumentException(ExceptionError, nameof(source));
                }

                IFormatter formatter = new BinaryFormatter();

                Stream stream = new MemoryStream();

                using (stream)
                {
                    formatter.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (T)formatter.Deserialize(stream);
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
                throw;
            }
        }

        #endregion Binary Methods


        #region Xml Methods

        public static T XmlClone<T>(T source, Type[] types)
        {
            try
            {
                if (ReferenceEquals(source, null))
                {
                    return default(T);
                }

                var type = AnTypes.GetType(source, null);

                if ((type == null) || !type.IsSerializable)
                {
                    throw new ArgumentException(ExceptionError, nameof(source));
                }

                var xmlSerializer = new XmlSerializer(type, types);

                Stream stream = new MemoryStream();

                using (stream)
                {
                    xmlSerializer.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);

                    // Uncomment these for debugging.
                    // var streamReader = new StreamReader(stream);
                    // var xmlContents = streamReader.ReadToEnd();
                    // stream.Seek(0, SeekOrigin.Begin);

                    return (T)xmlSerializer.Deserialize(stream);
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
                throw;
            }
        }

        public static string ToXml<T>(T source, Type[] types)
        {
            var xmlString = "";

            try
            {
                if (!ReferenceEquals(source, null))
                {
                    var type = AnTypes.GetType(source, null);

                    if ((type == null) || !type.IsSerializable)
                    {
                        throw new ArgumentException(ExceptionError, nameof(source));
                    }

                    var xmlSerializer = new XmlSerializer(type, types);

                    Stream stream = new MemoryStream();

                    using (stream)
                    {
                        xmlSerializer.Serialize(stream, source);
                        
                        stream.Seek(0, SeekOrigin.Begin);

                        var streamReader = new StreamReader(stream);
                        xmlString = streamReader.ReadToEnd();
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
                throw;
            }

            return xmlString;
        }

        public static T FromXml<T>(string xmlString, Type type, Type[] types)
        {
            try
            {
                if (!string.IsNullOrEmpty(xmlString) &&
                    (type != null))
                {
                    if (!type.IsSerializable)
                    {
                        throw new ArgumentException(ExceptionError, nameof(type));
                    }

                    var xmlSerializer = new XmlSerializer(type, types);

                    using (var stream = new MemoryStream())
                    {
                        var writer = new StreamWriter(stream);
                        writer.Write(xmlString);
                        writer.Flush();
                        stream.Seek(0, SeekOrigin.Begin);

                        return (T)xmlSerializer.Deserialize(stream);
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
                throw;
            }

            return default(T);
        }

        #endregion Xml Methods


        #region Json Methods

        public static T JsonClone<T>(T source, List<JsonConverter> jsonConverters = null)
        {
            try
            {
                if (ReferenceEquals(source, null))
                {
                    return default(T);
                }

                var type = AnTypes.GetType(source, null);

                if ((type == null) || !type.IsSerializable)
                {
                    throw new ArgumentException(ExceptionError, nameof(source));
                }

                var jsonString = new StringBuilder();

                var jsonSerializer = new JsonSerializer();

                if ((jsonConverters != null) && (jsonConverters.Count > 0))
                {
                    foreach (var jsonConverter in jsonConverters.Where(jsonConverter => jsonConverter != null))
                    {
                        jsonSerializer.Converters.Add(jsonConverter);
                    }
                }

                using (var stringWriter = new StringWriter(jsonString))
                {
                    jsonSerializer.Serialize(stringWriter, source);

                    using (var stringReader = new StringReader(jsonString.ToString()))
                    {
                        return (T)jsonSerializer.Deserialize(stringReader, type);
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
                throw;
            }
        }

        public static string ToJson<T>(T source, List<JsonConverter> jsonConverters = null)
        {
            var jsonString = new StringBuilder();

            try
            {
                if (!ReferenceEquals(source, null))
                {
                    var type = AnTypes.GetType(source, null);

                    if ((type == null) || !type.IsSerializable)
                    {
                        throw new ArgumentException(ExceptionError, nameof(source));
                    }

                    var jsonSerializer = new JsonSerializer();

                    if ((jsonConverters != null) && (jsonConverters.Count > 0))
                    {
                        foreach (var jsonConverter in jsonConverters.Where(jsonConverter => jsonConverter != null))
                        {
                            jsonSerializer.Converters.Add(jsonConverter);
                        }
                    }

                    using (var stringWriter = new StringWriter(jsonString))
                    {
                        jsonSerializer.Serialize(stringWriter, source);
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
                throw;
            }

            return jsonString.ToString();
        }

        public static string ToJson(Type type, object source, List<JsonConverter> jsonConverters = null)
        {
            var jsonString = new StringBuilder();

            try
            {
                if (!ReferenceEquals(source, null))
                {
                    if ((type == null) || !type.IsSerializable)
                    {
                        throw new ArgumentException(ExceptionError, nameof(source));
                    }

                    var jsonSerializer = new JsonSerializer();

                    if ((jsonConverters != null) && (jsonConverters.Count > 0))
                    {
                        foreach (var jsonConverter in jsonConverters.Where(jsonConverter => jsonConverter != null))
                        {
                            jsonSerializer.Converters.Add(jsonConverter);
                        }
                    }

                    using (var stringWriter = new StringWriter(jsonString))
                    {
                        jsonSerializer.Serialize(stringWriter, source);
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
                throw;
            }

            return jsonString.ToString();
        }

        public static T FromJson<T>(string jsonString, List<JsonConverter> jsonConverters = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(jsonString))
                {
                    var type = typeof (T);

                    if (!type.IsSerializable)
                    {
                        throw new Exception(ExceptionError);
                    }

                    var jsonSerializer = new JsonSerializer();

                    if ((jsonConverters != null) && (jsonConverters.Count > 0))
                    {
                        foreach (var jsonConverter in jsonConverters.Where(jsonConverter => jsonConverter != null))
                        {
                            jsonSerializer.Converters.Add(jsonConverter);
                        }
                    }

                    using (var stringReader = new StringReader(jsonString))
                    {
                        return (T)jsonSerializer.Deserialize(stringReader, type);
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
                throw;
            }

            return default(T);
        }

        public static object FromJson(Type type, string jsonString, List<JsonConverter> jsonConverters = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(jsonString))
                {
                    if (!type.IsSerializable)
                    {
                        throw new Exception(ExceptionError);
                    }

                    var jsonSerializer = new JsonSerializer();

                    if ((jsonConverters != null) && (jsonConverters.Count > 0))
                    {
                        foreach (var jsonConverter in jsonConverters.Where(jsonConverter => jsonConverter != null))
                        {
                            jsonSerializer.Converters.Add(jsonConverter);
                        }
                    }

                    using (var stringReader = new StringReader(jsonString))
                    {
                        return jsonSerializer.Deserialize(stringReader, type);
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
                throw;
            }

            return null;
        }

        #endregion Json Methods


        #region Dictionary Methods

        public static Dictionary<string, object> ToDictionary<T>(T t)
        {
            var result = new Dictionary<string, object>();

            try
            {
                if (t != null)
                {
                    var classType = AnTypes.GetType(t, null);

                    if (classType != null)
                    {
                        var properties = AnReflectionCache.GetPropertyInfoArray(classType);

                        if (properties?.Length > 0)
                        {
                            foreach (var property in properties)
                            {
                                var propertyType = property.PropertyType;

                                if (property.PropertyType.IsEnum)
                                {
                                    propertyType = AnReflectionCache.GetUnderlyingType(propertyType);
                                }

                                if (!string.IsNullOrEmpty(propertyType.FullName))
                                {
                                    object value = null;

                                    if (AnSerialization.SerialSizeDictionary.ContainsKey(propertyType) || 
                                        (propertyType == typeof(string)) ||
                                        (propertyType == typeof(byte[])))
                                    {
                                        value = property.GetValue(t, null);
                                    }

                                    else
                                    {
                                        if (propertyType.IsGenericType &&
                                            (propertyType.GetGenericTypeDefinition() == typeof(List<>)))
                                        {
                                            var toDictionaryList = AnReflectionCache.GetToDictionaryListMethod(typeof(AnCloneUtility));

                                            if (toDictionaryList != null)
                                            {
                                                var genericListItemType = GenericOfClassType(propertyType.FullName);

                                                if (genericListItemType != null)
                                                {
                                                    toDictionaryList = toDictionaryList.MakeGenericMethod(genericListItemType);

                                                    value = toDictionaryList.Invoke(null, new[] { property.GetValue(t, null) });
                                                }
                                            }
                                        }

                                        else if ((propertyType.BaseType != null) &&
                                            propertyType.BaseType.IsGenericType &&
                                            (propertyType.BaseType.GetGenericTypeDefinition() == typeof(List<>)))
                                        {
                                            var toDictionaryList = AnReflectionCache.GetToDictionaryListMethod(typeof(AnCloneUtility));

                                            if (toDictionaryList != null)
                                            {
                                                var genericListItemType = GenericOfClassType(propertyType.BaseType.FullName);

                                                if (genericListItemType != null)
                                                {
                                                    toDictionaryList = toDictionaryList.MakeGenericMethod(genericListItemType);

                                                    value = toDictionaryList.Invoke(null, new[] { property.GetValue(t, null) });
                                                }
                                            }
                                        }

                                        else
                                        {
                                            var toDictionary = AnReflectionCache.GetToDictionaryMethod(typeof(AnCloneUtility));

                                            if (toDictionary != null)
                                            {
                                                var genericListItemType = GenericOfClassType(propertyType.FullName);

                                                if (genericListItemType != null)
                                                {
                                                    toDictionary = toDictionary.MakeGenericMethod(genericListItemType);

                                                    value = toDictionary.Invoke(null, new[] { property.GetValue(t, null) });
                                                }
                                            }
                                        }
                                    }

                                    result.Add(property.Name.ToLower(), value);
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        public static T FromDictionary<T>(IDictionary<string, object> dictionary) where T : new()
        {
            var result = new T();

            try
            {
                if ((dictionary != null) &&
                    (dictionary.Count > 0))
                {
                    dictionary = AnTypes.LowerCaseKeys(dictionary);

                    var classType = AnTypes.GetType(result, null);

                    if (classType != null)
                    {
                        var properties = AnReflectionCache.GetPropertyInfoArray(classType);

                        if (properties?.Length > 0)
                        {
                            foreach (var property in properties.Where(property => dictionary.ContainsKey(property.Name.ToLower())))
                            {
                                var propertyType = property.PropertyType;

                                if (property.PropertyType.IsEnum)
                                {
                                    propertyType = AnReflectionCache.GetUnderlyingType(propertyType);
                                }

                                var key = property.Name.ToLower();

                                object value = null;

                                if (AnSerialization.SerialSizeDictionary.ContainsKey(propertyType))
                                {
                                    value = AnSafeConvert.ChangeType(dictionary[key], propertyType);
                                }

                                else if (propertyType == typeof(string))
                                {
                                    value = AnSafeConvert.ToString(dictionary[key]);
                                }

                                else if (propertyType == typeof(byte[]))
                                {
                                    value = dictionary[key] as byte[];
                                }

                                else
                                {
                                    if (dictionary[key] is Dictionary<string, object> subDictionary)
                                    {
                                        var fromDictionary = AnReflectionCache.GetFromDictionaryMethod(typeof(AnCloneUtility));

                                        if (fromDictionary != null)
                                        {
                                            var genericListItemType = GenericOfClassType(propertyType.FullName);

                                            if (genericListItemType != null)
                                            {
                                                fromDictionary = fromDictionary.MakeGenericMethod(genericListItemType);

                                                value = fromDictionary.Invoke(null, new object[] {subDictionary});
                                            }
                                        }
                                    }

                                    else
                                    {
                                        if ((dictionary[key] is List<Dictionary<string, object>> dictionaryList) &&
                                            (dictionaryList.Count > 0))
                                        {
                                            if (propertyType.IsGenericType &&
                                                (propertyType.GetGenericTypeDefinition() == typeof(List<>)))
                                            {
                                                var genericListItemType = GenericOfClassType(propertyType.FullName);

                                                if (genericListItemType != null)
                                                {
                                                    var fromDictionaryList = AnReflectionCache.GetFromDictionaryListMethod(typeof(AnCloneUtility));

                                                    if (fromDictionaryList != null)
                                                    {
                                                        fromDictionaryList = fromDictionaryList.MakeGenericMethod(genericListItemType);

                                                        value = fromDictionaryList.Invoke(null, new object[] {dictionaryList});
                                                    }
                                                }
                                            }

                                            else if ((propertyType.BaseType != null) &&
                                                        propertyType.BaseType.IsGenericType &&
                                                        (propertyType.BaseType.GetGenericTypeDefinition() == typeof (List<>)))
                                            {
                                                var genericListItemType = GenericOfClassType(propertyType.BaseType.FullName);

                                                if (genericListItemType != null)
                                                {
                                                    var fromDictionaryList = AnReflectionCache.GetFromDictionaryListMethod(typeof(AnCloneUtility));

                                                    if (fromDictionaryList != null)
                                                    {
                                                        fromDictionaryList = fromDictionaryList.MakeGenericMethod(genericListItemType);

                                                        var listObject = fromDictionaryList.Invoke(null, new object[] {dictionaryList});

                                                        if (listObject != null)
                                                        {
                                                            value = AnTypes.CreateItemInstance(propertyType.FullName, propertyType, new[] { listObject });
                                                        }
                                                    }
                                                }
                                            }

                                            else
                                            {
                                                var fromDictionary = AnReflectionCache.GetFromDictionaryMethod(typeof(AnCloneUtility));

                                                if (fromDictionary != null)
                                                {
                                                    var genericListItemType = GenericOfClassType(propertyType.FullName);

                                                    if (genericListItemType != null)
                                                    {
                                                        fromDictionary = fromDictionary.MakeGenericMethod(genericListItemType);

                                                        value = fromDictionary.Invoke(null, new object[] {dictionaryList[0]});
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                property.SetValue(result, value);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        public static List<Dictionary<string, object>> ToDictionaryList<T>(List<T> list)
        {
            var result = new List<Dictionary<string, object>>();

            try
            {
                if (list != null)
                {
                    result = (from t in list where t != null select ToDictionary(t) into dictionary where (dictionary != null) && (dictionary.Count > 0) select dictionary).ToList();
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        public static List<T> FromDictionaryList<T>(List<Dictionary<string, object>> dictionaryList) where T : new()
        {
            var result = new List<T>();

            try
            {
                if ((dictionaryList != null) &&
                    (dictionaryList.Count > 0))
                {
                    result.AddRange(dictionaryList.Where(dictionary => ((dictionary != null) && (dictionary.Count > 0))).Select(FromDictionary<T>));
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        #endregion Dictionary Methods


        #region Structure Methods

        public static string GenerateXmlTypeSchemaFileText(Type baseClassType)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(baseClassType?.FullName))
                {
                    var classes = new Dictionary<string, Type> {{baseClassType?.FullName ?? "", baseClassType}};

                    GetClasses(baseClassType, classes);

                    var baseElement = new XElement("Tables");

                    foreach (var className in classes.Keys.Where(className => !string.IsNullOrEmpty(className) && (classes[className] != null)))
                    {
                        baseElement.Add(new XElement("Table", new XAttribute("Type", TrimClassName(className)), GenerateClassElement(classes[className])));
                    }

                    var xDocument = new XDocument(
                        new XDeclaration("1.0", "utf-8", null),
                        new XComment("ActiveNet Inc. Type Definitions"),
                        baseElement);

                    result.Append(xDocument);
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateNonActiveNetXmlTypeSchemaFileText(Type baseClassType)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(baseClassType?.FullName))
                {
                    var parentClass = new Schema
                    {
                        FieldName = "Root",
                        ClassName = baseClassType?.Name ?? "",
                        ClassFullName = baseClassType?.FullName ?? "",
                        Type = baseClassType
                    };

                    var classes = new List<Schema> {parentClass};

                    GetNonActiveNetClasses(classes, parentClass);

                    var baseElement = new XElement("Tables");

                    foreach (var schema in classes.Where(schema => !string.IsNullOrEmpty(schema?.ClassFullName)))
                    {
                        baseElement.Add(new XElement("Table", new XAttribute("Type", TrimClassName(schema.ClassFullName)), GenerateSchemaElement(schema)));
                    }

                    var xDocument = new XDocument(
                        new XDeclaration("1.0", "utf-8", null),
                        new XComment("ActiveNet Inc. Type Definitions"),
                        baseElement);

                    result.Append(xDocument);
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        #endregion Structure Methods


        #region Names and Types

        public static Dictionary<string, Type> GetFieldNamesAndDataTypes<T>(T t)
        {
            var result = new Dictionary<string, Type>();

            try
            {
                if (t != null)
                {
                    var classType = AnTypes.GetType(t, null);

                    if (classType != null)
                    {
                        var properties = AnReflectionCache.GetPropertyInfoArray(classType);

                        if (properties?.Length > 0)
                        {
                            foreach (var property in properties)
                            {
                                var propertyType = property.PropertyType;

                                if (property.PropertyType.IsEnum)
                                {
                                    propertyType = AnReflectionCache.GetUnderlyingType(propertyType);
                                }

                                Type type;

                                if (AnSerialization.SerialSizeDictionary.ContainsKey(propertyType) ||
                                    (propertyType == typeof (string)) ||
                                    (propertyType == typeof (byte[])))
                                {
                                    type = propertyType;
                                }

                                else
                                {
                                    if (propertyType.IsGenericType &&
                                        (propertyType.GetGenericTypeDefinition() == typeof(List<>)))
                                    {
                                        type = GenericOfClassType(propertyType.FullName);
                                    }

                                    else if ((propertyType.BaseType != null) &&
                                        propertyType.BaseType.IsGenericType &&
                                        (propertyType.BaseType.GetGenericTypeDefinition() == typeof(List<>)))
                                    {
                                        type = GenericOfClassType(propertyType.BaseType.FullName);
                                    }

                                    else
                                    {
                                        type = GenericOfClassType(propertyType.FullName);
                                    }
                                }

                                if (type != null)
                                {
                                    result.Add(property.Name, type);
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        #endregion Names and Types


        #region Private Methods

        private static void GetClasses(Type parentClassType, IDictionary<string, Type> classes)
        {
            try
            {
                if ((parentClassType != null) &&
                    (classes != null))
                {
                    var properties = AnReflectionCache.GetPropertyInfoList(parentClassType);

                    if (properties?.Count > 0)
                    {
                        foreach (var property in properties.Where(prop => (prop?.Property != null) && (prop.Attribute != null)))
                        {
                            var type = property.Property.PropertyType;

                            if (property.Property.PropertyType.IsEnum)
                            {
                                type = AnReflectionCache.GetUnderlyingType(type);
                            }

                            if (!(AnSerialization.SerialSizeDictionary.ContainsKey(type) ||
                                (type == typeof (string)) ||
                                (type == typeof (byte[]))))
                            {
                                if (!string.IsNullOrEmpty(type.BaseType?.Name) &&
                                    (type.BaseType?.Name ?? "").ToLower().Contains("anbaseobjectlist"))
                                {
                                    if (!string.IsNullOrEmpty(type.BaseType?.FullName))
                                    {
                                        var subClassName = GenericOfClassName(type.BaseType?.FullName);

                                        if (!string.IsNullOrEmpty(subClassName))
                                        {
                                            if (!classes.ContainsKey(subClassName))
                                            {
                                                var subType = AnTypes.GetType(subClassName, null);

                                                if (subType != null)
                                                {
                                                    classes.Add(subClassName, subType);

                                                    GetClasses(subType, classes);
                                                }
                                            }
                                        }
                                    }
                                }

                                else
                                {
                                    if (!string.IsNullOrEmpty(type.FullName) && 
                                        !classes.ContainsKey(type.FullName))
                                    {
                                        classes.Add(type.FullName, type);

                                        GetClasses(type, classes);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }
        }

        private static void GetNonActiveNetClasses(ICollection<Schema> classes, Schema parentClass)
        {
            try
            {
                if ((classes != null) && 
                    (parentClass?.Type != null) &&
                    (parentClass.Columns != null) &&
                    (parentClass.Columns.Count == 0))
                {
                    var propertyArray = AnReflectionCache.GetPropertyInfoArray(parentClass.Type);

                    if (propertyArray?.Length > 0)
                    {
                        var properties = propertyArray.ToList();

                        if (properties.Count > 0)
                        {
                            foreach (var property in properties.Where(property => (property != null)))
                            {
                                var type = property.PropertyType;

                                if (property.PropertyType.IsEnum)
                                {
                                    type = AnReflectionCache.GetUnderlyingType(type);
                                }

                                if (AnSerialization.SerialSizeDictionary.ContainsKey(type) ||
                                    (type == typeof (string)) ||
                                    (type == typeof (byte[])))
                                {
                                    parentClass.Columns.Add(new Schema
                                    {
                                        FieldName = property.Name,
                                        ClassName = type.Name,
                                        ClassFullName = type.FullName,
                                        Type = type
                                    });
                                }

                                else
                                {
                                    if (type.IsGenericType &&
                                        (type.GetGenericTypeDefinition() == typeof(List<>)))
                                    {
                                        var genericListItemType = GenericOfClassType(type.FullName);

                                        if (!string.IsNullOrEmpty(genericListItemType?.FullName))
                                        {
                                            parentClass.Columns.Add(new Schema
                                            {
                                                FieldName = property.Name,
                                                ClassName = type.Name,
                                                ClassFullName = type.FullName,
                                                Type = type
                                            });

                                            if (!classes.Where(schema => schema != null).Any(schema => (schema.Type == genericListItemType) && string.Equals(schema.ClassFullName, genericListItemType?.FullName ?? "", StringComparison.CurrentCulture)))
                                            {
                                                var parentSchema = new Schema
                                                {
                                                    FieldName = property.Name,
                                                    ClassName = genericListItemType?.Name ?? "",
                                                    ClassFullName = genericListItemType?.FullName ?? "",
                                                    Type = genericListItemType
                                                };

                                                classes.Add(parentSchema);

                                                GetNonActiveNetClasses(classes, parentSchema);
                                            }
                                        }
                                    }

                                    else if ((type.BaseType != null) &&
                                                type.BaseType.IsGenericType &&
                                                (type.BaseType.GetGenericTypeDefinition() == typeof (List<>)))
                                    {
                                        var genericListItemType = GenericOfClassType(type.BaseType.FullName);

                                        if (!string.IsNullOrEmpty(genericListItemType?.FullName))
                                        {
                                            parentClass.Columns.Add(new Schema
                                            {
                                                FieldName = property.Name,
                                                ClassName = type.Name,
                                                ClassFullName = type.FullName,
                                                Type = type
                                            });

                                            if (!classes.Where(schema => schema != null).Any(schema => (schema.Type == genericListItemType) && string.Equals(schema.ClassFullName, genericListItemType?.FullName ?? "", StringComparison.CurrentCulture)))
                                            {
                                                var parentSchema = new Schema
                                                {
                                                    FieldName = property.Name,
                                                    ClassName = genericListItemType?.Name ?? "",
                                                    ClassFullName = genericListItemType?.FullName ?? "",
                                                    Type = genericListItemType
                                                };

                                                classes.Add(parentSchema);

                                                GetNonActiveNetClasses(classes, parentSchema);
                                            }
                                        }
                                    }

                                    else if (!classes.Where(schema => schema != null).Any(schema => (schema.Type == type) && (schema.ClassFullName == type.FullName)))
                                    {
                                        parentClass.Columns.Add(new Schema
                                        {
                                            FieldName = property.Name,
                                            ClassName = type.Name,
                                            ClassFullName = type.FullName,
                                            Type = type
                                        });

                                        var parentSchema = new Schema
                                        {
                                            FieldName = property.Name,
                                            ClassName = type.Name,
                                            ClassFullName = type.FullName,
                                            Type = type
                                        };

                                        classes.Add(parentSchema);

                                        GetNonActiveNetClasses(classes, parentSchema);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }
        }

        private static XElement GenerateClassElement(Type parentClassType)
        {
            var result = new XElement("Fields");

            try
            {
                if (parentClassType != null)
                {
                    var properties = AnReflectionCache.GetPropertyInfoList(parentClassType);

                    if (properties?.Count > 0)
                    {
                        foreach (var property in properties.Where(prop => (prop?.Property != null) && (prop.Attribute != null)))
                        {
                            var type = property.Property.PropertyType;

                            if (property.Property.PropertyType.IsEnum)
                            {
                                type = AnReflectionCache.GetUnderlyingType(type);
                            }

                            var className = type.FullName;

                            if (!string.IsNullOrEmpty(className))
                            {
                                result.Add(new XElement(property.Property.Name, new XAttribute("Type", TrimClassName(className))));
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        private static XElement GenerateSchemaElement(Schema parentSchema)
        {
            var result = new XElement("Fields");

            try
            {
                if ((parentSchema?.Columns != null) &&
                    (parentSchema.Columns.Count > 0))
                {
                    foreach (var schema in parentSchema.Columns.Where(schema => (schema?.Type != null) && !string.IsNullOrEmpty(schema.FieldName) && !string.IsNullOrEmpty(schema.ClassFullName)))
                    {
                        result.Add(new XElement(schema.FieldName, new XAttribute("Type", TrimClassName(schema.ClassFullName))));
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        private static string TrimClassName(string fullName)
        {
            var result = fullName ?? "";

            try
            {
                if (!string.IsNullOrEmpty(result))
                {
                    var parts = result.Split('.');

                    if (parts.Length > 0)
                    {
                        result = parts[parts.Length - 1].Trim();
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        private static string GenericOfClassName(string listFullClassName)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(listFullClassName))
                {
                    var startPos = listFullClassName.IndexOf("[[", 0, StringComparison.InvariantCulture);

                    if (startPos > -1)
                    {
                        var itemClass = listFullClassName.Substring(startPos + 2);

                        if (!string.IsNullOrEmpty(itemClass))
                        {
                            var classPieces = itemClass.Split(',');

                            var subClassName = ((classPieces.Length > 0) ? classPieces[0] : itemClass).Trim().Trim(',').Trim();

                            if (!string.IsNullOrEmpty(subClassName))
                            {
                                result.Append(subClassName);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static Type GenericOfClassType(string fullClassName)
        {
            Type result = null;

            try
            {
                var subClassName = GenericOfClassName(fullClassName);

                result = AnTypes.GetType(!string.IsNullOrEmpty(subClassName) ? subClassName : fullClassName, null);
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        #endregion Private Methods
    }
}
