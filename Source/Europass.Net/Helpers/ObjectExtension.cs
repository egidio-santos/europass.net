using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Europass.Net.Helpers
{
    /// <summary>
    /// Extensions for the object Class
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// Perform a deep Copy of the object.
        /// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
        /// Provides a method for performing a deep copy of an object.
        /// Binary Serialization is used to perform the copy.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            ////Since binary serialization might require extra configuration,
            //// i'll have less worries if i just use Json here
            //using (Stream stream = new MemoryStream())
            //{
            //    IFormatter formatter = new BinaryFormatter();
            //    formatter.Serialize(stream, source);
            //    stream.Seek(0, SeekOrigin.Begin);
            //    return (T)formatter.Deserialize(stream);
            //}

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Newtonsoft.Json.JsonConvert.SerializeObject(source));
        }
    }
}
