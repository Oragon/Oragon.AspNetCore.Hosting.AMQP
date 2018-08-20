using System.IO;
using System.Text;

namespace Oragon.AspNetCore.Hosting.AMQP
{
    public static class Helper
    {
        public static byte[] ReadToEnd(this Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static string UTF8GetString(this object objectToConvert) => objectToConvert is string ? (string)objectToConvert : objectToConvert == null ? null : Encoding.UTF8.GetString((byte[])objectToConvert);

        public static long UTF8GetLong(this object objectToConvert) => objectToConvert is long ? (long)objectToConvert : objectToConvert == null ? 0 : long.Parse(objectToConvert.UTF8GetString());
    }
}