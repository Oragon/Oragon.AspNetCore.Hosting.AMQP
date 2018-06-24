using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Oragon.AspNetCore.Hosting.AMQP
{
    public static class Helper
    {
        private static IFormatter formatter = new BinaryFormatter();

        public static byte[] Serialize(object objectToSerialize)
        {
            using (Stream stream = new MemoryStream())
            {
                formatter.Serialize(stream, objectToSerialize);
                stream.Position = 0;
                return stream.ReadToEnd();
            }
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            using (Stream stream = new MemoryStream(bytes))
            {
                return (T)formatter.Deserialize(stream);
            }
        }

        public static byte[] ReadToEnd(this Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }


        public static string UTF8GetString(this object objectToConvert)
        {
            return objectToConvert is string ? (string)objectToConvert : objectToConvert == null ? null : Encoding.UTF8.GetString((byte[])objectToConvert);
        }

        public static int UTF8GetInt(this object objectToConvert)
        {
            return objectToConvert is int ? (int)objectToConvert : objectToConvert == null ? 0 : int.Parse(objectToConvert.UTF8GetString());
        }

        public static long UTF8GetLong(this object objectToConvert)
        {
            return objectToConvert is long ? (long)objectToConvert : objectToConvert == null ? 0 : long.Parse(objectToConvert.UTF8GetString());
        }
    }
}
