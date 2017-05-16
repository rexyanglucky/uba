using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Latex2MathML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace mathjax2word
{
    public class OpenXmlWord
    {

        private WordprocessingDocument _wordDoc;
        public WordprocessingDocument wordDoc { get { return _wordDoc; } }
        public OpenXmlWord(string filePath)
        {
            if (!File.Exists(filePath))
            {
                CreateOpenXMLFile(filePath);
            }
            this._wordDoc =
                      WordprocessingDocument.Open(filePath, true);

        }

        public void CreateOpenXMLFile(string filePath, bool IsAddHead = false)
        {
            using (WordprocessingDocument objWordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart objMainDocumentPart = objWordDocument.AddMainDocumentPart();
                objMainDocumentPart.Document = new Document(new Body());
                Body objBody = objMainDocumentPart.Document.Body;
                //创建一些需要用到的样式,如标题3,标题4,在OpenXml里面,这些样式都要自己来创建的 
                //ReportExport.CreateParagraphStyle(objWordDocument);
                SectionProperties sectionProperties = new SectionProperties();
                PageSize pageSize = new PageSize();
                PageMargin pageMargin = new PageMargin();
                Columns columns = new Columns() { Space = "220" };//720
                DocGrid docGrid = new DocGrid() { LinePitch = 100 };//360
                //创建页面的大小,页距,页面方向一些基本的设置,如A4,B4,Letter, 
                //GetPageSetting(PageSize,PageMargin);

                //在这里填充各个Paragraph,与Table,页面上第一级元素就是段落,表格.
                objBody.Append(new Paragraph());
                objBody.Append(new Table());
                objBody.Append(new Paragraph());

                //我会告诉你这里的顺序很重要吗?下面才是把上面那些设置放到Word里去.(大家可以试试把这下面的代码放上面,会不会出现打开openxml文件有误,因为内容有误)
                sectionProperties.Append(pageSize, pageMargin, columns, docGrid);
                objBody.Append(sectionProperties);

                //如果有页眉,在这里添加页眉.
                if (IsAddHead)
                {
                    //添加页面,如果有图片,这个图片和上面添加在objBody方式有点不一样,这里搞了好久.
                    //ReportExport.AddHeader(objMainDocumentPart, image);
                }
                objMainDocumentPart.Document.Save();
            }
        }
        //~OpenXmlWord()
        //{
        //    _wordDoc.Close();
        //    _wordDoc.Dispose();
        //}
        /// <summary>
        /// 关闭文档
        /// </summary>
        public void Close()
        {
            _wordDoc.Close();
            _wordDoc.Dispose();
        }
        public OpenXmlWord()
        {

        }
        #region MyRegion

        /// <summary>将mathml转换为OfficeMathML
        /// </summary>
        /// <param name="mathml">mathml</param>
        /// <returns>OfficeMathML</returns>
        public static string ConvertMathMl2OMML(string mathml)
        {

            XslCompiledTransform xslTransform = new XslCompiledTransform();
            xslTransform.Load("MML2OMML.xsl");

            // Load the file containing your MathML presentation markup.
            //using (XmlReader reader = XmlReader.Create(File.Open("source.xml", FileMode.Open)))
            //{

            using (XmlReader reader = XmlReader.Create(new StringReader(mathml)))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    XmlWriterSettings settings = xslTransform.OutputSettings.Clone();

                    // Configure xml writer to omit xml declaration.
                    settings.ConformanceLevel = ConformanceLevel.Fragment;
                    settings.OmitXmlDeclaration = true;

                    XmlWriter xw = XmlWriter.Create(ms, settings);

                    // Transform our MathML to OfficeMathML
                    xslTransform.Transform(reader, xw);
                    ms.Seek(0, SeekOrigin.Begin);

                    StreamReader sr = new StreamReader(ms, Encoding.UTF8);

                    string officeML = sr.ReadToEnd();

                    Console.Out.WriteLine(officeML);

                    return officeML;
                }
            }
        }


        /// <summary>追加officeMathMl 到指定的word文档
        /// </summary>
        /// <param name="officeMathMl"></param>
        /// <param name="filePath"></param>
        public void WritOfficeMathMLToWord(string officeMathMl)
        {

            using (MemoryStream ms = new MemoryStream())
            {
                // Create a OfficeMath instance from the
                // OfficeMathML xml.
                DocumentFormat.OpenXml.Math.OfficeMath om =
                  new DocumentFormat.OpenXml.Math.OfficeMath(officeMathMl);

                // Add the OfficeMath instance to our 
                // word template.


                DocumentFormat.OpenXml.Wordprocessing.Paragraph par =
                  _wordDoc.MainDocumentPart.Document.Body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().FirstOrDefault();


                foreach (var currentRun in om.Descendants<DocumentFormat.OpenXml.Math.Run>())
                {
                    // Add font information to every run.
                    DocumentFormat.OpenXml.Wordprocessing.RunProperties runProperties2 =
                      new DocumentFormat.OpenXml.Wordprocessing.RunProperties();

                    RunFonts runFonts2 = new RunFonts() { Ascii = "Cambria Math", HighAnsi = "Cambria Math" };
                    runProperties2.Append(runFonts2);

                    currentRun.InsertAt(runProperties2, 0);
                }

                par.Append(om);



            }
        }
        /// <summary>追加officeMathMl 到指定的word文档
        /// </summary>
        /// <param name="officeMathMl"></param>
        /// <param name="filePath"></param>
        public void WritOfficeMathMLToWord(string officeMathMl, Run par)
        {

            DocumentFormat.OpenXml.Math.OfficeMath om =
              new DocumentFormat.OpenXml.Math.OfficeMath(officeMathMl);

            foreach (var currentRun in om.Descendants<DocumentFormat.OpenXml.Math.Run>())
            {
                // Add font information to every run.
                DocumentFormat.OpenXml.Wordprocessing.RunProperties runProperties2 =
                  new DocumentFormat.OpenXml.Wordprocessing.RunProperties();

                RunFonts runFonts2 = new RunFonts() { Ascii = "Cambria Math", HighAnsi = "Cambria Math" };
                runProperties2.Append(runFonts2);

                currentRun.InsertAt(runProperties2, 0);
            }
            par.Append(om);


        }
        /// <summary>追加officeMathMl 到指定的word文档
        /// </summary>
        /// <param name="officeMathMl"></param>
        /// <param name="filePath"></param>
        public void WriteTextToWord(string body)
        {
            DocumentFormat.OpenXml.Wordprocessing.Paragraph par =
                 _wordDoc.MainDocumentPart.Document.Body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().FirstOrDefault();
            DocumentFormat.OpenXml.Wordprocessing.Paragraph p = new Paragraph();
            ParagraphProperties propety = new ParagraphProperties();
            Justification justification = new Justification() { Val = JustificationValues.Left };
            propety.Append(justification);
            p.Append(propety);

            var brArray = body.Split(new string[] { "<br>" }, body.Length, StringSplitOptions.None);
            for (int k = 0; k < brArray.Length; k++)
            {

                HandleStr(brArray[k], p);
                var blank = new Run();
                blank.Append(new Break());
                p.Append(blank);


            }
            _wordDoc.MainDocumentPart.Document.Body.Append(p);



        }
        private bool HandleStr(string str, Paragraph p)
        {
            try
            {
                string latexBegin = "\\documentclass{article} \\begin{document}";
                string latexEnd = "\\end{document}";
                var run1 = new Run();
                if (!string.IsNullOrEmpty(str) && (str.IndexOf(@"\(") > 0 || str.IndexOf(@"\)") > 0))
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
                                var omml = ConvertMathMl2OMML(mathxml);
                                run1 = new Run();
                                WritOfficeMathMLToWord(omml, run1);
                                p.Append(run1);

                            }
                            if (mathArray1.Length > 1)
                            {
                                run1 = new Run();
                                run1.Append(new Text(mathArray1[1]));
                                p.Append(run1);
                            }


                        }
                        else
                        {
                            run1 = new Run();
                            run1.Append(new Text(mstr));
                            p.Append(run1);
                        }
                    }

                }
                else
                {
                    run1 = new Run();
                    run1.Append(new Text(str));
                    p.Append(run1);

                }


            }

            catch (Exception ex)
            {
                throw ex;
            }
            return true;



        }


        /// <summary>
        /// 替换书签
        /// </summary>
        /// <param name="bookMarkValueList"></param>
        public void ReplacBookMark(Dictionary<string, DocumentFormat.OpenXml.OpenXmlElement> bookMarkValueList)
        {
            var mainPart = _wordDoc.MainDocumentPart;


            var bookmarks = mainPart.Document.Descendants<BookmarkStart>().ToList();
            foreach (var mark in bookmarks)
            {
                if (bookMarkValueList.ContainsKey(mark.Name))
                {
                    var om = bookMarkValueList[mark.Name];
                    var omclone = (DocumentFormat.OpenXml.Math.OfficeMath)om.Clone();
                    mark.Parent.RemoveAllChildren();
                    mark.Parent.Append(omclone);
                }
            }
        }
        #endregion

    }
}






#region 暂时不用
//class Program
//{
//    static string math = @"<math  xmlns='http://www.w3.org/1998/Math/MathML' display='block'>
//                                  <mstyle id='x1-2r1' class='label'/>
//                                  <msup>
//                                    <mrow>
//                                      <mi>a</mi>
//                                    </mrow>
//                                    <mrow>
//                                      <mn>2</mn>
//                                    </mrow>
//                                  </msup>
//                                  <mo class='MathClass-bin'>+</mo>
//                                  <msup>
//                                    <mrow>
//                                      <mi>b</mi>
//                                    </mrow>
//                                    <mrow>
//                                      <mn>2</mn>
//                                    </mrow>
//                                  </msup>
//                                  <mo class='MathClass-rel'>=</mo>
//                                  <msup>
//                                    <mrow>
//                                      <mi>c</mi>
//                                    </mrow>
//                                    <mrow>
//                                      <mn>2</mn>
//                                    </mrow>
//                                  </msup>
//                                </math>";
//    static void Main(string[] args)
//    {
//        //var fileName = @"E:\math.docx";
//        //WordOp wordop = new WordOp();
//        //try
//        //{
//        //    wordop.OpenAndActive(fileName, false, false);
//        //    var app = wordop.WordApplication;
//        //    var doc = wordop.WordDocument;
//        //    object unitStory = WdUnits.wdStory;
//        //    app.Selection.Move(ref unitStory);
//        //    app.Selection.Text = "The result is: ";
//        //    app.Selection.Move(ref unitStory);
//        //    object fieldType = WdFieldType.wdFieldEmpty;
//        //    object preserveFormating = false;
//        //    doc.Fields.Add(app.Selection.Range, ref fieldType, @"EQ x=\f(1,y)", ref preserveFormating);
//        //}
//        //catch (Exception ex)
//        //{
//        //    Console.WriteLine(ex.Message);
//        //}
//        //finally
//        //{
//        //    wordop.Close();
//        //    Console.WriteLine("关闭文档");
//        //}

//        //Console.ReadLine();


















//        XslCompiledTransform xslTransform = new XslCompiledTransform();

//        // The MML2OMML.xsl file is located under 
//        // %ProgramFiles%\Microsoft Office\Office12\
//        xslTransform.Load("MML2OMML.xsl");

//        // Load the file containing your MathML presentation markup.
//        using (XmlReader reader = XmlReader.Create(File.Open("source.xml", FileMode.Open)))
//        {
//            using (MemoryStream ms = new MemoryStream())
//            {
//                XmlWriterSettings settings = xslTransform.OutputSettings.Clone();

//                // Configure xml writer to omit xml declaration.
//                settings.ConformanceLevel = ConformanceLevel.Fragment;
//                settings.OmitXmlDeclaration = true;

//                XmlWriter xw = XmlWriter.Create(ms, settings);

//                // Transform our MathML to OfficeMathML
//                xslTransform.Transform(reader, xw);
//                ms.Seek(0, SeekOrigin.Begin);

//                StreamReader sr = new StreamReader(ms, Encoding.UTF8);

//                string officeML = sr.ReadToEnd();

//                Console.Out.WriteLine(officeML);

//                // Create a OfficeMath instance from the
//                // OfficeMathML xml.
//                DocumentFormat.OpenXml.Math.OfficeMath om =
//                  new DocumentFormat.OpenXml.Math.OfficeMath(officeML);

//                // Add the OfficeMath instance to our 
//                // word template.
//                using (WordprocessingDocument wordDoc =
//                  WordprocessingDocument.Open("t1.docx", true))
//                {

//                    //DocumentFormat.OpenXml.Wordprocessing.Paragraph par =
//                    //  wordDoc.MainDocumentPart.Document.Body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().FirstOrDefault();


//                    foreach (var currentRun in om.Descendants<DocumentFormat.OpenXml.Math.Run>())
//                    {
//                        // Add font information to every run.
//                        DocumentFormat.OpenXml.Wordprocessing.RunProperties runProperties2 =
//                          new DocumentFormat.OpenXml.Wordprocessing.RunProperties();

//                        RunFonts runFonts2 = new RunFonts() { Ascii = "Cambria Math", HighAnsi = "Cambria Math" };
//                        runProperties2.Append(runFonts2);

//                        currentRun.InsertAt(runProperties2, 0);
//                    }

//                    //par.Append(om);






//                    var mainPart = wordDoc.MainDocumentPart;

//                    /////////////////////////////////////////////////////////////
//                    var bookmarks = mainPart.Document.Descendants<BookmarkStart>().ToList();
//                    foreach (var mark in bookmarks)
//                    {
//                        var omclone = (DocumentFormat.OpenXml.Math.OfficeMath)om.Clone();

//                        mark.Parent.Append(omclone);
//                    }

//                }

//            }
//        }
//        Console.ReadLine();
//    }



//} 
#endregion