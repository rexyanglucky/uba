using CssInline;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;
using DocumentFormat.OpenXml.Wordprocessing;
using Latex2MathML;
using NotesFor.HtmlToOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenXmlWord
{
    class Program
    {
        static void Main(string[] args)
        {


            CssParser cp = new CssParser();
            cp.AddStyleSheet(@"1_files/quemain.css");
            var html = File.ReadAllText("2.html");
            var htmlText = CssHtmlMerger.MergeHtmlAndCss(cp, html);
            var bodyhtml = htmlText;
            //string body = File.ReadAllText("E:\\N1.txt");
            //mathjax2word.OpenXmlWord word = new mathjax2word.OpenXmlWord("E:\\a4.docx");
            //word.WriteTextToWord(bodyhtml);
            //word.Close();
            //Console.ReadLine();








            #region html 2 openxml
            const string filename = "test.docx";

            if (File.Exists(filename)) File.Delete(filename);

            using (MemoryStream generatedDocument = new MemoryStream())
            {
                // Uncomment and comment the second using() to open an existing template document
                // instead of creating it from scratch.

                byte[] data = Resource1.template;
                generatedDocument.Write(data, 0, data.Length);
                generatedDocument.Position = 0L;
                using (WordprocessingDocument package = WordprocessingDocument.Open(generatedDocument, true))
                //using (WordprocessingDocument package = WordprocessingDocument.Create(generatedDocument, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = package.MainDocumentPart;
                    if (mainPart == null)
                    {
                        mainPart = package.AddMainDocumentPart();
                        new Document(new Body()).Save(mainPart);
                    }

                    HtmlConverter converter = new HtmlConverter(mainPart);
                    //converter.WebProxy.Credentials = new System.Net.NetworkCredential("nizeto", "****", "domain");
                    //converter.WebProxy.Proxy = new System.Net.WebProxy("proxy01:8080");
                    converter.ImageProcessing = ImageProcessing.AutomaticDownload;
                    converter.ProvisionImage += OnProvisionImage;

                    Body body = mainPart.Document.Body;

                    converter.ParseHtml(bodyhtml);
                    mainPart.Document.Save();

                    //AssertThatOpenXmlDocumentIsValid(package);
                }

                File.WriteAllBytes(filename, generatedDocument.ToArray());
            }

            System.Diagnostics.Process.Start(filename);
            #endregion



















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
                            var omml = mathjax2word.OpenXmlWord.ConvertMathMl2OMML(mathxml);
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


        static void OnProvisionImage(object sender, ProvisionImageEventArgs e)
        {
            string filename = Path.GetFileName(e.ImageUrl.OriginalString);
            if (!File.Exists("../../images/" + filename))
            {
                e.Cancel = true;
                return;
            }

            e.Provision(File.ReadAllBytes("../../images/" + filename));
        }

        static void AssertThatOpenXmlDocumentIsValid(WordprocessingDocument wpDoc)
        {
            var validator = new OpenXmlValidator(FileFormatVersions.Office2010);
            var errors = validator.Validate(wpDoc);

            if (!errors.GetEnumerator().MoveNext())
                return;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("The document doesn't look 100% compatible with Office 2010.\n");

            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (ValidationErrorInfo error in errors)
            {
                Console.Write("{0}\n\t{1}", error.Path.XPath, error.Description);
                Console.WriteLine();
            }

            Console.ReadLine();
        }
    }
}