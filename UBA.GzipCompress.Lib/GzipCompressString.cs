using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UBA.GzipCompress.Lib
{
    public class GzipCompressString
    {
        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DecompressString(string value)
        {
            byte[] byteArray = Convert.FromBase64String(value);
            byte[] tmpArray;

            using (MemoryStream msOut = new MemoryStream())
            {
                using (MemoryStream msIn = new MemoryStream(byteArray))
                {
                    using (GZipStream swZip = new GZipStream(msIn, CompressionMode.Decompress))
                    {
                        swZip.CopyTo(msOut);
                        tmpArray = msOut.ToArray();
                    }
                }
            }
            return Encoding.UTF8.GetString(tmpArray);
        }
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CompressString(string value)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(value);
            byte[] tmpArray;

            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream sw = new GZipStream(ms, CompressionMode.Compress))
                {
                    sw.Write(byteArray, 0, byteArray.Length);
                    sw.Flush();
                }
                tmpArray = ms.ToArray();
            }
            return Convert.ToBase64String(tmpArray);
        }

    }
}
