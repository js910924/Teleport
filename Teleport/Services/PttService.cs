using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Teleport.Entities;
using Teleport.Services.Interfaces;

namespace Teleport.Services
{
    public class PttService : IPttService
    {
        public static string PttUrl = "https://www.ptt.cc";

        public async Task<string> CrawlPtt(string board)
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(PttUrl)
            };

            var httpResponseMessage = await httpClient.GetAsync($"/bbs/{board}/index.html");
            var response = await httpResponseMessage.Content.ReadAsStringAsync();

            return response;
        }

        public IEnumerable<PttArticle> GetArticles(string html, string titleElement)
        {
            var regex = new Regex($"<a href=(\".*\")>(.*{titleElement}.*)</a>");
            var match = regex.Match(html);
            var articles = new List<PttArticle>();

            while (match.Success)
            {
                var article = ToPttArticle(match);

                articles.Add(article);

                match = match.NextMatch();
            }

            return articles;
        }

        private static PttArticle ToPttArticle(Match match)
        {
            var title = match.Groups[2].ToString().Trim('"');
            var link = match.Groups[1].ToString().Trim('"');

            return new PttArticle()
            {
                Title = title,
                Link = link
            };
        }
    }
}