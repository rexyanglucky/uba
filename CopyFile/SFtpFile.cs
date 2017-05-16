using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UBA.Ftp.Lib;

namespace CopyFile
{
    public class SFtpFile
    {
        public static void UploadFile(ServerModel destModel)
        {
            try
            {

                Console.WriteLine("上传文件开始");
                UBA.Common.LogHelperNet.Info("上传文件开始：", null);
                string baseFolder = destModel.SourceDirectory;
                string destDir = destModel.DestDirectory;
                var host = destModel.ServerHost;
                var userName = destModel.ServerUserName;
                var pwd = destModel.ServerPwd;

                if (Directory.Exists(baseFolder))
                {
                    List<FileInfo> fileList = new List<FileInfo>();
                    ListFiles(baseFolder, ref fileList);
                    //fileList = fileList.Where(m => m.Name.Contains(DateTime.Now.ToString("yyyy_MM_dd"))).ToList();
                    UBA.Ftp.Lib.ISftpHelper helper = new SftpHelper(host, userName, pwd);
                    if (helper.Connect())
                    {
                        //创建destDir
                        CopyDir(destModel.SourceDirectory, destModel.DestDirectory, helper);
                        //helper.Mkdir(destDir);
                        //foreach (var item in fileList)
                        //{

                        //    //string destPath = destDir.TrimEnd('/') + "/" + item.Directory.Name;
                        //    //helper.Mkdir(destDir, item.Directory.Name);

                        //    string destName = destDir + "/" + item.Name;

                        //    if (helper.Put(item.FullName, destName))
                        //    {
                        //        Console.WriteLine("上传文件成功：" + item.FullName);
                        //        UBA.Common.LogHelperNet.Info("上传文件成功：" + item.FullName, null);
                        //    }
                        //    else
                        //    {
                        //        UBA.Common.LogHelperNet.Info("上传文件失败：" + item.FullName, null);
                        //        Console.WriteLine("Upload Failure");

                        //    }
                        //}
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
        public static void CopyDir
            (string path, ref List<FileInfo> fileList, ISftpHelper helper)
        {
            if (!Directory.Exists(path)) return;
            DirectoryInfo dir = new DirectoryInfo(path);
            //不是目录   
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


        public static void CopyDir(string srcPath, string aimPath, ISftpHelper helper)
        {
            try
            {
                // 检查目标目录是否以目录分割字符结束如果不是则添加之
                if (!aimPath[aimPath.Length - 1].Equals(helper.DefPath))
                    aimPath += helper.DefPath;
                // 判断目标目录是否存在如果不存在则新建之
                if (!helper.DirExist(aimPath))
                {
                    helper.Mkdir(aimPath);
                }

                // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                //如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
                //string[] fileList = Directory.GetFiles(srcPath);
                string[] fileList = Directory.GetFileSystemEntries(srcPath);
                //遍历所有的文件和目录
                foreach (string file in fileList)
                {
                    //先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                    if (Directory.Exists(file))
                        CopyDir(file, aimPath + Path.GetFileName(file), helper);
                    //否则直接Copy文件
                    else
                        //File.Copy(file, aimPath + Path.GetFileName(file), true);
                        helper.Put(file, aimPath + Path.GetFileName(file));
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.ToString());
            }
        }

        public static void CopyDir(string srcPath, string aimPath, ISftpHelper helper, Action<int, string> ShowMessage)
        {
            try
            {
                //判断传入的是否是文件
                DirectoryInfo dir = new DirectoryInfo(srcPath);

                // 检查目标目录是否以目录分割字符结束如果不是则添加之
                if (!aimPath[aimPath.Length - 1].ToString().Equals(helper.DefPath))
                    aimPath += helper.DefPath;
                // 判断目标目录是否存在如果不存在则新建之
                if (!helper.DirExist(aimPath))
                {
                    helper.Mkdir(aimPath);
                }

                //是文件   
                if ((dir.Attributes & FileAttributes.Directory) == 0)
                {
                    helper.Put(srcPath, aimPath + Path.GetFileName(srcPath));
                    ShowMessage(0, srcPath + "-->" + aimPath + Path.GetFileName(srcPath));
                    return;
                }
                // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                //如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
                //string[] fileList = Directory.GetFiles(srcPath);
                string[] fileList = Directory.GetFileSystemEntries(srcPath);
                //遍历所有的文件和目录
                foreach (string file in fileList)
                {
                    //先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                    if (Directory.Exists(file))
                        CopyDir(file, aimPath + Path.GetFileName(file), helper, ShowMessage);
                    //否则直接Copy文件
                    else
                    {
                        helper.Put(file, aimPath + Path.GetFileName(file));
                        ShowMessage(0, file + "-->" + aimPath + Path.GetFileName(file));
                    }
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.ToString());
            }
        }


        internal static void UploadFile(ServerModel destModel, Action<int, string> ShowMessage)
        {
            try
            {
                string baseFolder = destModel.SourceDirectory;
                string destDir = destModel.DestDirectory;
                var host = destModel.ServerHost;
                var userName = destModel.ServerUserName;
                var pwd = destModel.ServerPwd;

                if (Directory.Exists(baseFolder) || File.Exists(baseFolder))
                {
                    List<FileInfo> fileList = new List<FileInfo>();

                    UBA.Ftp.Lib.ISftpHelper helper = new SftpHelper(host, userName, pwd);
                    if (helper.Connect())
                    {
                        //创建destDir
                        CopyDir(destModel.SourceDirectory, destModel.DestDirectory, helper, ShowMessage);
                        Console.WriteLine(helper.Disconnect());
                    }
                    else
                    {
                        ShowMessage(0, "上传文件失败：无法连接服务器" + DateTime.Now);
                        UBA.Common.LogHelperNet.Info("上传文件失败：无法连接服务器" + DateTime.Now, null);
                        Console.WriteLine("connect failure");
                    }

                }
                else
                {
                    Console.WriteLine("请输入正确的文件夹");
                    ShowMessage(0, "请输入正确的文件夹");
                }
            }
            catch (Exception ex)
            {
                ShowMessage(0, "上传文件失败");
                UBA.Common.LogHelperNet.Info("上传文件失败：" + DateTime.Now, ex);
            }

        }


        internal static void DownloadFile(ServerModel destModel, Action<int, string> ShowMessage)
        {
            try
            {
                string baseFolder = destModel.SourceDirectory;
                string destDir = destModel.DestDirectory;
                var host = destModel.ServerHost;
                var userName = destModel.ServerUserName;
                var pwd = destModel.ServerPwd;

                if (Directory.Exists(baseFolder) || File.Exists(baseFolder))
                {
                    List<FileInfo> fileList = new List<FileInfo>();

                    UBA.Ftp.Lib.ISftpHelper helper = new SftpHelper(host, userName, pwd);
                    if (helper.Connect())
                    {
                        //创建destDir
                        CopyDir(destModel.SourceDirectory, destModel.DestDirectory, helper, ShowMessage);
                        Console.WriteLine(helper.Disconnect());
                    }
                    else
                    {
                        ShowMessage(0, "下载文件失败：无法连接服务器" + DateTime.Now);
                        UBA.Common.LogHelperNet.Info("下载文件失败：无法连接服务器" + DateTime.Now, null);
                        Console.WriteLine("connect failure");
                    }

                }
                else
                {
                    Console.WriteLine("请输入正确的文件夹");
                    ShowMessage(0, "请输入正确的文件夹");
                }
            }
            catch (Exception ex)
            {
                ShowMessage(0, "下载文件失败");
                UBA.Common.LogHelperNet.Info("下载文件失败：" + DateTime.Now, ex);
            }
        }
    }
}
