using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Fizzler;
using Fizzler.Systems.HtmlAgilityPack;
using Fizzler.Systems.XmlNodeQuery;

namespace CssInline
{
    public static class CssHtmlMerger
    {
        public static string MergeHtmlAndCss(CssParser cssParser, String htmlText)
        {
            HtmlDocument html = new HtmlDocument();
            html.OptionOutputAsXml = true;

            html.LoadHtml(htmlText);
            HtmlNode document = html.DocumentNode;

            foreach (KeyValuePair<String, CssParser.StyleClass> style in cssParser.Styles)
            {
                List<HtmlNode> foundNodes = document.QuerySelectorAll(style.Key).ToList();

                foreach (HtmlNode foundNode in foundNodes)
                {
                    if (foundNode.Attributes["style"] == null)
                    {
                        foundNode.SetAttributeValue("style", style.Value.ToString());
                    }
                    else
                    {
                        foundNode.SetAttributeValue("style", foundNode.Attributes["style"].Value + ";" + style.Value.ToString());
                    }
                }
            }
            
            var tableList = document.QuerySelectorAll(".MathJye");
            foreach (var item in tableList)
            {
                if (item.HasChildNodes)
                {
                    var tb = item.FirstChild;
                    var math = @"\(\frac{{0}}{{1}}\)";
                    var arr = new List<string>();
                    foreach (var tr in tb.ChildNodes)
                    {
                        arr.Add(tr.FirstChild.InnerText);

                    }
                    if (arr.Count > 1)
                    {
                        math = string.Format(math, arr[0], arr[1]);
                        item.ParentNode.ReplaceChild(HtmlNode.CreateNode(math), item); ;
                    }
                }
            }
            htmlText = html.DocumentNode.OuterHtml;
            htmlText = htmlText.Replace("<?xml version=\"1.0\" encoding=\"gb2312\"?>", "");
            htmlText = htmlText.Replace(@"<?xml version=""1.0"" encoding=""iso-8859-1""?>", "").Replace("&amp;", "&");
            htmlText = htmlText.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "").Replace("&amp;", "&");
            return htmlText;
        }
    }
}
