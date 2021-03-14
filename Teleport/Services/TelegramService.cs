using System.Net.Http;
using System.Threading.Tasks;

namespace Teleport.Services
{
    public class TelegramService : ITelegramService
    {
        private const string Token = "1571464997:AAHP0um5Vlk3kpIxbwgO2K0p20Jb7rbrZgE";
        private readonly HttpClient _httpClient;

        public TelegramService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Telegram");
        }

        public async Task<string> SendMessage(string message)
        {
            const string chatId = "-1001369038638";

            var httpResponseMessage = await _httpClient.GetAsync($"/bot{Token}/sendMessage?chat_id={chatId}&text={message}");
            var response = await httpResponseMessage.Content.ReadAsStringAsync();

            return response;
        }
    }
}