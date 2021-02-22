using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Teleport.Services.Interfaces;

namespace Teleport.Controllers
{
    public class TelegramController : Controller
    {
        private readonly ITelegramService _telegramService;
        private readonly IPttService _pttService;

        public TelegramController(ITelegramService telegramService, IPttService pttService)
        {
            _telegramService = telegramService;
            _pttService = pttService;
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

        public async Task CrawlPtt(string board, string titleElement, int pageAmount)
        {
            var currentPageHtml = await _pttService.CrawlPtt($"/bbs/{board}/index.html");
            var articles = _pttService.GetArticles(currentPageHtml, titleElement);

            for (var i = 1; i < pageAmount; i++)
            {
                var previousPageLink = _pttService.GetPreviousPage(currentPageHtml);
                currentPageHtml = await _pttService.CrawlPtt(previousPageLink);

                articles = articles.Concat(_pttService.GetArticles(currentPageHtml, titleElement));
            }

            foreach (var article in articles)
            {
                await _telegramService.SendMessage(article.ToPttLink());
            }
        }
    }
}
