using System;
using System.Text;
using System.IO;
using UBA.Mysql.lib;

namespace UBA.Hadoop.CreateHDICluster
{
    public class UserActionBll
    {
        public static string CreateInsertSql(string jobResult, MySqlHelper sqlhelper)
        {
            StringBuilder str = new StringBuilder();
            if (!String.IsNullOrEmpty(jobResult) && !jobResult.StartsWith("Moved"))
            {
                var array = jobResult.Split('\t');
                var id = Guid.NewGuid();
                var userid = array[0];
                if (userid == "60009252")
                {
                    var sss = 1;
                }
                var sex = array[1];
                var ulev = array[2];
                var grade = array[3];
                var sta = array[4];
                var sch = array[5];
                var frs = array[6];
                var qq = array[7];
                var mobile = array[8];
                var rtm = array[9];
                var atime = array[10];
                var mdou = array[11];
                var kb = array[12];
                var adate = array[13];
                if (select(userid, adate, sqlhelper))
                {
                    return string.Empty;
                }
                str.Append(
                    @"insert into uba_useraction (ID,userID,CreateTime,sex,grade,sta,kb,lev,rtm,sch,frs,qq,mobile,mdou) VALUES (");
                str.Append("'" + id + "',");
                str.Append("" + Convert.ToInt32(userid) + ",");
                str.Append("'" + adate + "',");
                str.Append("'" + sex + "',");
                str.Append("'" + grade + "',");
                str.Append("'" + sta + "',");
                str.Append("'" + kb + "',");
                str.Append("'" + ulev + "',");
                str.Append("'" + rtm + "',");
                str.Append("'" + sch + "',");
                str.Append("'" + frs + "',");
                str.Append("'" + qq + "',");
                str.Append("'" + mobile + "',");
                //str.Append("'" + atime + "',");
                str.Append("'" + mdou + "'");
                str.Append(");");
            }
            return str.ToString();
        }

        /// <summary>
        /// 插入用户信息
        /// </summary>
        /// <param name="stream"></param>
        public static void InsertSql(object stream)
        {
            Update(stream);
        }

        /// <summary>
        /// 更新普通action数据
        /// </summary>
        /// <param name="stream"></param>
        public static void UpdateSql(object stream)
        {
            Update(stream, ActionType.update);
        }

        /// <summary>
        /// 更新定制化练习，个人练习数据
        /// </summary>
        /// <param name="stream"></param>
        public static void PracticeSql(object stream)
        {
            Update(stream, ActionType.practice);
        }

        /// <summary>
        /// 更新好友Pk，全球pk数据
        /// </summary>
        /// <param name="stream"></param>
        public static void PkSql(object stream)
        {
            var splitor = "$$$$$$$$$$$$$$$$$$$";
            //Update(stream, ActionType.practice);
            using (StreamReader sr = new StreamReader((Stream)stream, Encoding.UTF8))
            {
                var msg = "开始处理job返回数据，更新数据库";
                Console.WriteLine(msg);
                UBA.Common.LogHelperNet.Info(msg, null);
                Console.WriteLine();
                var result = string.Empty;
                MySqlHelper sqlhelper = new MySqlHelper();
                sqlhelper.getConnection();
                sqlhelper.createTransaction();
                int count = 0;
                sr.ReadLine();
                //处理pk
                do
                {
                    string sql = string.Empty;
                    result = sr.ReadLine();
                    sql = CreateInsertSqlPK(result, sqlhelper);
                    if (!string.IsNullOrEmpty(sql))
                    {
                        sqlhelper.executeUpdate(sql);
                        sqlhelper.commitTransactionKeepCon();
                        sqlhelper.createTransaction();
                    }
                    count++;
                } while (!string.IsNullOrEmpty((result)) && result.Equals(splitor));
                //处理全球pk机器人
                do
                {
                    string sql = string.Empty;
                    result = sr.ReadLine();
                    sql = CreateInsertSqlPKRobot(result, sqlhelper);
                    if (!string.IsNullOrEmpty(sql))
                    {
                        sqlhelper.executeUpdate(sql);
                        sqlhelper.commitTransactionKeepCon();
                        sqlhelper.createTransaction();
                    }
                    count++;
                } while (!string.IsNullOrEmpty((result)) && result.Equals(splitor));

                sqlhelper.commitTransaction();
                msg = string.Format("数据库更新成功，总条数{0}", count);
                Console.WriteLine(msg);
                UBA.Common.LogHelperNet.Info(msg, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream">job返回流</param>
        /// <param name="op">操作</param>
        private static void Update(object stream, ActionType op = ActionType.insert)
        {
            using (StreamReader sr = new StreamReader((Stream)stream, Encoding.UTF8))
            {
                var msg = "开始处理job返回数据，更新数据库";
                Console.WriteLine(msg);
                UBA.Common.LogHelperNet.Info(msg, null);
                Console.WriteLine();
                var result = string.Empty;
                MySqlHelper sqlhelper = new MySqlHelper();
                sqlhelper.getConnection();
                sqlhelper.createTransaction();
                int count = 0;
                sr.ReadLine();
                while (!string.IsNullOrEmpty((result = sr.ReadLine())))
                {
                    string sql = string.Empty;
                    switch (op)
                    {
                        case ActionType.insert:
                            {
                                sql = CreateInsertSql(result, sqlhelper);
                            }
                            break;
                        case ActionType.update:
                            {
                                sql = CreateUpdateSql(result, sqlhelper);
                            }
                            break;
                        case ActionType.practice:
                            {
                                sql = CreateUpdateSqlPractice(result, sqlhelper);
                            }
                            break;
                        case ActionType.pk:
                            break;
                        default:
                            break;
                    }

                    if (!string.IsNullOrEmpty(sql))
                    {
                        sqlhelper.executeUpdate(sql);
                        sqlhelper.commitTransactionKeepCon();
                        sqlhelper.createTransaction();
                    }
                    count++;
                }
                sqlhelper.commitTransaction();
                msg = string.Format("数据库更新成功，总条数{0}", count);
                Console.WriteLine(msg);
                UBA.Common.LogHelperNet.Info(msg, null);
            }
        }


        public static string CreateUpdateSql(string jobResult, MySqlHelper sqlhelper)
        {
            StringBuilder str = new StringBuilder();

            if (!jobResult.StartsWith("Moved"))
            {
                var array = jobResult.Split('\t');
                var userid = array[0];
                var action = array[1];
                if (userid == "60009252")
                {
                    var sss = 1;
                }
                if (action == "022")
                {
                    action = "a022";
                }
                var actionValue = array[2];
                var date = array[3];
                var oldValue = GetActionValue(action, userid, date, sqlhelper);
                var actCount = string.IsNullOrEmpty(actionValue) ? oldValue : int.Parse(actionValue) + oldValue;


                str.AppendFormat("update  uba_useraction set {0}={1} where userid={2} and CreateTime='{3}' ", action,
                    actCount, userid, date);
            }
            return str.ToString();
        }

        public static string CreateUpdateSqlPractice(string jobResult, MySqlHelper sqlhelper)
        {
            StringBuilder str = new StringBuilder();
            if (!jobResult.StartsWith("Moved"))
            {
                //userid bigint,Single string,total int,eount[错题],adate string
                var array = jobResult.Split('\t');
                var userid = array[0];
                var action = array[1];
                var actionValue = array[2];
                var wcount = array[3];
                var date = array[4];
                //计算action次数
                var oldValue = GetActionValue(action, userid, date, sqlhelper);
                var actCount = string.IsNullOrEmpty(actionValue) ? oldValue : int.Parse(actionValue) + oldValue;
                //计算错题数
                var kb = action.Substring(2, 2);
                var wrongColumn = "wp" + kb;
                var wcountOld = GetActionValue(wrongColumn, userid, date, sqlhelper);
                var wcountNew = string.IsNullOrEmpty(wcount) ? wcountOld : int.Parse(wcount) + wcountOld;
                str.AppendFormat("update  uba_useraction set {0}={1},{2}={3} where userid={4} and CreateTime='{5}' ",
                    action, actCount, wrongColumn, wcountNew, userid, date);
            }
            return str.ToString();
        }

        private static string CreateInsertSqlPK(string jobResult, MySqlHelper sqlhelper)
        {
            StringBuilder str = new StringBuilder();
            if (!jobResult.StartsWith("Moved"))
            {
                //userid,actid,memo1[科目],count(1)[行为数量],sum(memo2)[错题数],memo4[胜利失败],adate
                //userid,actid,memo1[科目],count(1)[行为数量],sum(memo2)[错题数],memo3[是否邀请],adate
                var array = jobResult.Split('\t');
                var userid = array[0];
                var action = array[1];
                var kb = array[2];
                var actionValue = array[3];
                var wcount = array[4];
                var success = array[5];
                var date = array[6];
                var actionid = GetActionId(action, kb, success);
                if (string.IsNullOrEmpty(actionid))
                {
                    return string.Empty;
                }
                //计算action次数         
                var oldValue = GetActionValue(actionid, userid, date, sqlhelper);
                var actCount = string.IsNullOrEmpty(actionValue) ? oldValue : int.Parse(actionValue) + oldValue;
                //计算错题数
                var wrongColumn = "wp0" + kb;
                var wcountOld = GetActionValue(wrongColumn, userid, date, sqlhelper);
                var wcountNew = string.IsNullOrEmpty(wcount) ? wcountOld : (int)double.Parse(wcount) + wcountOld;


                str.AppendFormat("update  uba_useraction set {0}={1},{2}={3} where userid={4} and CreateTime='{5}' ",
                    actionid, actCount, wrongColumn, wcountNew, userid, date);
            }
            return str.ToString();
        }

        /// <summary>
        /// 机器人pk
        /// </summary>
        /// <param name="jobResult"></param>
        /// <param name="sqlhelper"></param>
        /// <returns></returns>
        private static string CreateInsertSqlPKRobot(string jobResult, MySqlHelper sqlhelper)
        {
            //userid,actid,count(1),memo3,memo4,adate
            var array = jobResult.Split('\t');
            var userid = array[0];
            var action = array[1];
            var actionValue = array[2];
            var isRobot = array[3];
            var success = array[4];
            var date = array[5];
            var actionid = string.Empty;
            if (success.Equals("1"))
            {
                actionid = "gp04";
            }
            else
            {
                actionid = "gp05";
            }
            if (string.IsNullOrEmpty(actionid))
            {
                return string.Empty;
            }
            //计算action次数         
            var oldValue = GetActionValue(actionid, userid, date, sqlhelper);
            var actCount = string.IsNullOrEmpty(actionValue) ? oldValue : int.Parse(actionValue) + oldValue;

            StringBuilder str = new StringBuilder();
            if (!jobResult.StartsWith("Moved"))
            {
                str.AppendFormat("update  uba_useraction set {0}={1} where userid='{2}' and CreateTime='{3}' ", action,
                    actCount, userid, date);
            }
            return str.ToString();
        }

        private static string GetActionId(string action, string kb, string success)
        {
            var actionid = string.Empty;
            //推算actionID
            if (action.Contains("gpk"))
            {
                switch (kb)
                {
                    case "1":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "gp081";
                            }
                            else
                            {
                                actionid = "gp082";
                            }
                        }
                        break;
                    case "2":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "gp091";
                            }
                            else
                            {
                                actionid = "gp092";
                            }
                        }
                        break;
                    case "3":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "gp101";
                            }
                            else
                            {
                                actionid = "gp102";
                            }
                        }
                        break;
                    case "4":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "gp111";
                            }
                            else
                            {
                                actionid = "gp112";
                            }
                        }
                        break;
                    case "5":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "gp121";
                            }
                            else
                            {
                                actionid = "gp122";
                            }
                        }
                        break;
                    case "6":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "gp131";
                            }
                            else
                            {
                                actionid = "gp132";
                            }
                        }
                        break;
                    case "7":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "gp141";
                            }
                            else
                            {
                                actionid = "gp142";
                            }
                        }
                        break;
                    case "8":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "gp151";
                            }
                            else
                            {
                                actionid = "gp152";
                            }
                        }
                        break;
                    case "9":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "gp161";
                            }
                            else
                            {
                                actionid = "gp162";
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            if (action.Contains("fpk"))
            {
                switch (kb)
                {
                    case "1":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "fp041";
                            }
                            else
                            {
                                actionid = "fp042";
                            }
                        }
                        break;
                    case "2":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "fp051";
                            }
                            else
                            {
                                actionid = "fp052";
                            }
                        }
                        break;
                    case "3":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "fp061";
                            }
                            else
                            {
                                actionid = "fp062";
                            }
                        }
                        break;
                    case "4":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "fp071";
                            }
                            else
                            {
                                actionid = "fp072";
                            }
                        }
                        break;
                    case "5":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "fp081";
                            }
                            else
                            {
                                actionid = "fp082";
                            }
                        }
                        break;
                    case "6":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "fp091";
                            }
                            else
                            {
                                actionid = "fp092";
                            }
                        }
                        break;
                    case "7":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "fp101";
                            }
                            else
                            {
                                actionid = "fp102";
                            }
                        }
                        break;
                    case "8":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "fp111";
                            }
                            else
                            {
                                actionid = "fp112";
                            }
                        }
                        break;
                    case "9":
                        {
                            if (success.Equals("1"))
                            {
                                actionid = "fp121";
                            }
                            else
                            {
                                actionid = "fp122";
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return actionid;
        }

        public static int GetActionValue(string column, string userid, string cratetime, MySqlHelper sqlhelper)
        {
            var sql = string.Format("select {0} from uba_useraction where userid='{1}' and CreateTime='{2}' ", column,
                userid, cratetime);
            var tab = sqlhelper.executeQuery(sql);
            if (tab != null && tab.Rows.Count > 0)
            {
                var value = tab.Rows[0][0].ToString();
                return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
            }
            return 0;
        }

        public static bool select(string userid, string time, MySqlHelper sqlhelper)
        {
            //var timestmp = Int64.Parse(time);
            string sql = string.Format("select 1 from UBA_USERACTION where  userID ='{0}' and CreateTime='{1}'", userid,
                time);
            var table = sqlhelper.executeQuery(sql);
            if (table != null & table.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
    }

    public enum ActionType : int
    {
        insert = 0,
        update = 1,
        practice = 2,
        pk = 3
    }

    public enum KBEnum : int
    {
        语文 = 1,
        数学 = 2,
        英语 = 3,
        物理 = 4,
        化学 = 5,
        地理 = 6,
        历史 = 7,
        政治 = 8,
        生物 = 9,
    }
}