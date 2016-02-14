using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UBA.Ftp.Lib;
using System.Diagnostics;
namespace UBA.FTP.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            UBA.Ftp.Lib.ISftpHelper Helper = new SftpHelper("ubaservice.chinacloudapp.cn", "ubaservice", "Uba20150316");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            if (Helper.Connect())
            {
                Console.WriteLine("connect");
                for (int i = 0; i < 1; i++)
                {
                    if (Helper.Put(@"/usr/local/src/2015-03-15.txt", @"/home/ubaservice/" + i + ".txt"))
                    {
                        Console.WriteLine(i);
                    }
                }
            }
            Helper.Disconnect();
            watch.Stop();
            Console.WriteLine(watch.Elapsed);
            Console.ReadKey();

        }
    }
}
