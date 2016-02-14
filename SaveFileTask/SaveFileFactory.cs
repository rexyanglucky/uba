using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UBA.Redis.SaveFileTask
{
    public class SaveFileFactory
    {
        public static ISaveFile GetInstance(string name)
        {
            var objAssembly = Assembly.Load("SaveFileTask");
            var obj = objAssembly.CreateInstance("UBA.Redis.SaveFileTask." + name);
            log4net.LogManager.GetLogger("LogFile").Error("实例化");
            log4net.LogManager.GetLogger("LogFile").Error("实例化" + obj.GetType());
            return (ISaveFile)obj;
        }
    }
}
