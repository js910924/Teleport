using System.Collections.Generic;
using System.Threading.Tasks;
using Teleport.Entities;

namespace Teleport.Services.Interfaces
{
    public interface IPttService
    {
        Task<string> CrawlPtt(string pageLink);
        IEnumerable<PttArticle> GetArticles(string html, string titleElement);
        string GetPreviousPage(string html);
    }
}