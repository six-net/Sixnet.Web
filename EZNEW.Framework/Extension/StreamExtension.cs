using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Extension
{
    public static class StreamExtension
    {
        /// <summary>
        /// serialization a Stream object to byte array
        /// </summary>
        /// <param name="value">stream object</param>
        /// <returns>byte array</returns>
        public static byte[] ToBytes(this Stream value)
        {
            if (value == null)
            {
                return new byte[0];
            }
            using (var mem = new MemoryStream())
            {
                value.CopyTo(mem);
                return mem.ToArray();
            }
        }
    }
}
