using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Management.HDInsight;
using Microsoft.Hadoop.Client;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
namespace UBA.Azure.Hadoop.Job
{
    class Program
    {
        static void Main(string[] args)
        {
           


        }
        /// <summary>
        /// 函数用来等待 Hadoop 作业
        /// </summary>
        /// <param name="jobResults"></param>
        /// <param name="client"></param>
        private static void WaitForJobCompletion(JobCreationResults jobResults, IJobSubmissionClient client)
        {
            JobDetails jobInProgress = client.GetJob(jobResults.JobId);
            while (jobInProgress.StatusCode != JobStatusCode.Completed && jobInProgress.StatusCode != JobStatusCode.Failed)
            {
                jobInProgress = client.GetJob(jobInProgress.JobId);
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
        }
        HiveJobCreateParameters hiveJobDefinition = new HiveJobCreateParameters()
        {
            JobName = "show tables job",
            StatusFolder = "/ShowTableStatusFolder",
            Query = "show tables;"
        };

    }

}
