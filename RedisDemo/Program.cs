using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Redis;
using UBA.Redis.SaveFileTask;


namespace UBA.Redis.RedisDemo
{
    class Program
    {

        static void Main(string[] args)
        {
            //string path = @"C:\Users\Administrator\Desktop\2015-03-15.data";
            //var decompStr = System.IO.File.ReadLines(path, Encoding.UTF8).ToList();
            //System.Threading.Thread.Sleep(10000);
            //for (int k = 0; k < decompStr.Count(); k++)
            //{
            //    var length= decompStr[k].Split('\f').Length;
            //    Console.WriteLine("line" + k.ToString() + ":" +length);
            //}
            //Console.ReadLine();
            // var msg = "H4sIAAAAAAAEADPgMagxBEIDGOQBAjMTIzMjUyNLnudTtvPwGEChmaERmDYyMDTVN9Y3NFUwsDIAIQALQjizRAAAAA==\fH4sIAAAAAAAAA4XOPQ7CMAwF4KtUmWkU58ekycQNkGD0kpZaMKUqZULcnQQGhASqPNnv05Pvok/XUQShiKHTEtBLkKAc8WWfT80x34YzsSEm3k3TYcnzSKylluUExFXrGoqNSMNSiurynl4VoBW4VpkWbAPb4FzQGKyx8cOS+uewg7jSh8EUh2at7+W8xW9nfzlf/sMoHk9hbNVLGwEAAA==";
            //var user = "H4sIAAAAAAAEADPgMagxBEIDGOQBAjMTIzMjUyNLnudTtvPwGEChmaERmDYyMDTVN9Y3NFUwsDIAIQALQjizRAAAAA==";
            //var baseinfo = "H4sIAAAAAAAAA4XOPQ7CMAwF4KtUmWkU58ekycQNkGD0kpZaMKUqZULcnQQGhASqPNnv05Pvok/XUQShiKHTEtBLkKAc8WWfT80x34YzsSEm3k3TYcnzSKylluUExFXrGoqNSMNSiurynl4VoBW4VpkWbAPb4FzQGKyx8cOS+uewg7jSh8EUh2at7+W8xW9nfzlf/sMoHk9hbNVLGwEAAA==";
            //var msg = user + "\f" + baseinfo;
            //var msgm = "H4sIAAAAAAAEADPgMawxQIY8QGBmYmRsaWFqzPN8ynYeHgMoNDUBU0YGhqb6xvqG5goGVgYgBACRFpPIQwAAAA==";
            //var msgb = "H4sIAAAAAAAEAKtWSkosTlWyMjMxMra0MDXmMeBBAGulxOQSJSswO8nAkMfIwNBU31jf0FzB0MTK2NDKyNS6FgA4jayJQQAAAA==";
            //GetTotalAction(baseinfo, user);
            //using (IRedisClient client = new PooledRedisClientManager(new string[] { "192.168.140.23:8369" }).GetClient())
            //{
            //    client.Add("rex", "rexyang");
            //    Console.WriteLine(client.Get<string>("rex"));
            //    Console.ReadLine();
            //}
            //RedisHelper.redisClient.RemoveAllFromList("SaveFileQueue");
            //var count = RedisHelper.GetListCount();
            //for (int k = 0; k < 10; k++)
            //{
            //    var r = GetTotalAction(msgb, msgm);
            //    //RedisHelper.EnqueueItemOnList(msg);
            //    Console.WriteLine(k + ":" + r);
            //}
            //Console.ReadLine();
            ////Console.WriteLine("******************************");
            //SaveFileTask.SaveFile sf = new SaveFileTask.SaveFile();
            for (int k = 0; k < 10; k++)
            {
                var abc = "aaa" + "\f" + "bbbc";
                RedisHelper.EnqueueItemOnList(abc);
            }
            Console.ReadLine();
            var className = System.Configuration.ConfigurationSettings.AppSettings["SaveFileClass"];
            SaveFileTask.ISaveFile sf = SaveFileTask.SaveFileFactory.GetInstance(className);
            sf.SaveGzipToFile();
            Console.ReadLine();
        }

        public static string GetTotalAction(string content, string uinfo)
        {
            //string a ="124,0,1,2,3";
            //string b ="b01,20140507123040;b01,20140507123340;";
            //var obj="{base:"+a+",act:"+b+"}";
            //var tcpSave = (SaveFileRemoting.SaveFile)Activator.GetObject(typeof(SaveFileRemoting.SaveFile), "tcp://192.168.140.21:8088/Save");
            string result = string.Empty;
            try
            {
                result = RedisHelper.EnqueueItemOnList(uinfo + "\f" + content);
            }
            catch (Exception)
            {
            }
            return result;
        }



    }
}
