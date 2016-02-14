using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UBA.Redis.SaveFileTask
{
    public class E2StatusCode
    {
        public static string OK = "11-001";//成功
        public static string Error_ParWro = "11-002";//传入内容格式错误
        public static string Error_NoData = "11-003";//传入内容为空
        public static string Error_Else = "11-004";//其他错误
        public static string Error_U_NL = "11-005";
    }
}
