using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UBA.Ftp.Lib
{
    public interface ISftpHelper
    {

        /// <summary>  
        /// 连接服务器  
        /// </summary>  
        /// <returns>true：成功；false：失败</returns>  
        bool Connect();
        /// <summary>
        /// SFTP存放文件 
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        bool Put(string localPath, string remotePath);

        bool Mkdir(string remotePath);
        bool Mkdir(string baseFolder, string dirName);
        string GetBaseDir(string dirName, int k);
        bool DirExist(string dirName);
        /// <summary>
        /// 关闭连接
        /// </summary>
        bool Disconnect();
        string DefPath { get; }


    }
}
