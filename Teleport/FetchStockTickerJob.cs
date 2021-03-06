using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Teleport.Services.Interfaces;

namespace Teleport
{
    public class FetchStockTickerJob : IJob
    {
        private readonly IPttService _pttService;
        private readonly ITelegramService _telegramService;

        public FetchStockTickerJob(IPttService pttService, ITelegramService telegramService)
        {
            _pttService = pttService;
            _telegramService = telegramService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var articleLinks = await _pttService.CrawlTargetArticleLinks("Stock", "標的", 1);

            var tasks = articleLinks
                .Select(link => Task.Run(() => _telegramService.SendMessage(link.ToPttLink())));

            Task.WaitAll(tasks.ToArray());
        }
    }
}