using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace itvdnDownloader
{
    class ItvdnWeb
    {
        public const string AuthUrl = "http://itvdn.com/ru/Account/Login";
        private const string RequestVerificationToken = "__RequestVerificationToken";
        private readonly WebClient webClient = new CookieAwareWebClient();


        public async Task<IEnumerable<LessonData>> GetLessons(string url)
        {
            var html = await webClient.DownloadStringTaskAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var select = "//div[@id='lessons-list']/a[@class='media']"; // XPath
            var lessons = doc.DocumentNode.SelectNodes(select);
            return lessons.Select(href => new LessonData
            {
                Url = href.Attributes["href"].Value,
                Title = href.SelectSingleNode("div[@class = 'media-body']/p")?.InnerText,
                Number = href.SelectSingleNode("div[@class = 'pull-left']//div[@class = 'media-inner']/span")?.InnerText
            });
        }

        public async Task<bool> Auth(string login, string password)
        {
            var html = await webClient.DownloadStringTaskAsync(AuthUrl);
            var doc = new HtmlDocument();
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

    }
}
