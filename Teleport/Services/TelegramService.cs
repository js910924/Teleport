using System;
using System.Net.Http;
using System.Threading.Tasks;
using Teleport.Services.Interfaces;

namespace Teleport.Services
{
    public class TelegramService : ITelegramService
    {
        private const string Token = "1571464997:AAHP0um5Vlk3kpIxbwgO2K0p20Jb7rbrZgE";
        private const string BaseAddress = "https://api.telegram.org";

        public async Task<string> SendMessage(string message)
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(BaseAddress)
            };

            const string chatId = "-1001369038638";

            var httpResponseMessage = await httpClient.GetAsync($"/bot{Token}/sendMessage?chat_id={chatId}&text={message}");
            var response = await httpResponseMessage.Content.ReadAsStringAsync();

            return response;
        }
    }
}