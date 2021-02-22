using System.Collections.Generic;
using System.Threading.Tasks;
using Teleport.Entities;

namespace Teleport.Services.Interfaces
{
    public interface IPttService
    {
        Task<string> CrawlPtt(string pageLink);
        string GetPreviousPage(string html);
        IEnumerable<PttArticle> GetArticles(string html);
    }
}