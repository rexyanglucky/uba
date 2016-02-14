using Microsoft.WindowsAzure.Management.HDInsight;
using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace UBA.Hadoop.Lib
{
    public class HIDClusterHelper
    {
        public static bool CreateHDICluster()
        {
            try
            {
                Console.WriteLine("Creating the HDInsight cluster ...");
                Stopwatch watch = new Stopwatch();
                watch.Start();
                string pfx = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ubaClient.pfx");
                //string pfx = AppDomain.CurrentDomain.BaseDirectory + "ubaClient.pfx";
                string subscriptionid = System.Configuration.ConfigurationSettings.AppSettings["Subscriptionid"];
                string clustername = System.Configuration.ConfigurationSettings.AppSettings["Clustername"];
                string location = System.Configuration.ConfigurationSettings.AppSettings["Location"];
                string storageaccountname = System.Configuration.ConfigurationSettings.AppSettings["StorageAccountName"];
                string storageaccountkey = System.Configuration.ConfigurationSettings.AppSettings["StorageAccountKey"];
                string containername = System.Configuration.ConfigurationSettings.AppSettings["ContainerName"];
                string username = System.Configuration.ConfigurationSettings.AppSettings["UserName"];
                string password = System.Configuration.ConfigurationSettings.AppSettings["Password"];
                int clustersize = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["Clustersize"]);


                X509Certificate2 cert = new X509Certificate2(pfx, "1");//c8321a5a-6f7e-4f2e-a0c8-7b19f076877a

                HDInsightCertificateCredential creds = new HDInsightCertificateCredential(new Guid(subscriptionid), cert, new Uri("https://management.core.chinacloudapi.cn"));
                var client = HDInsightClient.Connect(creds);
                //Console.WriteLine(clustername + "    " + location);
                if (client.GetCluster(clustername) == null)
                {
                    ClusterCreateParameters clusterInfo = new ClusterCreateParameters()
                    {
                        Name = clustername,
                        Location = location,
                        DefaultStorageAccountName = storageaccountname,
                        DefaultStorageAccountKey = storageaccountkey,
                        DefaultStorageContainer = containername,
                        UserName = username,
                        Password = password,
                        ClusterSizeInNodes = clustersize,
                        ClusterType = ClusterType.Hadoop,
                        HeadNodeSize = NodeVMSize.Large

                    };
                    // Create the cluster


                    ClusterDetails cluster = client.CreateCluster(clusterInfo);


                    Console.WriteLine("Created cluster: {0}.", cluster.ConnectionUrl);
                    watch.Stop();
                    Console.WriteLine("耗时:" + watch.Elapsed + "毫秒");
                    Console.WriteLine();

                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "/r/n" + ex.TargetSite);
                return false;
            }

        }

        public static bool DeleteHDICluster()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            string pfx = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ubaClient.pfx");
            string subscriptionid = System.Configuration.ConfigurationSettings.AppSettings["Subscriptionid"];
            string clustername = System.Configuration.ConfigurationSettings.AppSettings["Clustername"];


            X509Certificate2 cert = new X509Certificate2(pfx, "1");//c8321a5a-6f7e-4f2e-a0c8-7b19f076877a

            HDInsightCertificateCredential creds = new HDInsightCertificateCredential(new Guid(subscriptionid), cert, new Uri("https://management.core.chinacloudapi.cn"));
            var client = HDInsightClient.Connect(creds);
            if (client.GetCluster(clustername) != null)
            {
                client.DeleteCluster(clustername);
            }
            // Create the cluster
            Console.WriteLine("Deleting the HDInsight cluster ...");

            Console.WriteLine("Deleted cluster: {0}.", clustername);
            watch.Stop();
            Console.WriteLine("耗时:" + watch.Elapsed + "毫秒");
            Console.WriteLine();
            return true;
        }
    }
}
