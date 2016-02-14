using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UBA.Redis.SaveFileTaskV1
{
    public class RedisHelper
    {
        private static IRedisClientsManager prcm;
        public static IRedisClient redisClient;
        static RedisHelper()
        {
            try
            {

                RedisClientManagerConfig RedisConfig = new RedisClientManagerConfig();

                RedisConfig.AutoStart = true;
                RedisConfig.MaxReadPoolSize = 60;
                RedisConfig.MaxWritePoolSize = 60;
                var hostWrite = System.Configuration.ConfigurationManager.AppSettings["RedisServerWrite"];
                var arryWrite = hostWrite.Split(';').ToList();
                var hostRead = System.Configuration.ConfigurationManager.AppSettings["RedisServerRead"];
                var arryRead = hostWrite.Split(';').ToList();
                if (arryWrite.Count > 0 && arryRead.Count > 0)
                {
                    prcm = new PooledRedisClientManager(arryWrite, arryRead, RedisConfig);
                    redisClient = prcm.GetClient();
                }
                else
                {
                    throw new Exception("连接服务器失败,请检查配置文件");
                }
            }
            catch
            {
                throw new Exception("连接服务器失败,请检查配置文件");
            }


        }
        public static string DequeueItemFromList(string listId = "SaveFileQueue")
        {
            try
            {
                return redisClient.DequeueItemFromList(listId);
            }
            catch
            { return string.Empty; }

        }
        //保存数据到队列
        public static string EnqueueItemOnList(string value, string listId = "SaveFileQueue")
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return E2StatusCode.Error_NoData;
                }
                else
                {
                    if (value.Split('\f').Length != 2)
                    {
                        return E2StatusCode.Error_ParWro;
                    }
                    redisClient.EnqueueItemOnList(listId, value);
                    return E2StatusCode.OK;
                }
            }
            catch
            {
                return E2StatusCode.Error_Else;
            }

        }
        public static long GetListCount(string listId = "SaveFileQueue")
        {
            return redisClient.GetListCount(listId);
        }

    }
}
