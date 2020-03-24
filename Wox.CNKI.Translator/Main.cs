using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using Wox.Plugin;
using System.Net.Http;
using HtmlAgilityPack;
using System.Windows.Forms;

namespace Wox.CNKI.Translator
{
    public class TranslateResult
    {
        public int errorCode { get; set; }
        public List<string> translation { get; set; }
        public BasicTranslation basic { get; set; }
        public List<WebTranslation> web { get; set; }
    }

    // 有道词典-基本词典
    public class BasicTranslation
    {
        public string phonetic { get; set; }
        public List<string> explains { get; set; }
    }

    public class WebTranslation
    {
        public string key { get; set; }
        public List<string> value { get; set; }
    }

    public class Main : IPlugin
    {
        private PluginInitContext _context;
        public void Init(PluginInitContext context)
        {
            _context = context;
        }

        public List<Result> Query(Query query)
        {
            try
            {
                return Translator.Translate(query.Search, msg =>
               {
                   _context.API.ShowMsg(msg);
               });
            }
            catch (Exception e)
            {
                _context.API.ShowMsg("Oops: " + e.Message);
                return Enumerable.Empty<Result>().ToList();
            }
        }

    }
}
