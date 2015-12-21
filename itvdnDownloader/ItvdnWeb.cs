using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        const string RequestVerificationToken = "__RequestVerificationToken";
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


        public async Task<bool> Auth2(string login, string password)
        {
            var html = await webClient.DownloadStringTaskAsync(AuthUrl);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            // get __RequestVerificationToken
            var selector = "//section[@class='login-section']//form";
            var input = doc.DocumentNode.SelectSingleNode(selector)?.NextSibling; // wtf (input is not inside form)
            if (input?.Attributes["name"]?.Value != RequestVerificationToken)
            {
                return false;
            }

            var requestVerificationToken = input.Attributes["value"].Value;

            var authData = new NameValueCollection
            {
                {"Email", login},
                {"Password", password},
                {RequestVerificationToken, requestVerificationToken}
            };

            var responseBytes = await webClient.UploadValuesTaskAsync(AuthUrl, "POST", authData);
            var response = Encoding.UTF8.GetString(responseBytes);

            doc.LoadHtml(response);
            var userEmailNode = doc.DocumentNode.SelectSingleNode("//div[@class='user-credentials']/span");
            return userEmailNode?.InnerText == login;
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
