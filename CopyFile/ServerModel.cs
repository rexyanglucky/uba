﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyFile
{
    public class ServerModel
    {

        public string ServerHost { get; set; }
        public string ServerUserName { get; set; }
        public string ServerPwd { get; set; }
        public string DestDirectory { get; set; }

        /// <summary>
        /// SFTP WINDOWS
        /// </summary>
        public int Type { get; set; }

        public string ReamrkName { get; set; }

        public string SourceDirectory { get; set; }
        public bool IsVisible { get; set; }

        public int Id { get; set; }
    }
    //public class SourceModel
    //{

    //    public string MyProperty { get; set; }
    //    public string ServerUserName { get; set; }
    //    public string ServerPwd { get; set; }
    //    public string SourceDirectory { get; set; }

    //    /// <summary>
    //    /// SFTP WINDOWS
    //    /// </summary>
    //    public int Type { get; set; }
    //}
}
