using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Teleport.Repository;
using Teleport.Services;

namespace Teleport.Job
{
    public class FetchStockTickerJob : IJob
    {
        private readonly IPttService _pttService;
        private readonly ITelegramService _telegramService;
        private readonly IPttArticleRepo _pttArticleRepo;

        public FetchStockTickerJob(IPttService pttService, ITelegramService telegramService, IPttArticleRepo pttArticleRepo)
        {
            _pttService = pttService;
            _telegramService = telegramService;
            _pttArticleRepo = pttArticleRepo;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var targetArticles = await _pttService.CrawlTargetArticleLinks("Stock", "標的", 1);

            var pttArticlesInDatabase = await _pttArticleRepo.GetAllArticlesTitle("Stock");
            var latestArticles = targetArticles.Where(article =>
                pttArticlesInDatabase.All(x => x.Title != article.Title));

            var tasks = Enumerable.Empty<Task>().ToList();
            foreach (var article in latestArticles)
            {
                tasks.Add(_telegramService.SendMessage(article.ToPttLink()));
                tasks.Add(_pttArticleRepo.Insert(article));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}