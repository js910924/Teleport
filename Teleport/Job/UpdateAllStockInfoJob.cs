using System.Threading.Tasks;
using Quartz;
using Teleport.Proxy;
using Teleport.Repository;

namespace Teleport.Job
{
    public class UpdateAllStockInfoJob : IJob
    {
        private readonly IStockProxy _stockProxy;
        private readonly IStockInfoRepo _stockInfoRepo;

        public UpdateAllStockInfoJob(IStockProxy stockProxy, IStockInfoRepo stockInfoRepo)
        {
            _stockProxy = stockProxy;
            _stockInfoRepo = stockInfoRepo;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var stockInfos = await _stockInfoRepo.GetAllStockInfo();

            foreach (var stockInfo in stockInfos)
            {
                var info = await _stockProxy.GetStockInfo(stockInfo.Symbol);
                await _stockInfoRepo.UpsertStockInfo(info);
            }
        }
    }
}