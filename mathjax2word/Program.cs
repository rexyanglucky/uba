using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using System.Xml.Xsl;
using System.Xml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Latex2MathML;
using System.Text.RegularExpressions;
namespace mathjax2word
{
    class Program
    {
        static void Main(string[] args)
        {

#if MONO
		//LatexToMathMLConverter.GhostScriptBinaryPath = "gs";	
#else
            //LatexToMathMLConverter.GhostScriptBinaryPath = @"C:\\Program Files (x86)\\gs\\gs8.64\\bin\\gswin32c.exe";
#endif
            //var lmm = new LatexToMathMLConverter(
            //    "E:\\source.txt",
            //    Encoding.GetEncoding(1251),
            //   "E:\\source1.xml");



            string latexBegin = "\\documentclass{article} \\begin{document}";
            string latexEnd = "\\end{document}";



           
            string body = @"计算：<br>1.(5\({a}^{2}\)+2a)-4(2+2\({a}^{2}\))<br>2.(\({x}^{2}\)+4)(x+2)(x-2)";
            body = File.ReadAllText("E:\\N1.txt");
            OpenXmlWord word = new OpenXmlWord("E:\\a1.docx");
            word.WriteTextToWord(body);
            word.Close();
            Console.ReadLine();









            #region 1
            // string br = "<w:p><w:r><w:t></w:t></w:r></w:p>";
            // string oxmlText = "<w:p><w:r><w:t>{0}</w:t></w:r></w:p>";


            // var brArray = body.Split(new string[] { "<br>" }, body.Length, StringSplitOptions.None);
            // var strList = new List<string>();
            // for (int k = 0; k < brArray.Length; k++)
            // {
            //     strList.Add(HandleStr(brArray[k]));

            //     strList.Add(br);

            // }

            // var xml = string.Join("", strList);




            // Console.WriteLine(xml);




            // OpenXmlWord word = new OpenXmlWord("E:\\a.docx");

            //// var omml = OpenXmlWord.ConvertMathMl2OMML(xml);
            // word.WriteTextToWord(xml);
            // word.Close();
            // Console.ReadLine(); 
            #endregion



            #region 2
            //var lmm = new LatexToMathMLConverter(
            //   "E:\\source.txt",
            //  Encoding.UTF8,
            //  "E:\\source.xml");
            ////lmm.ValidateResult = true;
            ////lmm.Convert();
            //var str = lmm.ConvertToText();
            //Console.WriteLine(str);


            //string strText = System.Text.RegularExpressions.Regex.Replace(body, "<[^>]+>", "");
            //strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");
            ////Regex.Match(body,)


            //body.Replace(@"\(", "$");




            //OpenXmlWord word = new OpenXmlWord("E:\\a.docx");
            //var omml = OpenXmlWord.ConvertMathMl2OMML(str);
            //word.WritOfficeMathMLToWord(omml);
            //word.Close();
            //Console.ReadLine(); 
            #endregion
        }

        private static string HandleStr(string str)
        {
            string latexBegin = "\\documentclass{article} \\begin{document}";
            string latexEnd = "\\end{document}";
            string oxmlText = "<w:p><w:r><w:t>{0}</w:t></w:r></w:p>";
            StringBuilder result = new StringBuilder("<w:p>");
            if (str.IndexOf(@"\(") > 0 || str.IndexOf(@"\)") > 0)
            {
                var mathArray = str.Split(new string[] { @"\(" }, str.Length, StringSplitOptions.None);
                for (int i = 0; i < mathArray.Length; i++)
                {
                    var mstr = mathArray[i];
                    if (mstr.IndexOf(@"\)") > 0)
                    {
                        var mathArray1 = mstr.Split(new string[] { @"\)" }, mstr.Length, StringSplitOptions.None);
                        if (mathArray1.Length > 0)
                        {
                            var latexxml = latexBegin + "$" + mathArray1[0] + "$" + latexEnd;
                            var lmm = new LatexToMathMLConverter(latexxml);
                            var mathxml = lmm.ConvertToText();
                            var omml = OpenXmlWord.ConvertMathMl2OMML(mathxml);
                            result.Append(omml);
                        }
                        if (mathArray1.Length > 1)
                        {
                            result.AppendFormat("<w:r><w:t>{0}</w:t></w:r>", mathArray1[1]);
                        }


                    }
                    else
                    {
                        result.AppendFormat("<w:r><w:t>{0}</w:t></w:r>", mstr);
                    }
                }

            }
            else
            {
                result.AppendFormat("<w:r><w:t>{0}</w:t></w:r>", str);

            }
            result.Append("</w:p>");


            return result.ToString();



        }
    }
}
