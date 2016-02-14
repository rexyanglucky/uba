using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using UBA.Blob.Lib;
using UBA.GzipCompress.Lib;

namespace UBA.Blob.DecompAndUpload
{
    class Program
    {
        static void Main(string[] args)
        {
            //监控指定文件夹
            Run();
            Console.ReadKey();

            //FileWatch.Lib.FileWatchHelper fileWatchHelper = new FileWatch.Lib.FileWatchHelper("");
            //fileWatchHelper
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Run()
        {

            var path = System.Configuration.ConfigurationManager.AppSettings["FileWatchPath"];
            var filter = System.Configuration.ConfigurationManager.AppSettings["FilewatchFilter"];
            FileSystemWatcher watcher = new FileSystemWatcher(path, filter);
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
   | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.Created += fileWatch_Created;
            watcher.Disposed += fileWatch_Disposed;
            // Begin watching.
            watcher.EnableRaisingEvents = true;
            // Wait for the user to quit the program.
            Console.WriteLine("Press \'q\' to quit the sample.");
            while (Console.Read() != 'q') ;
        }
        static void fileWatch_Disposed(object sender, EventArgs e)
        {
            Console.WriteLine("was disposed");
        }
        private static void fileWatch_Created(object sender, FileSystemEventArgs e)
        {
            bool IsCreated = false;
            Console.WriteLine(e.FullPath + "was created:" + e.ChangeType.ToString());
            var zipFile = Path.GetFileName(e.FullPath);
            var dirName = Path.GetDirectoryName(e.FullPath);
            var destFile = Path.Combine(dirName, Path.GetFileNameWithoutExtension(zipFile) + ".txt");
            while (!IsCreated)
            {
                try
                {
                    var result = new GZipResult();
                    GzipCompressFile.DecompressOneFile(e.FullPath, destFile, result);
                    if (!result.Errors)
                    {
                        IsCreated = true;
                        Console.WriteLine("解压成功");
                    }
                    else
                    {
                        Console.WriteLine("解压失败");
                        System.Threading.Thread.Sleep(5000);
                    }
                }
                catch (Exception ex)
                {
                    System.Threading.Thread.Sleep(50);
                }

            }
            Console.WriteLine("开始上传文件");
            PutBlockToBlob(destFile);
            Console.WriteLine("上传文件成功");
        }
        private static void BlobUpload(string fileName)
        {
            var start = DateTime.Now;
            BlobStorageManager blob = new BlobStorageManager("StorageConnectionString");
            var inputContainer = System.Configuration.ConfigurationManager.AppSettings["ContainerName"];
            blob.CreateContainer(inputContainer);
            var blobName = Path.GetFileNameWithoutExtension(fileName) + ".txt";
            Task.WaitAll(blob.UploadFromFile(fileName, inputContainer, blobName));
            Console.WriteLine(DateTime.Now.Subtract(start).TotalMilliseconds);
        }
        private static bool PutBlockToBlob(string fileName)
        {
            try
            {
                Console.WriteLine("开始上传文件");
                var start = DateTime.Now;
                BlobStorageManager blob = new BlobStorageManager("StorageConnectionString");
                var inputContainer = System.Configuration.ConfigurationManager.AppSettings["ContainerName"];
                UBA.Common.LogHelperNet.Info("开始创建容器", null);
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


    }
}
