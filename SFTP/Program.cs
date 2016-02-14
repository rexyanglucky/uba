using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UBA.Ftp.Lib;

namespace SFTP
{
    internal class Program
    {

        private static void Main(string[] args)
        {
            //ThreadPool.QueueUserWorkItem(new WaitCallback(UploadFile));
            UploadFile(null);
            FileSharp.CopyDir(@"G:\ei_online\ei_net",@"//");
        }

        private static void UploadFile(object state)
        {
            try
            {

                Console.WriteLine("上传文件开始");
                UBA.Common.LogHelperNet.Info("上传文件开始：", null);
                string baseFolder = System.Configuration.ConfigurationManager.AppSettings["FileDirectory"];
                string destDir = System.Configuration.ConfigurationManager.AppSettings["DestDirectory"];
                var host = System.Configuration.ConfigurationManager.AppSettings["ServerHost"];
                var userName = System.Configuration.ConfigurationManager.AppSettings["ServerUserName"];
                var pwd = System.Configuration.ConfigurationManager.AppSettings["ServerPwd"];

                if (Directory.Exists(baseFolder))
                {
                    List<FileInfo> fileList = new List<FileInfo>();
                    ListFiles(baseFolder, ref fileList);
                    fileList = fileList.Where(m => m.Name.Contains(DateTime.Now.ToString("yyyy_MM_dd"))).ToList();
                    UBA.Ftp.Lib.ISftpHelper helper = new SftpHelper(host, userName, pwd);
                    if (helper.Connect())
                    {
                        Console.WriteLine("connect");

                        foreach (var item in fileList)
                        {
                            string destPath = destDir.TrimEnd('/') + "/" + item.Directory.Name;
                            helper.Mkdir(destDir, item.Directory.Name);
                            string destName = destPath + "/" + item.Name;

                            if (helper.Put(item.FullName, destName))
                            {
                                Console.WriteLine("上传文件成功：" + item.FullName);
                                UBA.Common.LogHelperNet.Info("上传文件成功：" + item.FullName, null);
                            }
                            else
                            {
                                UBA.Common.LogHelperNet.Info("上传文件失败：" + item.FullName, null);
                                Console.WriteLine("Upload Failure");

                            }
                        }
                        Console.WriteLine(helper.Disconnect());
                    }
                    else
                    {
                        UBA.Common.LogHelperNet.Info("上传文件失败：无法连接服务器" + DateTime.Now, null);
                        Console.WriteLine("connect failure");
                    }

                }
                else
                {
                    Console.WriteLine("请输入正确的文件夹");
                }
            }
            catch (Exception ex)
            {
                UBA.Common.LogHelperNet.Info("上传文件失败：" + DateTime.Now, ex);
            }
            Console.WriteLine("上传文件结束");
        }


        public static void ListFiles(string path, ref List<FileInfo> fileList)
        {
            if (!Directory.Exists(path)) return;
            DirectoryInfo dir = new DirectoryInfo(path);
            //不是目录   
            if (dir == null) return;
            FileSystemInfo[] files = dir.GetFileSystemInfos();
            for (int i = 0; i < files.Length; i++)
            {
                FileSystemInfo file = files[i];

                //是文件   
                if ((file.Attributes & FileAttributes.Directory) == 0)
                    fileList.Add((FileInfo)file);
                //对于子目录，进行递归调用   
                else
                    ListFiles(files[i].FullName, ref fileList);
            }
        }
    }
}
