using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UBA.Ftp.Lib;

namespace UBA.CompressAndUploadV2
{
    class Program
    {
        private static ISftpHelper helper;
        static void Main(string[] args)
        {
            //TODO
            try
            {
                Common.LogHelperNet.Info("压缩文件并上传：", null);
                string baseFolder = System.Configuration.ConfigurationManager.AppSettings["FileDirectory"];

                string destFolder = System.Configuration.ConfigurationManager.AppSettings["DestDirectory"];

                List<DirectoryInfo> directoryList = new List<DirectoryInfo>();
                Common.IOHelper.ListDirectory(baseFolder, ref directoryList);

                directoryList = directoryList.Where(m => m.Name.Contains(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"))).ToList();
                directoryList = directoryList.Where(m => !m.Name.Contains("information")).ToList();

                foreach (var item in directoryList)
                {
                    string folder = Path.Combine(baseFolder, item.Parent.Name, item.Name);
                    string zipName = item.Name + ".zip";
                    string destDir = destFolder.TrimEnd('/') + "/" + item.Parent.Name;
                    string fromFolder = Path.Combine(baseFolder, item.Parent.Name);
                    if (CombineFilesByDirAndCompress(folder, fromFolder, zipName))
                    {
                        Common.LogHelperNet.Info("压缩文件成功：" + zipName, null);
                        Console.WriteLine("Compress successful");
                        var start = DateTime.Now;
                        //上传失败，休息10秒，再次上传
                        while (true)
                        {
                            if (Upload(zipName, fromFolder, destDir, zipName))
                            { break; }
                            else
                            {
                                System.Threading.Thread.Sleep(10000);
                            }
                        }

                        Common.LogHelperNet.Info("压缩上传文件结束，耗时" + DateTime.Now.Subtract(start).TotalMilliseconds + ":" + zipName, null);
                        Console.WriteLine("压缩上传文件结束，耗时" + DateTime.Now.Subtract(start).TotalMilliseconds);
                    }
                }
            }
            catch (Exception e)
            {
                Common.LogHelperNet.Info("压缩上传文件失败", e);
                Console.WriteLine(e.Message);
                Console.WriteLine("上传失败");
                Console.WriteLine("failure");
            }
            finally
            {
                Helper.Disconnect();
                Environment.Exit(0);
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
        #region sftp 上传
        public static ISftpHelper Helper
        {
            get
            {
                if (helper == null)
                {
                    var host = System.Configuration.ConfigurationManager.AppSettings["ServerHost"];
                    var userName = System.Configuration.ConfigurationManager.AppSettings["ServerUserName"];
                    var pwd = System.Configuration.ConfigurationManager.AppSettings["ServerPwd"];
                    //UBA.Ftp.Lib.ISftpHelper Helper = new SftpHelper("ubaservice.chinacloudapp.cn", "ubaservice", "Uba20150316");
                    helper = new SftpHelper(host, userName, pwd);
                    if (helper.Connect())
                    {
                        Console.WriteLine("connect");
                    }
                    else
                    {
                        Common.LogHelperNet.Info("上传文件失败：无法连接服务器", null);
                        Console.WriteLine("connect failure");
                        return null;
                    }
                }

                return helper;

            }
        }
        private static bool Upload
            (string zipName, string baseFolder, string destName)
        {
            try
            {

                //Helper.Mkdir(destDir, item.Directory.Name);
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

                Console.WriteLine(Helper.Disconnect());
                return false;

            }
            catch
            {
                return false;
            }
        }

        private static bool Upload(string zipName, string baseFolder, string destDir, string destName)
        {
            try
            {
                try
                {
                    Helper.Mkdir(destDir);
                }
                catch { }
                destName = destDir + "/" + destName;
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
                return false;

            }
            catch
            {
                return false;
            }
        }
        #endregion


    }
}
