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

        public static string UTF8GetString(this object objectToConvert)
        {
            if (objectToConvert is string)
                return (string)objectToConvert;

            if (objectToConvert != null)
                return Encoding.UTF8.GetString((byte[])objectToConvert);

            return null;
        }

        public static long UTF8GetLong(this object objectToConvert)
        {
            if (objectToConvert is long)
                return (long)objectToConvert;

            if (objectToConvert != null)
                return long.Parse(objectToConvert.UTF8GetString());

            return 0;
        }
    }
}