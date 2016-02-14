using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UBA.Mysql.lib;

namespace UBA.Mysql.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            MySqlHelper sqlhelper = new MySqlHelper();
            sqlhelper.getConnection();
            sqlhelper.createTransaction();
            string sql = "INSERT INTO `YN_TEST` VALUES ('2', 'yin');";
            var time = sqlhelper.executeUpdate(sql);
            Console.ReadLine();
        }
    }
}
