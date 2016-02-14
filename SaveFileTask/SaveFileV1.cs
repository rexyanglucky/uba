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
    public class SaveFileV1 : ISaveFile
    {
        public SaveFileV1()
        {
            log4net.LogManager.GetLogger("LogFile").Error("调用构造函数");
        }
        ~SaveFileV1()
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
                        return decompBase;
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
                        if (decompAll.Contains("\"mid\""))
                        {
                            StringBuilder sb = new StringBuilder();

                            LogManager.GetLogger("log_in_information").Info(sb.AppendLine(decompAll).ToString());
                        }
                        else if (decompAll.Contains("acttype"))
                        {
                            StringBuilder sb = new StringBuilder();

                            LogManager.GetLogger("action_information").Info(sb.AppendLine(decompAll).ToString());
                        }

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
                return E2StatusCode.OK;
            }
            catch (Exception ex)
            {
                log4net.LogManager.GetLogger("LogFile").Error("SaveGzipToFile写入文件失败", ex);
                return E2StatusCode.Error_Else;
            }
        }
    }

}
