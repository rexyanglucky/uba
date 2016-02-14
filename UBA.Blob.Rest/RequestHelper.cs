using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UBA.Blob.Rest
{
    public class RequestHelper
    {
        public void CreateHelper(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("x-ms-version", DateTime.Now.ToString("yyyy-MM-dd"));
            request.Headers.Add("x-ms-date", DateTime.Now.ToString("yyyy-MM-dd"));
            request.ContentType = "text/plain; charset=UTF-8";
            request.Headers.Add("x-ms-blob-content-disposition", "attachment; filename=\"fname.ext\"");
            request.Headers.Add("x-ms-blob-type", "BlockBlob");
            request.Headers.Add("x-ms-meta-m1", "v1");
            request.Headers.Add("x-ms-meta-m2", "v2");
            request.Headers.Add("Authorization", "SharedKey myaccount:YhuFJjN4fAR8/AmBrqBz7MG2uFinQ4rkh4dscbj598g=");
            request.ContentLength = 11;
            var requestStream = request.GetRequestStream();
            var content = "Hello world";
            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            requestStream.Write(bytes, 0, bytes.Length);
            var resoponse = (HttpWebResponse)request.GetResponse();
            var rstream = resoponse.GetResponseStream();
            var rbytes = new Byte[resoponse.ContentLength];
            rstream.Read(rbytes, 0, (int)resoponse.ContentLength);
            System.Text.Encoding.UTF8.GetString(rbytes);

        }
    }
}
