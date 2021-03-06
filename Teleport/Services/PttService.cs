using System;
using System.Collections.Generic;
using System.Linq;
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

        private async Task<string> CrawlPtt(string pageLink)
        {
            var httpResponseMessage = await _httpClient.GetAsync(pageLink);
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }

        private static IEnumerable<PttArticle> GetArticles(string html)
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

        private static string GetPreviousPage(string html)
        {
            var regex = new Regex($"<a class=\"btn wide\" href=\"(.*)\">.*上頁</a>");
            var match = regex.Match(html);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            throw new Exception($"Fail to get previous page, html = {html}");
        }

        public async Task<IEnumerable<PttArticle>> CrawlTargetArticleLinks(string board, string titleElement, int pageAmount)
        {
            var currentPageHtml = await CrawlPtt($"/bbs/{board}/index.html");
            var targetArticles =
                GetArticles(currentPageHtml)
                    .Where(article => article.Title.Contains(titleElement));

            for (var i = 1; i < pageAmount; i++)
            {
                var previousPageLink = GetPreviousPage(currentPageHtml);
                currentPageHtml = await CrawlPtt(previousPageLink);
                var articles =
                    GetArticles(currentPageHtml)
                        .Where(article => article.Title.Contains(titleElement));

                targetArticles = targetArticles.Concat(articles);
            }

            return targetArticles;
        }
    }
}