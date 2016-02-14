using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssInline
{
    class Program
    {
        static void Main(string[] args)
        {
            CssParser cp = new CssParser();
            //cp.AddStyleSheet(@"1_files/public.css");
            cp.AddStyleSheet(@"1_files/quemain.css");
            //cp.AddStyleSheet(@"1_files/skin6.css");
            //cp.AddStyleSheet(@"1_files/web.css");


            var html = File.ReadAllText("1.html");
            var htmlText = CssHtmlMerger.MergeHtmlAndCss(cp, html);

            Console.WriteLine(htmlText);
            Console.ReadLine();




        }
    }
}
