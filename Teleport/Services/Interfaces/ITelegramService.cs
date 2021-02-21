using System.Threading.Tasks;

namespace Teleport.Services.Interfaces
{
    public interface ITelegramService
    {
        Task<string> SendMessage(string message);
    }
}