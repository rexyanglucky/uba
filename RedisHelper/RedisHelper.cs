using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisHelper
{
    public class RedisHelper
    {
        private static IRedisClientsManager prcm;
        public static IRedisClient redisClient;
        private const string listId = "SaveFileQueue";
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
        /// <summary>
        /// 存入Redis
        /// </summary>
        /// <param name="UserInfo">用户信息</param>
        /// <param name="AppValue">Aciton信息</param>
        /// <returns></returns>
        public static string EnqueueItemOnList(string UserInfo, string AppValue)
        {
            try
            {
                var value = UserInfo + "\f" + AppValue;
                if (string.IsNullOrEmpty(value))
                {
                    return E2StatusCode.Error_NoData;
                }
                else
                {
                    redisClient.EnqueueItemOnList(listId, value);
                    return E2StatusCode.OK;
                }
            }
            catch
            {
                return E2StatusCode.Error_Else;
            }

        }

    }
}
