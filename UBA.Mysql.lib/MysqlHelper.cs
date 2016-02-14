using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UBA.Mysql.lib
{
    public class MySqlHelper
    {
        private MySqlConnection myConnection;
        private MySqlCommand myCommand;
        private MySqlDataAdapter myAdapter;
        private MySqlTransaction myTransaction;
        // private string strConn = "Database=test;Data Source=192.168.58.128;User Id=root;Password=123456;pooling=false;CharSet=utf8;port=5008";
        //建立DB连接
        public void getConnection()
        {
            //StreamReader din = File.OpenText("TextFile.ini");
            //string contString = din.ReadLine();
            var strConn = System.Configuration.ConfigurationSettings.AppSettings["MysqlConn"];
            string contString = strConn;
            try
            {
                if (myConnection == null)
                {
                    myConnection = new MySqlConnection();
                    myConnection.ConnectionString = contString;
                    myConnection.Open();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        //数据查询操作
        public DataTable executeQuery(String sql)
        {
            DataTable myTable;
            try
            {
                myCommand = myConnection.CreateCommand();
                myCommand.CommandText = sql;
                myAdapter = new MySqlDataAdapter(myCommand);
                DataSet mySet = new DataSet();
                myAdapter.Fill(mySet, "selectDa");
                myTable = mySet.Tables["selectDa"];
                return myTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                myTable = new DataTable();
                return myTable;
            }
        }

        //数据插入,删除,更新操作
        public double executeUpdate(String sql)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                myCommand = myConnection.CreateCommand();
                myCommand.CommandText = sql;
                myCommand.ExecuteNonQuery();
                if (myTransaction == null)
                {
                    myConnection.Close();
                    myConnection = null;
                }
                //this.commitTransaction();
                DateTime endTime = DateTime.Now;
                TimeSpan ts = endTime.Subtract(startTime).Duration();

                return ts.TotalMilliseconds;
            }
            catch (Exception ex)
            {
                if (myTransaction != null)
                {
                    myTransaction.Rollback();
                    myTransaction = null;
                    //MessageBox.Show("数据发生错误，正在启用事务回滚！");
                }
                else if (myConnection == null)
                {
                    // MessageBox.Show("请启用事务！");
                }
                else
                {
                    //MessageBox.Show("发生错误！");
                }
                Console.WriteLine(ex);
            }
            return -1;
        }
        //创建事务
        public void createTransaction()
        {
            try
            {
                myTransaction = myConnection.BeginTransaction();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //MessageBox.Show("启用事务失败！");
            }
        }
        //提交事务
        public void commitTransaction()
        {
            try
            {
                if (myTransaction != null) myTransaction.Commit();

            }
            catch (Exception ex)
            {
                myTransaction.Rollback();
                Console.WriteLine(ex);
                // MessageBox.Show("数据发生错误，正在启用事务回滚！");
            }
            finally
            {
                myConnection.Close();
                myConnection = null;
            }
        }

        //提交事务
        public void commitTransactionKeepCon()
        {
            try
            {
                if (myTransaction != null) myTransaction.Commit();

            }
            catch (Exception ex)
            {
                myTransaction.Rollback();
                Console.WriteLine(ex);
                // MessageBox.Show("数据发生错误，正在启用事务回滚！");
            }

        }
    }
}
