using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Teleport.Entities;

namespace Teleport.Repository
{
    public class PttArticleRepo : IPttArticleRepo
    {
        private const string DirectoryPathPrefix = "./Database/PttArticles/";

        public async Task<IEnumerable<PttArticle>> GetAllArticlesTitle(string board)
        {
            var directoryPath = $@"{DirectoryPathPrefix}{board}/";
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

            var filePath = $"{DirectoryPathPrefix}Stock/{article.Title}.json";
            if (!File.Exists(filePath))
            {
                await File.WriteAllTextAsync(filePath, json);
            }
        }
    }
}