using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using log4net;
using System.Threading.Tasks;
using ServiceStack.Redis;

namespace UBA.Redis.SaveFileTask
{
    /*
     * 1.从redis队列中读取数据格式为：userInfo\fBaseInfo
     * 2.使用Gzip解压数据
     * 3.将解压数据进行分析，重新格式化
     * 4.用log4Net将数据保存磁盘文件
     */
    public class SaveFile : ISaveFile
    {
        public SaveFile()
        {
            log4net.LogManager.GetLogger("LogFile").Error("调用构造函数");
        }
        ~SaveFile()
        {
            log4net.LogManager.GetLogger("LogFile").Error("调用析构函数");
        }
        /// <summary>
        /// 解压字符串
        /// </summary>
        private string DecompressByQueue(string compressStr)
        {
            try
            {
                string compBase = string.Empty;
                string compUserInfo = string.Empty;


                if (!string.IsNullOrEmpty(compressStr))
                {
                    var compArry = compressStr.Split('\f');
                    if (compArry.Length > 1)
                    {
                        compUserInfo = compArry[0];
                        compBase = compArry[1];
                    }
                    if (!string.IsNullOrEmpty(compBase) && !string.IsNullOrEmpty(compUserInfo))
                    {
                        string decompBase = GzipCompress.DecompressString(compBase);
                        string decompUserInfo = GzipCompress.DecompressString(compUserInfo);
                        string decompAll = string.Empty;
                        if (CreateMsg(decompBase, decompUserInfo, ref decompAll).Equals(E2StatusCode.OK))
                        {
                            return decompAll;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                log4net.LogManager.GetLogger("LogFile").Error("解压字符串失败", ex);
            }
            return string.Empty;
        }

        /// <summary>
        /// 保存到文件
        /// </summary>
        private void Save(object obj)
        {
            var start = DateTime.Now;
            while (true)
            {
                var start0 = DateTime.Now;
                var compressStr = RedisHelper.DequeueItemFromList();

                if (!string.IsNullOrEmpty(compressStr))
                {
                    string decompAll = DecompressByQueue(compressStr);

                    if (!string.IsNullOrEmpty(decompAll))
                    {
                        LogHelperNet.Info(decompAll, null);
                    }
                    else
                    {
                        Console.WriteLine(DateTime.Now.Subtract(start).TotalMilliseconds);
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    Console.WriteLine(DateTime.Now.Subtract(start).TotalMilliseconds);
                    Thread.Sleep(1000);
                }
            }
        }
        public string SaveGzipToFile()
        {
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Save));
                //if (!haveThread)
                //{
                //    //开启线程处理队列内容
                //    for (int k = 0; k < 5; k++)
                //    {
                //        Thread th = new Thread(new ParameterizedThreadStart(Save));
                //        //Thread th = new Thread(new ThreadStart(Save));
                //        th.Start(k);
                //    }
                //    haveThread = true;
                //}
                return E2StatusCode.OK;
            }
            catch (Exception ex)
            {
                log4net.LogManager.GetLogger("LogFile").Error("SaveGzipToFile写入文件失败", ex);
                return E2StatusCode.Error_Else;
            }
        }
        private string CreateMsg(string appMsg, string userInfo, ref string result)
        {
            StringBuilder resultBuilder = new StringBuilder();
            string splitor = "\"act\":";
            string userInfoFlag = "\"uinfo\":";
            string[] strArray = appMsg.Split(new string[] { splitor }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length != 2)
            {
                log4net.LogManager.GetLogger("LogFile").Error("base数据格式不对");
                return E2StatusCode.Error_ParWro;
            }
            var baseStr = strArray[0];
            var preStr = baseStr + userInfoFlag + "\"" + userInfo + "\",";
            var actStr = strArray[1];
            string[] actArray = actStr.Split(';');
            var actLength = actArray.Length;
            if (actLength == 0)
            {
                log4net.LogManager.GetLogger("LogFile").Error("act数据格式不对");
                return E2StatusCode.Error_ParWro;
            }

            for (int k = 0; k < actLength - 1; k++)
            {
                if (!string.IsNullOrEmpty(actArray[k]))
                {
                    var act = actArray[k].Trim('"');
                    //转换时间开始 
                    string[] actarray = act.Split('\f');
                    var actTime = actarray[actarray.Length - 1];
                    DateTime dt = DateTime.Parse(actTime);
                    //TimeSpan ts = new TimeSpan();
                    var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var actTimeNew = Convert.ToInt64((dt - start).TotalSeconds).ToString();
                    //var actTimeNew = dt.ToString("yyyy-MM-dd HH:mm:ss");
                    actarray[actarray.Length - 1] = actTimeNew;
                    var actNew = string.Join("\f", actarray);
                    //转换时间结束
                    act = "\"" + actNew + "\"";
                    var actInfo = preStr + splitor + act + "}";

                    //TODO 将\f 替换为\
                    var actnew = actInfo.Replace('\f', '\\').Replace("{\"base\":\"", "").Replace("\",\"uinfo\":\"", "\\").Replace("\",\"act\":\"", "\\").Replace("\"}", "").Replace("|", "\\");

                    //TODO 硬编码 将android客户端老版本发过来的错误格式数据过滤掉
                    if (!actnew.Contains("uinfo"))
                        resultBuilder.AppendLine(actnew);

                }
            }
            result = resultBuilder.ToString();


            return E2StatusCode.OK;
        }
    }

}
