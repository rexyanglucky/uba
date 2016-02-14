using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace UBA.FileWatch.Lib
{
    public class FileWatchHelper:IFileWatchHelper
    {
        public FileWatchHelper(string path)
        {
            FileSystemWatcher watch = new FileSystemWatcher(path, "*.txt");
           
            watch.Changed += OnChanged;
            watch.Created += OnChanged;
        }
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
        }  
    }
}
