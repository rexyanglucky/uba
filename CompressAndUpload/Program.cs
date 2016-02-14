using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UBA.Ftp.Lib;

namespace UBA.CompressAndUpload
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO
            try
            {
                Common.LogHelperNet.Info("压缩文件并上传：", null);
                string baseFolder = System.Configuration.ConfigurationManager.AppSettings["FileDirectory"];

                string destDir = System.Configuration.ConfigurationManager.AppSettings["DestDirectory"];

                string folderDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                string folder = Path.Combine(baseFolder, folderDate);
                string zipName = folderDate + ".zip";
                string destName = Path.Combine(destDir, zipName);

                if (CombineFilesByDirAndCompress(folder, baseFolder, zipName))
                {
                    Common.LogHelperNet.Info("压缩文件成功：" + zipName, null);
                    Console.WriteLine("Compress successful");
                    var start = DateTime.Now;
                    //上传失败，休息10秒，再次上传
                    while (true)
                    {
                        if (Upload(zipName, baseFolder, destName))
                        { break; }
                        else
                        {
                            System.Threading.Thread.Sleep(10000);
                        }
                    }

                    Common.LogHelperNet.Info("压缩上传文件结束，耗时" + DateTime.Now.Subtract(start).TotalMilliseconds + ":" + zipName, null);
                    Console.WriteLine(DateTime.Now.Subtract(start).TotalMilliseconds);

                }
                else
                {
                    Common.LogHelperNet.Info("压缩文件失败：文件不存在" + zipName, null);
                    Console.WriteLine("Compress failure");
                    return;
                }
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Common.LogHelperNet.Info("压缩上传文件失败", e);
                Console.WriteLine(e.Message);
                Console.WriteLine("上传失败");
                Console.WriteLine("failure");
            }


        }
        /// <summary>
        /// 将多文件合并，并压缩
        /// </summary>
        /// <param name="dirName"></param>
        /// <param name="destName"></param>
        private static bool CombineFilesByDirAndCompress(string dirName, string destDir, string destName)
        {
            try
            {
                if (!Directory.Exists(dirName))
                {
                    Console.WriteLine("请输入正确的文件夹");
                    return false;
                }
                var fileArray = System.IO.Directory.GetFiles(dirName);
                FileInfo[] fileInfos = new FileInfo[fileArray.Length];

                for (int k = 0; k < fileArray.Length; k++)
                {
                    fileInfos[k] = new FileInfo(fileArray[k]);
                }
                UBA.GzipCompress.Lib.GZipResult result = UBA.GzipCompress.Lib.GzipCompressFile.CompresToOneFile(fileInfos, dirName, destDir, destName, true);
                return !result.Errors;
            }
            catch
            {
                return false;
            }
        }

        private static bool Upload
            (string zipName, string baseFolder, string destName)
        {
            try
            {
                var host = System.Configuration.ConfigurationManager.AppSettings["ServerHost"];
                var userName = System.Configuration.ConfigurationManager.AppSettings["ServerUserName"];
                var pwd = System.Configuration.ConfigurationManager.AppSettings["ServerPwd"];
                //UBA.Ftp.Lib.ISftpHelper Helper = new SftpHelper("ubaservice.chinacloudapp.cn", "ubaservice", "Uba20150316");
                UBA.Ftp.Lib.ISftpHelper Helper = new SftpHelper(host, userName, pwd);

                if (Helper.Connect())
                {
                    Console.WriteLine("connect");
                    if (Helper.Put(Path.Combine(baseFolder, zipName), destName))
                    {
                        Common.LogHelperNet.Info("上传文件成功：" + zipName, null);
                        Console.WriteLine("Upload successful");
                        return true;

                    }
                    else
                    {
                        Common.LogHelperNet.Info("上传文件失败：" + zipName, null);
                        Console.WriteLine("Upload Failure");

                    }
                }
                else
                {
                    Common.LogHelperNet.Info("上传文件失败：无法连接服务器" + zipName, null);
                    Console.WriteLine("connect failure");
                }
                Console.WriteLine(Helper.Disconnect());
                return false;

            }
            catch
            {
                return false;
            }
        }
    }
}
