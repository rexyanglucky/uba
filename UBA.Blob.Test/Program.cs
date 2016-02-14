using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UBA.Blob.Lib;

namespace UBA.Blob.Test
{
    class Program
    {
        static void Main(string[] args)
        {

            for (int k = 0; k < 100; k++)
            {

                var filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test" + k + ".txt");
                using (var streamWrite = File.CreateText(filename))
                {
                    streamWrite.Write(filename);
                }
                Console.WriteLine(k);
                PutBlockToBlob(filename);
                Console.WriteLine("successful");
                //Task.WaitAll(PutBlockToBlob(filename));
            }
            Console.ReadLine();
        }
        private async static Task BlobUpload(string fileName)
        {
            var start = DateTime.Now;
            BlobStorageManager blob = new BlobStorageManager("StorageConnectionString");
            var inputContainer = System.Configuration.ConfigurationManager.AppSettings["ContainerName"];
            blob.CreateContainer(inputContainer);
            var blobName = Path.GetFileNameWithoutExtension(fileName) + ".txt";
            await blob.UploadFromFile(fileName, inputContainer, blobName);
            Console.WriteLine(DateTime.Now.Subtract(start).TotalMilliseconds);
        }
        private static void PutBlockToBlob(string fileName)
        {
            var start = DateTime.Now;
            BlobStorageManager blob = new BlobStorageManager("StorageConnectionString");
            var inputContainer = System.Configuration.ConfigurationManager.AppSettings["ContainerName"];

            blob.CreateContainer(inputContainer);
            var blobName = Path.GetFileNameWithoutExtension(fileName) + ".txt";

            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                byte[] bytes = new byte[4096];
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
            Console.WriteLine(DateTime.Now.Subtract(start).TotalMilliseconds);
        }

    }
}
