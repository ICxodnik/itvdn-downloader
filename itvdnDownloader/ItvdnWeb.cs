using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WB = System.Windows.Forms.WebBrowser;

namespace itvdnDownloader
{
    class ItvdnWeb
    {
        const string AuthUrl = "http://itvdn.com/ru/Account/Login";
        const string AuthCompletedUrl = "http://itvdn.com/ru";
        readonly WB browser = new WB();
        readonly WebClient webClient = new CookieAwareWebClient();
        private AuthContext context;
        private Action onAuth;

        public ItvdnWeb()
        {
            browser.ScriptErrorsSuppressed = true;
            browser.DocumentCompleted += Browser_DocumentCompleted;
            
        }

        public async Task<IEnumerable<LessonData>> GetLessons(string url)
        {
            //browser.Navigate()


            //var doc = new HtmlDocument();
            //doc.LoadHtml(html);







            return null;
        }


        public async void Auth2(string login, string password)
        {
            var html = await webClient.DownloadStringTaskAsync("http://itvdn.com/ru/Account/Login");
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            // todo
            doc.DocumentNode.SelectSingleNode("section[class='login-section']//form/input[name='__RequestVerificationToken']");


        }

        public void Auth(AuthContext authContext, Action authCallback)
        {
            context = authContext;
            onAuth = authCallback;
            browser.Navigate(AuthUrl);
        }

        private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var uri = e.Url.AbsoluteUri;
            switch (uri)
            {
                case AuthUrl:
                    SetInputValue("Email", context.Login);
                    SetInputValue("Password", context.Password);
                    SubmitForm();
                    break;
                case AuthCompletedUrl:
                    onAuth();
                    break;
            }
        }

        private void SetInputValue(string inputId, string value)
        {
            var field = browser.Document.GetElementById(inputId);
            if (field != null)
            {
                field.SetAttribute("value", value);
            }
        }

        private void SubmitForm()
        {
            var form = browser.Document.Forms[0];
            if (form != null)
            {
                form.InvokeMember("submit");
            }
        }

    }
}
