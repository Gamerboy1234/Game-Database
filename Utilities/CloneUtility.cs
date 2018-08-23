
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Logger;
using Newtonsoft.Json;

namespace Utilities
{
    public static class CloneUtility
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

                var type = typeof(T);

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
                Log.Error(ex);
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

                var type = typeof(T);

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
                Log.Error(ex);
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
                    var type = typeof(T);

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
                Log.Error(ex);
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
                Log.Error(ex);
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

                var type = typeof(T);

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
                Log.Error(ex);
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
                    var type = typeof(T);

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
                Log.Error(ex);
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
                Log.Error(ex);
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
                Log.Error(ex);
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
                Log.Error(ex);
                throw;
            }

            return null;
        }

        #endregion Json Methods
    }
}
