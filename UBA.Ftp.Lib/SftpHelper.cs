using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamir.SharpSsh.jsch;
using Tamir.SharpSsh;
using Tamir.Streams;
using System.IO;
using Org.Mentalis.Security.Cryptography;
namespace UBA.Ftp.Lib
{
    public sealed class SftpHelper : ISftpHelper
    {
        private Session m_session;
        private Channel m_channel;
        private ChannelSftp m_sftp;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="host"></param>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        public SftpHelper(string host, string user, string pwd)
        {
            string[] arr = host.Split(':');
            string ip = arr[0];
            int port = 22;
            if (arr.Length > 1) port = Int32.Parse(arr[1]);
            JSch jsch = new JSch();
            m_session = jsch.getSession(user, ip, port);
            MyUserInfo ui = new MyUserInfo();
            ui.setPassword(pwd);
            m_session.setUserInfo(ui);

        }
        /// <summary>
        /// 判断是否连接
        /// </summary>
        public bool Connected { get { return m_session.isConnected(); } }
        //连接SFTP  
        public bool Connect()
        {
            try
            {
                if (!Connected)
                {
                    m_session.connect();
                    m_channel = m_session.openChannel("sftp");
                    m_channel.connect();
                    m_sftp = (ChannelSftp)m_channel;

                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("host:" + m_session.getHost() + "port:" + m_session.getPort());
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message + "\r\n" + "host:" + m_session.getHost() + "port:" + m_session.getPort());
                //return false;
            }
        }

        //断开SFTP          
        public bool Disconnect()
        {
            bool result = false;
            if (Connected)
            {
                m_channel.disconnect();
                m_channel = null;
                m_session.disconnect();
                m_session = null;
                result = true;
            }
            return result;
        }

        //SFTP存放文件          
        public bool Put(string localPath, string remotePath)
        {
            try
            {
                Tamir.SharpSsh.java.String src = new Tamir.SharpSsh.java.String(localPath);
                Tamir.SharpSsh.java.String dst = new Tamir.SharpSsh.java.String(remotePath);
                m_sftp.put(src, dst);

                return true;
            }
            catch
            {
                return false;
            }
        }

        //public bool Mkdir(string dirName)
        //{
        //    m_sftp.cd("/");
        //    m_sftp.mkdir(new Tamir.SharpSsh.java.String(dirName));
        //    return true;
        //}
        public bool Mkdir(string baseFolder, string dirName)
        {
            try
            {
                m_sftp.cd(baseFolder);

                m_sftp.mkdir(new Tamir.SharpSsh.java.String(dirName));
                return true;
            }
            catch
            {
                return false;
            }

        }

        private readonly string defRemotePath = "/";
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="dirName">目录名称必须从根开始</param>
        /// <returns></returns>
        public bool Mkdir(string dirName)
        {
            var pathArray = dirName.Split('/').ToList().Where(m => !string.IsNullOrEmpty(m)).ToArray();
            for (int k = 0; k < pathArray.Length; k++)
            {
                var dir = pathArray[k];
                if (!string.IsNullOrEmpty(dir))
                {
                    var currdir = GetBaseDir(dirName, k);
                    m_sftp.cd(currdir);
                    if (!DirExist(dir))
                    {
                        m_sftp.mkdir(dir);
                    }
                }
            }
            return true;
        }

        public string GetBaseDir(string dirName, int k)
        {
            var pathArray = dirName.Split('/').ToList().Where(m => !string.IsNullOrEmpty(m)).ToArray();
            if (k > pathArray.Length)
            {
                k = pathArray.Length - 1;
            }
            var path = string.Empty;
            if (k <= 0)
            {
                return defRemotePath;
            }
            else
            {
                path += GetBaseDir(dirName, k - 1) + pathArray[k - 1] + "/";
                return path;
            }

        }


        /// <summary>
        /// 目录是否存在
        /// </summary>
        /// <param name="dirName">目录名称必须从根开始</param>
        /// <returns></returns>
        public bool DirExist(string dirName)
        {
            try
            {
                m_sftp.ls(dirName);
                return true;
            }
            catch (Tamir.SharpSsh.jsch.SftpException)
            {
                return false;//执行ls命令时出错，则目录不存在。
            }
        }
        public class MyUserInfo : UserInfo
        {
            String passwd;
            public String getPassword() { return passwd; }
            public void setPassword(String passwd) { this.passwd = passwd; }

            public String getPassphrase() { return null; }
            public bool promptPassphrase(String message) { return true; }

            public bool promptPassword(String message) { return true; }
            public bool promptYesNo(String message) { return true; }
            public void showMessage(String message) { }
        }










        public string DefPath
        {
            get
            {
                return defRemotePath;
            }
        }
    }
}
