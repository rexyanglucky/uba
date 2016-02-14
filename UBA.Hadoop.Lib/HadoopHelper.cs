using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UBA.Hadoop.Lib
{
    public class HadoopHelper
    {
        public static void HadHoopStart(string[] args)
        {
            while (true)
            {
                if (UBA.Hadoop.Lib.HIDClusterHelper.CreateHDICluster())
                {
                    break;
                }
            }
            var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var timeStamp = Convert.ToInt64((DateTime.Now - start).TotalSeconds).ToString();

            string jobName = "GetUserInfo" + timeStamp;
            var job = new Microsoft.Hadoop.Client.HiveJobCreateParameters()
            {
                JobName = jobName,
                StatusFolder = "/ShowTableStatusFolder",
                File = "/Uba_Base.hql",
                RunAsFileJob = true

            };
            SubmitHiveJob.SubmitJob(job, SubmitHiveCallBack);
            timeStamp = Convert.ToInt64((DateTime.Now - start).TotalSeconds).ToString();
            jobName = "GeActionInfo" + timeStamp;

            var actionjob = new Microsoft.Hadoop.Client.HiveJobCreateParameters()
            {
                JobName = jobName,
                StatusFolder = "/ShowTableStatusFolder",
                File = "/Uba_Group.hql",
                RunAsFileJob = true

            };
            SubmitHiveJob.SubmitJob(actionjob, SubmitHiveCallBackUpdate);

            Console.ReadLine();
        }
        static string SubmitHiveCallBack(Stream stream)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(UserActionBll.InsertSql, stream);
            return string.Empty;
        }
        static string SubmitHiveCallBackUpdate(Stream stream)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(UserActionBll.UpdateSql, stream);
            //删除集群
            //UBA.Hadoop.Lib.HIDClusterHelper.DeleteHDICluster();
            return string.Empty;
        }
    }
}
