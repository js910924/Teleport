using System.Collections.Generic;
using System.Threading.Tasks;
using Teleport.Models;

namespace Teleport.Services
{
    public interface IStockService
    {
        Task<IEnumerable<StockPosition>> GetStockPositionsBy(int customerId);
    }
}