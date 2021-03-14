using System.Threading.Tasks;

namespace Teleport.Services
{
    public interface ITelegramService
    {
        Task<string> SendMessage(string message);
    }
}