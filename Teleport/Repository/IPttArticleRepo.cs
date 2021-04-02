using System.Collections.Generic;
using System.Threading.Tasks;
using Teleport.Entities;

namespace Teleport.Repository
{
    public interface IPttArticleRepo
    {
        Task<IEnumerable<PttArticle>> GetAllArticlesTitle(string board);
        Task Insert(PttArticle article);
    }
}