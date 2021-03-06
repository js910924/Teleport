using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace Teleport
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await SetUpScheduler();

            await CreateHostBuilder(args).Build().RunAsync();
        }

        private static async Task SetUpScheduler()
        {
            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler();

            await scheduler.Start();

            var job = JobBuilder.Create<FetchStockTickerJob>()
                .WithIdentity("job1", "group1")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(60)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
