using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UBA.Common
{
    public class IOHelper
    {
        public static void ListFiles(string path, ref List<FileInfo> fileList)
        {
            if (!Directory.Exists(path)) return;
            DirectoryInfo dir = new DirectoryInfo(path);
            //不是目录   
            if (dir == null) return;
            FileSystemInfo[] files = dir.GetFileSystemInfos();
            for (int i = 0; i < files.Length; i++)
            {
                FileSystemInfo file = files[i];

                //是文件   
                if ((file.Attributes & FileAttributes.Directory) == 0)
                    fileList.Add((FileInfo)file);
                //对于子目录，进行递归调用   
                else
                    ListFiles(files[i].FullName, ref fileList);
            }
        }
        public static void ListDirectory(string path, ref List<DirectoryInfo> directoryList)
        {
            if (!Directory.Exists(path)) return;
            DirectoryInfo dir = new DirectoryInfo(path);
            //不是目录   
            if (dir == null) return;
            FileSystemInfo[] files = dir.GetFileSystemInfos();
            for (int i = 0; i < files.Length; i++)
            {
                FileSystemInfo file = files[i];

                //是文件   
                if ((file.Attributes & FileAttributes.Directory) == 0)
                { }
                //对于子目录，进行递归调用   
                else
                {
                    directoryList.Add((DirectoryInfo)file);
                    ListDirectory(files[i].FullName, ref directoryList);
                }
            }
        }
    }
}
