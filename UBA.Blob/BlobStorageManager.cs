using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UBA.Blob.Lib
{
    public class BlobStorageManager
    {
        private readonly CloudStorageAccount _account;
        private readonly CloudBlobClient _blobClient;


        public BlobStorageManager(string connectionStringName)
        {

            var blobServer = ConfigurationManager.AppSettings[connectionStringName];
            _account = CloudStorageAccount.Parse(blobServer);
            _blobClient = _account.CreateCloudBlobClient();
        }


        public void CreateContainer(string containerName)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            Console.WriteLine("inputContainer:" + containerName);
            Console.WriteLine("EndPoint:" + _account.BlobEndpoint);
            if (container.CreateIfNotExists())
            {
                container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob }
                );
            }
        }

        /// <summary>
        /// 上传流文件到blob
        /// </summary>
        /// <param name="memoryStream">文件流</param>
        /// <param name="containerName">容器名称</param>
        /// <param name="blobName"></param>
        public void UploadFromStream(Stream memoryStream, string containerName, string blobName)
        {

            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);

            CloudBlockBlob blockBob = container.GetBlockBlobReference(blobName);

            blockBob.UploadFromStream(memoryStream);
        }
        public bool PutBlock(string containerName, string blobName, int blockId, List<string> blockIds, byte[] content)
        {
            try
            {
                CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
                CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

                string blockIdBase64 = Convert.ToBase64String(System.BitConverter.GetBytes(blockId));

                UTF8Encoding utf8Encoding = new UTF8Encoding();
                using (MemoryStream memoryStream = new MemoryStream(content))
                {
                    blob.PutBlock(blockIdBase64, memoryStream, null);
                }

                //blockIds[blockId] = blockIdBase64;
                blockIds.Add(blockIdBase64);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // Put block list - complete creation of blob based on uploaded content.  
        // Return true on success, false if already exists, throw exception on error.  

        public bool PutBlockList(string containerName, string blobName, List<string> blockIds)
        {
            try
            {
                CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
                CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

                blob.PutBlockList(blockIds);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 异步上传文件到blob
        /// </summary>
        /// <param name="path"></param>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        public Task UploadFromFile(string path, string containerName, string blobName)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBob = container.GetBlockBlobReference(blobName);
            return blockBob.UploadFromFileAsync(path, FileMode.OpenOrCreate);

        }
        public void BeginUploadFromFile(string path, string containerName, string blobName, AsyncCallback callback, object state)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBob = container.GetBlockBlobReference(blobName);
            blockBob.BeginUploadFromFile(path, FileMode.OpenOrCreate, callback, state);
        }

        public void UploadFromText(string text, string containerName, string blobName)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);

            CloudBlockBlob blockBob = container.GetBlockBlobReference(blobName);

            blockBob.UploadText(text);
        }

        public FileStream DownloadToStream(string containerName, string blobName, FileStream fileStream)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            blockBlob.DownloadToStream(fileStream);

            return fileStream;
        }

        public string DownloadToText(string containerName, string blobName)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            string text;

            using (var memoryStream = new MemoryStream())
            {
                blockBlob.DownloadToStream(memoryStream);
                text = System.Text.Encoding.Default.GetString(memoryStream.ToArray());
            }

            return text;
        }

        public List<string> ListBlobs(string containerName)
        {
            List<string> blobs = new List<string>();
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);

            foreach (IListBlobItem item in container.ListBlobs(null, false))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    blobs.Add(blob.Name);
                }
            }

            return blobs;
        }

        public string GetdSpecificUrlFromBlobStorage(string containerName, string blobName)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            try
            {
                blockBlob.FetchAttributes();

                return blockBlob.Uri.OriginalString;
            }
            catch (Exception ex)
            {
                throw new Exception("Blob não existe");
            }
        }

        public bool DeleteSpecificBlob(string containerName, string blobName)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            if (blockBlob.DeleteIfExists())
            {
                return true;
            }
            return false;

        }
        /// <summary>
        /// 删除容器
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public bool DeleteContainer(string containerName)
        {
            try
            {
                CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
                container.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
