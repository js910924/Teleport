using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Teleport.Services.Interfaces;

namespace Teleport.Controllers
{
    public class TelegramController : Controller
    {
        private readonly ITelegramService _telegramService;

        public TelegramController(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        [HttpGet]
        public async Task<IActionResult> SendMessage(string message)
        {
            var response = await _telegramService.SendMessage(message);

            return Content(response);
        }
    }
}
