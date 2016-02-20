using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Torrent.Serialization
{
    using Extensions;
    using Helpers.Utils;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;
    using static Helpers.Utils.DebugUtils;

    public static class DataContractUtils
    {
        static readonly Dictionary<Type, DataContractSerializer> serializers = new Dictionary<Type, DataContractSerializer>();
        static readonly NamespaceAttribute[] defaultNamespaces = new NamespaceAttribute[]
        {
            new NamespaceAttribute("x", "http://www.w3.org/2001/XMLSchema"),
            new NamespaceAttribute("a", "http://schemas.microsoft.com/2003/10/Serialization/Arrays"),
        };

        static DataContractSerializer GetSerializer<T>()
            => GetSerializer(typeof (T));

        static DataContractSerializer GetSerializer(Type t)
        {            
            if (!serializers.ContainsKey(t))
            {
                var dataContract = t.GetAttribute<DataContractAttribute>();
                var name = string.IsNullOrEmpty(dataContract?.Name) ? t.Name : dataContract.Name;
                serializers[t] = new DataContractSerializer(t, name, dataContract.Namespace);
            }
            return serializers[t];
        }

        public static string Serialize<T>(T obj)
        {
            var serializer = GetSerializer<T>();
            using (var writer = new StringWriter())
            using (var stm = new XmlTextWriter(writer))
            {
                serializer.WriteObject(stm, obj);
                return writer.ToString();
            }
        }
        public static T Deserialize<T>(string serialized)
        {
            var serializer = GetSerializer<T>();
            using (var reader = new StringReader(serialized))
            using (var stm = new XmlTextReader(reader))
            {
                return (T)serializer.ReadObject(stm);
            }
        }

        /// <summary>
        /// Writes the given object instance to an XML file using a DataContractSerializer.
        /// Utilizes the NamespaceAttribute to prefix all specified namespaces
        /// </summary>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        public static void WriteToFile(string filePath, object objectToWrite)
        {
            filePath = PathUtils.ConvertFromUri(filePath);
            var type = objectToWrite.GetType();
            using (var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                if (outputStream == null)
                    throw new ArgumentNullException("Must have valid output stream");

                if (outputStream.CanWrite == false)
                    throw new ArgumentException("Cannot write to output stream");

                var attributes = type.GetAttributes<NamespaceAttribute>();

                using (var writer = XmlWriter.Create(outputStream, new XmlWriterSettings
                {
                    Async = true,
                    Indent = true,
                    NewLineOnAttributes = true,
                    NamespaceHandling = NamespaceHandling.OmitDuplicates
                }))
                {
                    var serializer = GetSerializer(type);
                    serializer.WriteStartObject(writer, objectToWrite);
                    foreach (var ns in attributes.Union(defaultNamespaces))
                    {
                        writer.WriteAttributeString("xmlns", ns.Prefix, null, ns.Uri);
                    }
                    
                    serializer.WriteObjectContent(writer, objectToWrite);
                    serializer.WriteEndObject(writer);
                }
            }
        }
        /// <summary>
        /// Writes the given object instance to an XML file using a DataContractSerializer.
        /// </summary>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        public static void WriteToFileNormal(string filePath, object objectToWrite)
        {
            filePath = PathUtils.ConvertFromUri(filePath);
            var type = objectToWrite.GetType();
            using (var writer = XmlWriter.Create(filePath, new XmlWriterSettings {
                Async = true,
                Indent = true,
                NamespaceHandling = NamespaceHandling.OmitDuplicates
            }))
            {
                try
                {
                    var serializer = GetSerializer(type);
                    serializer.WriteObject(writer, objectToWrite);
                }
                catch (Exception ex)
                {
                    DEBUG.ReportError(ex, $"Error Serializing {type.GetFriendlyFullName()} To XML File at {filePath}");
                }
            }
        }

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>        
        public static T ReadFromFile<T>(string filePath) where T : new()
        {
            filePath = PathUtils.ConvertFromUri(filePath);
            var fi = FileUtils.GetInfo(filePath);
            if (!fi.Exists || fi.Length < 5) {
                return new T();
            }
            //TextReader reader = null;
            using (var reader = XmlReader.Create(filePath))
            {
                try
                {
                    var serializer = GetSerializer<T>();
                    var obj = serializer.ReadObject(reader);
                    var cast = (T) obj;
                    return cast;
                    // return (T) serializer.ReadObject(reader);
                }
                catch (Exception ex)
                {
                    DEBUG.ReportError(ex, $"Error Deserializing {typeof(T).GetFriendlyFullName()} From XML File at {filePath}");
                }
            }
            return new T();
        }
    }
}