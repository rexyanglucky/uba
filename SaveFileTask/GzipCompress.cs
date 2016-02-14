using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace UBA.Redis.SaveFileTask
{
    public class GzipCompress
    {
        //#region 压缩
        ///// <summary>
        ///// gzip压缩字符串
        ///// </summary>
        ///// <param name="input">字符串 UTF8格式</param>
        ///// <returns>Base64</returns>
        //public static string CompressString(string input)
        //{
        //    var finalBuffer = CompressStringToByte(input);
        //    if (finalBuffer.Length > 0)
        //    {
        //        return System.Convert.ToBase64String(finalBuffer);
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }
        //}
        ///// <summary>
        ///// gzip压缩byte[]
        ///// </summary>
        ///// <param name="input">UTF8编码</param>
        ///// <returns>Base64</returns>
        //public static string Compress(byte[] input)
        //{
        //    var finalBuffer = CompressToByte(input);
        //    if (finalBuffer.Length > 0)
        //    {
        //        return System.Convert.ToBase64String(finalBuffer);
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }
        //}

        ///// <summary>
        ///// gzip压缩字符串
        ///// </summary>
        ///// <param name="input">字符串 UTF8格式</param>
        ///// <returns>byte[]</returns>
        //public static byte[] CompressStringToByte(string input)
        //{
        //    string retValue = string.Empty;
        //    if (!string.IsNullOrEmpty(input))
        //    {
        //        byte[] byteSource = Encoding.UTF8.GetBytes(input);
        //        MemoryStream msm = new MemoryStream();
        //        using (GZipStream gzs = new GZipStream(msm, CompressionMode.Compress, true))
        //        {
        //            gzs.Write(byteSource, 0, byteSource.Length);
        //        }

        //        msm.Position = 0;

        //        byte[] compBytes = new byte[msm.Length];
        //        msm.Read(compBytes, 0, compBytes.Length);

        //        msm.Close();

        //        byte[] finalBuffer = new byte[compBytes.Length + 4];
        //        Buffer.BlockCopy(compBytes, 0, finalBuffer, 4, compBytes.Length);
        //        Buffer.BlockCopy(BitConverter.GetBytes(byteSource.Length), 0, finalBuffer, 0, 4);
        //        return finalBuffer;

        //    }

        //    return new byte[0];
        //}
        ///// <summary>
        ///// gzip压缩byte[]
        ///// </summary>
        ///// <param name="input">UTF8编码</param>
        ///// <returns>byte[]</returns>
        //public static byte[] CompressToByte(byte[] input)
        //{
        //    string retValue = string.Empty;
        //    if (input.Length > 0)
        //    {
        //        MemoryStream msm = new MemoryStream();
        //        using (GZipStream gzs = new GZipStream(msm, CompressionMode.Compress, true))
        //        {
        //            gzs.Write(input, 0, input.Length);
        //        }

        //        msm.Position = 0;

        //        byte[] compBytes = new byte[msm.Length];
        //        msm.Read(compBytes, 0, compBytes.Length);

        //        msm.Close();

        //        byte[] finalBuffer = new byte[compBytes.Length + 4];
        //        Buffer.BlockCopy(compBytes, 0, finalBuffer, 4, compBytes.Length);
        //        Buffer.BlockCopy(BitConverter.GetBytes(input.Length), 0, finalBuffer, 0, 4);
        //        return finalBuffer;

        //    }
        //    return new byte[0];
        //}
        //#endregion

        //#region 解压
        ///// <summary>
        ///// gzip解压
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public static string DecompressString(string input)
        //{
        //    var finalBuffer = DecompressStringToByte(input);
        //    if (finalBuffer.Length > 0)
        //    {
        //        return System.Text.Encoding.UTF8.GetString(finalBuffer);
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }

        //}
        //public static string Decompress(byte[] input)
        //{
        //    var finalBuffer = DecompressByteToByte(input);
        //    if (finalBuffer.Length > 0)
        //    {
        //        return System.Text.Encoding.UTF8.GetString(finalBuffer);
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }

        //}
        ///// <summary>
        ///// gzip解压为byte[]
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public static byte[] DecompressStringToByte(string input)
        //{
        //    string retValue = string.Empty;
        //    if (!string.IsNullOrEmpty(input))
        //    {
        //        byte[] source = System.Convert.FromBase64String(input);
        //        using (MemoryStream msm = new MemoryStream())
        //        {
        //            int length = BitConverter.ToInt32(source, 0);
        //            msm.Write(source, 4, source.Length - 4);
        //            msm.Position = 0;
        //            byte[] decmpBytes = new byte[length];
        //            using (GZipStream gzs = new GZipStream(msm, CompressionMode.Decompress))
        //            {
        //                gzs.Read(decmpBytes, 0, length);
        //            }
        //            return decmpBytes;
        //        }
        //    }
        //    return new byte[0];
        //}

        //public static byte[] DecompressByteToByte(byte[] source)
        //{
        //    string retValue = string.Empty;
        //    if (source.Length > 0)
        //    {
        //        using (MemoryStream msm = new MemoryStream())
        //        {
        //            int length = BitConverter.ToInt32(source, 0);
        //            msm.Write(source, 4, source.Length - 4);
        //            msm.Position = 0;
        //            byte[] decmpBytes = new byte[length];
        //            using (GZipStream gzs = new GZipStream(msm, CompressionMode.Decompress))
        //            {
        //                gzs.Read(decmpBytes, 0, length);

        //            }
        //            return decmpBytes;

        //        }
        //    }

        //    return new byte[0];
        //}
        //#endregion


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
