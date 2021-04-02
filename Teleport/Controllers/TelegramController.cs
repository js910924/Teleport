using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Teleport.Repository;
using Teleport.Services;

namespace Teleport.Controllers
{
    public class TelegramController : Controller
    {
        private readonly ITelegramService _telegramService;
        private readonly IPttService _pttService;
        private readonly IPttArticleRepo _pttArticleRepo;

        public TelegramController(ITelegramService telegramService, IPttService pttService, IPttArticleRepo pttArticleRepo)
        {
            _telegramService = telegramService;
            _pttService = pttService;
            _pttArticleRepo = pttArticleRepo;
        }

        [HttpGet]
        public async Task<IActionResult> SendMessage(string message)
        {
            var response = await _telegramService.SendMessage(message);

            return Content(response);
        }

        public async Task Index()
        {
            await CrawlPtt("Stock", "標的", 1);
        }

        public async Task<OkResult> CrawlPtt(string board, string titleElement, int pageAmount)
        {
            var targetArticles = await _pttService.CrawlTargetArticleLinks(board, titleElement, pageAmount);
            var pttArticlesInDatabase = await _pttArticleRepo.GetAllArticlesTitle(board);
            var latestArticles = targetArticles.Where(article =>
                pttArticlesInDatabase.All(x => x.Title != article.Title));

            var tasks = Enumerable.Empty<Task>().ToList();
            foreach (var article in latestArticles)
            {
                tasks.Add(_telegramService.SendMessage(article.ToPttLink()));
                tasks.Add(_pttArticleRepo.Insert(article));
            }

            await Task.WhenAll(tasks);

            return Ok();
        }
    }
}
