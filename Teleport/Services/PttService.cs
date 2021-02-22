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
        private readonly HttpClient _httpClient;

        public PttService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Ptt");
        }

        public async Task<string> CrawlPtt(string pageLink)
        {
            var httpResponseMessage = await _httpClient.GetAsync(pageLink);
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }

        public IEnumerable<PttArticle> GetArticles(string html)
        {
            var titleRegex = new Regex("<div class=\"title\">([\\s\\S]*?)</div>");
            var authorRegex = new Regex("<div class=\"author\">(.*)</div>");
            var dateRegex = new Regex("<div class=\"date\">(.*)</div>");

            var titleMatch = titleRegex.Match(html);
            var authorMatch = authorRegex.Match(html);
            var dateMatch = dateRegex.Match(html);

            var articles = new List<PttArticle>();

            while (titleMatch.Success)
            {
                var titleDiv = titleMatch.Groups[1].Value;
                var regex = new Regex("<a href=\"(.*)\">(.*)</a>");
                var match = regex.Match(titleDiv);
                var article = new PttArticle()
                {
                    Title = match.Groups[2].Value,
                    Link = match.Groups[1].Value,
                    Author = authorMatch.Groups[1].Value,
                    Date = dateMatch.Groups[1].Value
                };

                articles.Add(article);

                titleMatch = titleMatch.NextMatch();
                authorMatch = authorMatch.NextMatch();
                dateMatch = dateMatch.NextMatch();
            }

            return articles;
        }

        public string GetPreviousPage(string html)
        {
            var regex = new Regex($"<a class=\"btn wide\" href=\"(.*)\">.*上頁</a>");
            var match = regex.Match(html);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            throw new Exception($"Fail to get previous page, html = {html}");
        }
    }
}