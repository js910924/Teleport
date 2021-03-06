using System.Collections.Generic;
using System.Threading.Tasks;
using Teleport.Entities;

namespace Teleport.Services.Interfaces
{
    public interface IPttService
    {
        Task<IEnumerable<PttArticle>> CrawlTargetArticleLinks(string board, string titleElement, int pageAmount);
    }
}