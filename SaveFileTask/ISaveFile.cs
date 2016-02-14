using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UBA.Redis.SaveFileTask
{
    public interface ISaveFile
    {
        string SaveGzipToFile();
    }
}
