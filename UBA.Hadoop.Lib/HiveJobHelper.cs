using Microsoft.Hadoop.Client;
using Microsoft.WindowsAzure.Management.HDInsight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UBA.Hadoop.Lib
{
    public class SubmitHiveJob
    {
        private static void WaitForJobCompletion(JobCreationResults jobResults, IJobSubmissionClient client)
        {
            JobDetails jobInProgress = client.GetJob(jobResults.JobId);
            while (jobInProgress.StatusCode != JobStatusCode.Completed && jobInProgress.StatusCode != JobStatusCode.Failed)
            {
                jobInProgress = client.GetJob(jobInProgress.JobId);
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        public static string SubmitJob(HiveJobCreateParameters hiveJobDefinition, Func<Stream, string> submitCallback, string jobid = "")
        {
            string msg = string.Empty;
            var start = DateTime.Now;
            Console.WriteLine("开始提交job：" + hiveJobDefinition.JobName);
            UBA.Common.LogHelperNet.Info("开始提交job：" + hiveJobDefinition.JobName, null);
            string pfx = AppDomain.CurrentDomain.BaseDirectory + "ubaClient.pfx";
            string subscriptionid = System.Configuration.ConfigurationSettings.AppSettings["Subscriptionid"];
            string clustername = System.Configuration.ConfigurationSettings.AppSettings["Clustername"];
            System.IO.Stream stream = null;
            try
            {
                X509Certificate2 cert = new X509Certificate2(pfx, "1");//c8321a5a-6f7e-4f2e-a0c8-7b19f076877a
                JobSubmissionCertificateCredential creds = new JobSubmissionCertificateCredential(new Guid(subscriptionid), cert, clustername, new Uri("https://management.core.chinacloudapi.cn"));
                // Submit the Hive job
                var jobClient = JobSubmissionClientFactory.Connect(creds);
                if (!string.IsNullOrEmpty(jobid))
                {
                    stream = jobClient.GetJobOutput(jobid);
                }
                else
                {
                    JobCreationResults jobResults = jobClient.CreateHiveJob(hiveJobDefinition);
                    msg = string.Format("提交job成功,耗时{0}秒\r\n开始处理job", DateTime.Now.Subtract(start).TotalMilliseconds / 1000);
                    Console.WriteLine(msg);
                    UBA.Common.LogHelperNet.Info(msg, null);
                    start = DateTime.Now;
                    //// Wait for the job to complete
                    WaitForJobCompletion(jobResults, jobClient);
                    stream = jobClient.GetJobOutput(jobResults.JobId);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("提交job失败：" + ex.Message);
                UBA.Common.LogHelperNet.Error("提交job失败：", ex);
                return "error";
            }
            msg = string.Format("处理完成job,耗时{0}秒", DateTime.Now.Subtract(start).TotalMilliseconds / 1000);
            Console.WriteLine(msg);
            UBA.Common.LogHelperNet.Info(msg, null);
            return submitCallback(stream);
        }


    }
}
