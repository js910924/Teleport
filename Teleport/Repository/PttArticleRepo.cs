using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Teleport.Entities;

namespace Teleport.Repository
{
    public class PttArticleRepo : IPttArticleRepo
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string DirPath = "/Database/PttArticles/";

        public PttArticleRepo(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<PttArticle>> GetAllArticlesTitle(string board)
        {
            var directoryPath = $"{_webHostEnvironment.ContentRootPath}{DirPath}{board}/";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var pttArticles = new List<PttArticle>();
            foreach (var articlePath in Directory.EnumerateFiles(directoryPath))
            {
                var json = await File.ReadAllTextAsync(articlePath);
                var article = JsonConvert.DeserializeObject<PttArticle>(json);
                pttArticles.Add(article);
            }

            return pttArticles;
        }

        public async Task Insert(PttArticle article)
        {
            var json = JsonConvert.SerializeObject(article);

            var filePath = $"{_webHostEnvironment.ContentRootPath}{DirPath}Stock/{article.Title}.json";
            if (!File.Exists(filePath))
            {
                await File.WriteAllTextAsync(filePath, json);
            }
        }
    }
}