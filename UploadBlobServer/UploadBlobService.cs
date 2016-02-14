using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using UBA.Blob.Lib;
using UBA.GzipCompress.Lib;

namespace UploadBlobServer
{
    public partial class UploadBlobService : ServiceBase
    {
        public UploadBlobService()
        {
            InitializeComponent();
        }
        static FileSystemWatcher watcher = new FileSystemWatcher();
        protected override void OnStart(string[] args)
        {
            System.Threading.ThreadPool.QueueUserWorkItem((obj) =>
            {
                bool isstart = false;
                while (true)
                {
                    if (!isstart)
                    {
                        isstart = true;
                        Run();
                    }
                    else
                    {
                        //UBA.Common.LogHelperNet.Info("程序运行中", null);
                        System.Threading.Thread.Sleep(5000);
                    }
                }
            });

        }
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private static void Run()
        {
            var path = System.Configuration.ConfigurationManager.AppSettings["FileWatchPath"];
            var filter = System.Configuration.ConfigurationManager.AppSettings["FilewatchFilter"];
            watcher.Filter = filter;
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
| NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Created += fileWatch_Created;
            watcher.Disposed += fileWatch_Disposed;
            // Begin watching.
            watcher.EnableRaisingEvents = true;
            // Wait for the user to quit the program.

        }
        static void fileWatch_Disposed(object sender, EventArgs e)
        {
            Console.WriteLine("was disposed");
        }
        private static void fileWatch_Created(object sender, FileSystemEventArgs e)
        {
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {

                bool IsCreated = false;
                Console.WriteLine(e.FullPath + "was created:" + e.ChangeType.ToString());
                UBA.Common.LogHelperNet.Error(e.FullPath + "was created:" + e.ChangeType.ToString(), null);
                var zipFile = Path.GetFileName(e.FullPath);
                var dirName = Path.GetDirectoryName(e.FullPath);
                var destFile = Path.Combine(dirName, Path.GetFileNameWithoutExtension(zipFile) + ".txt");
                while (!IsCreated)
                {
                    try
                    {
                        System.Threading.Thread.Sleep(10000);
                        var result = new GZipResult();
                        GzipCompressFile.DecompressOneFile(e.FullPath, destFile, result);
                        if (!result.Errors)
                        {
                            IsCreated = true;
                            Console.WriteLine("解压成功");
                            UBA.Common.LogHelperNet.Info("解压成功", null);
                        }
                        else
                        {
                            Console.WriteLine("解压失败");
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("解压失败");
                        UBA.Common.LogHelperNet.Info("解压失败" + ex.Message, null);
                        continue;
                    }
                }
                if (BlobUpload(destFile))
                {
                    //TODO
                    UBA.Common.LogHelperNet.Info("开始创建HDI集群，并处理job", null);
                    UBA.Hadoop.CreateHDICluster.HadoopHelper.HadHoopStart();
                    UBA.Common.LogHelperNet.Info("提交job完成", null);
                }
                //BlobUpload(destFile);
            });
            th.Start();


        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileName"></param>
        private static bool BlobUpload(string fileName)
        {
            try
            {
                Console.WriteLine("开始上传文件");
                UBA.Common.LogHelperNet.Info("开始上传文件", null);
                var start = DateTime.Now;
                BlobStorageManager blob = new BlobStorageManager("StorageConnectionString");
                var inputContainer = System.Configuration.ConfigurationManager.AppSettings["ContainerName"];
                //删除容器
                blob.DeleteContainer("ubadata");
                UBA.Common.LogHelperNet.Info("开始创建容器", null);
                blob.CreateContainer(inputContainer);
                UBA.Common.LogHelperNet.Info("创建容器成功", null);
                var blobName = Path.GetFileNameWithoutExtension(fileName) + ".txt";

                Task.WaitAll(blob.UploadFromFile(fileName, inputContainer, blobName));

                UBA.Common.LogHelperNet.Info("上传成功--耗时：" + DateTime.Now.Subtract(start).TotalMilliseconds, null);
                Console.WriteLine("上传成功");
                Console.WriteLine(DateTime.Now.Subtract(start).TotalMilliseconds);
                return true;
            }
            catch (Exception ex)
            {
                UBA.Common.LogHelperNet.Error("上传文件失败", ex);
                return false;
            }
        }

        /// <summary>
        /// 大文件，分块上传
        /// </summary>
        /// <param name="fileName"></param>
        private static bool PutBlockToBlob(string fileName)
        {
            try
            {
                Console.WriteLine("开始上传文件");
                UBA.Common.LogHelperNet.Info("开始上传文件", null);
                var start = DateTime.Now;
                BlobStorageManager blob = new BlobStorageManager("StorageConnectionString");
                var inputContainer = System.Configuration.ConfigurationManager.AppSettings["ContainerName"];
                UBA.Common.LogHelperNet.Info("开始创建容器", null);
                //删除之前数据
                //blob.DeleteContainer(inputContainer);
                blob.CreateContainer(inputContainer);
                UBA.Common.LogHelperNet.Info("创建容器成功", null);
                var blobName = Path.GetFileNameWithoutExtension(fileName) + ".txt";

                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    byte[] bytes = new byte[1024 * 1024 * 3];
                    int k = 0;
                    int id = 0;
                    List<string> blobIds = new List<string>();
                    while ((k = fs.Read(bytes, 0, bytes.Length)) > 0)
                    {
                        id++;
                        blob.PutBlock(inputContainer, blobName, id, blobIds, bytes);
                    }
                    blob.PutBlockList(inputContainer, blobName, blobIds);

                }
                UBA.Common.LogHelperNet.Info("上传成功--耗时：" + DateTime.Now.Subtract(start).TotalMilliseconds, null);
                Console.WriteLine("上传成功");
                Console.WriteLine(DateTime.Now.Subtract(start).TotalMilliseconds);
                return true;
            }
            catch (Exception ex)
            {
                UBA.Common.LogHelperNet.Error("上传文件失败", ex);
                return false;
            }
        }


        protected override void OnStop()
        {
        }
    }
}
