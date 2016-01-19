using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Torrent.Helpers.Utils
{
    using Extensions;
    using static DebugUtils;

    public static class SerializationUtils
	{
        static readonly Dictionary<Type, XmlSerializer> serializers = new Dictionary<Type, XmlSerializer>();

        static XmlSerializer GetSerializer<T>()
            => GetSerializer(typeof (T));
        static XmlSerializer GetSerializer(Type t) 
            => serializers.ContainsKey(t)
                       ? serializers[t]
                       : (serializers[t] = XmlSerializer.FromTypes(new[] { t })[0]);

        /// <summary>
        /// Writes the given object instance to an XML file.
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToXmlFile(string filePath, object objectToWrite, bool append = false) 
		{
            filePath = PathUtils.ConvertFromUri(filePath);
            var t = objectToWrite.GetType();
            TextWriter writer = null;
            try {
                var serializer = GetSerializer(t);
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
            } catch (Exception ex) {
                DEBUG.ReportError(ex, $"Error Serializing {t.GetFriendlyFullName()} To XML File at {filePath}");
            }
            finally {
			    writer?.Close();
			}
		}

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>        
        public static T ReadFromXmlFile<T>(string filePath) where T : new()
		{
            filePath = PathUtils.ConvertFromUri(filePath);
            if (!new FileInfo(filePath).Exists) {
				return new T();
			}
			TextReader reader = null;
            try {
                var serializer = GetSerializer<T>();
                reader = new StreamReader(filePath);
                return (T) serializer.Deserialize(reader);
            } catch (Exception ex) {
                DEBUG.ReportError(ex, $"Error Deserializing {typeof(T).GetFriendlyFullName()} From XML File at {filePath}");
            } finally {
			    reader?.Close();
			}
            return new T(); 
		}
	}
}