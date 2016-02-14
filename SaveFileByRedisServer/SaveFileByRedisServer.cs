using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace UBA.Redis.SaveFileByRedisServer
{
    public partial class SaveFileByRedisServer : ServiceBase
    {
        public SaveFileByRedisServer()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Console.WriteLine("服务已启动");
            var className = System.Configuration.ConfigurationManager.AppSettings["SaveFileClass"];
            SaveFileTask.ISaveFile sf = SaveFileTask.SaveFileFactory.GetInstance(className);
            sf.SaveGzipToFile();
        }

        protected override void OnStop()
        {
            Console.WriteLine("服务关闭");
            this.Stop();
        }
    }
}
