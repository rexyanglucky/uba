using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UBA.Hadoop.Lib;

namespace UBA.Hadoop.CreateHDICluster
{
    public class HadoopHelper
    {
        private static readonly DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static void HadHoopStart()
        {
            while (true)
            {
                if (UBA.Hadoop.Lib.HIDClusterHelper.CreateHDICluster())
                {
                    break;
                }
            }
            //获取用户信息
            SubmitJob("GetUserInfo", "Uba_Base.hql", SubmitHiveCallBack, "job_1428138175046_0231");
            //SubmitJob("GetUserInfo", "Uba_Base.hql", SubmitHiveCallBack);
        }

        private static void SubmitJob(string jobDesc, string jobFile, Func<Stream, string> submitCallback,
            string jobid = "")
        {
            var timeStamp = Convert.ToInt64((DateTime.Now - start).TotalSeconds).ToString();
            //获取行为信息
            timeStamp = Convert.ToInt64((DateTime.Now - start).TotalSeconds).ToString();
            string jobName = jobDesc + timeStamp;
            var actionjob = new Microsoft.Hadoop.Client.HiveJobCreateParameters()
            {
                JobName = jobName,
                StatusFolder = "/ShowTableStatusFolder",
                File = "/" + jobFile,
                RunAsFileJob = true

            };

            SubmitHiveJob.SubmitJob(actionjob, submitCallback, jobid);
        }

        //插入用户信息
        private static string SubmitHiveCallBack(Stream stream)
        {
            //UserActionBll.InsertSql(stream);
            //SubmitJob("GeActionInfo", "Uba_Group.hql", SubmitHiveCallBackUpdate, "job_1428138175046_0235");
            //SubmitJob("GeActionInfo", "Uba_Group.hql", SubmitHiveCallBackUpdate);

            //SubmitJob("PracticeInfo", "Uba_Practice.hql", SubmitHiveCallBackUpdatePractice, "job_1428138175046_0240");
            SubmitJob("PracticeInfo", "Uba_Practice.hql", SubmitHiveCallBackUpdatePractice);

            SubmitJob("PKInfo", "Uba_Pk.hql", SubmitHiveCallBackUpdatePK, "job_1428138175046_0249");
            //SubmitJob("PKInfo", "Uba_Pk.hql", SubmitHiveCallBackUpdatePK);
            return string.Empty;
        }

        private static string SubmitHiveCallBackUpdate(Stream stream)
        {
            UserActionBll.UpdateSql(stream);
            //System.Threading.ThreadPool.QueueUserWorkItem(UserActionBll.UpdateSql, stream);
            //删除集群
            //UBA.Hadoop.Lib.HIDClusterHelper.DeleteHDICluster();
            return string.Empty;
        }

        private static string SubmitHiveCallBackUpdatePractice(Stream stream)
        {
            UserActionBll.PracticeSql(stream);
            //System.Threading.ThreadPool.QueueUserWorkItem(UserActionBll.PracticeSql, stream);
            return string.Empty;
        }

        private static string SubmitHiveCallBackUpdatePK(Stream stream)
        {
            UserActionBll.PkSql(stream);
            UBA.Hadoop.Lib.HIDClusterHelper.DeleteHDICluster();
            //System.Threading.ThreadPool.QueueUserWorkItem(UserActionBll.PkSql, stream);
            return string.Empty;
        }


    }
}
