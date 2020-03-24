using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wox.Plugin;

namespace Wox.CNKI.Translator
{
    public static class Translator
    {
        public static List<Result> Translate(string q, Action<string> toast)
        {
            List<Result> results = new List<Result>();
            const string ico = "Images\\youdao.ico";
            if (string.IsNullOrWhiteSpace(q))
            {
                results.Add(new Result
                {
                    Title = "开始CNKI中英互译",
                    SubTitle = "基于CNKI网页 API",
                    IcoPath = ico
                });
                return results;
            }
            using (var httpClient = new HttpClient())
            {
                var body = new FormUrlEncodedContent(new Dictionary<string, string>() { { "searchword", q }, { "txt2", q }, { "saveID", q }, });
                var res = httpClient.PostAsync("http://dict.cnki.net/dict_result.aspx", body).Result;
                var html = res.Content.ReadAsStringAsync().Result;

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);
                var fonts = doc.DocumentNode.SelectNodes(@"//div[@class=""zztj""]/ul/li/font");

                foreach (var font in fonts)
                {
                    var r = font.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element).Select(c => c.InnerText).ToList();
                    var translation = r[0];
                    var count = r[1];
                    results.Add(new Result
                    {
                        Title = translation,
                        SubTitle = count,
                        IcoPath = ico,
                        Action = ctx =>
                        {
                            try
                            {
                                Clipboard.SetText(translation);
                                toast("翻译已被存入剪贴板");
                            }
                            catch (Exception e)
                            {
                                toast("剪贴板打开失败，请稍后再试");
                                return false;
                            }
                            return true;
                        },
                    });
                }
            }
            return results;
        }

        public static HtmlNode NextSiblingOfType(this HtmlNode node, HtmlNodeType nodeType)
        {
            var next = node.NextSibling;
            while (next != null && next.NodeType != nodeType)
            {
                next = next.NextSibling;
            }
            return next;
        }
    }
}
