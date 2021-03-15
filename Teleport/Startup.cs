using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Teleport.Factory;
using Teleport.Job;
using Teleport.Proxy;
using Teleport.Repository;
using Teleport.Services;

namespace Teleport
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            ConfigureHttpClient(services);

            ConfigureService(services);
            ConfigureRepo(services);
            services.AddTransient<IStockProxy, StockProxy>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
            {
                option.LoginPath = new PathString("/Account/SignIn");
                option.LogoutPath = new PathString("/Account/SignIn");
                option.ExpireTimeSpan = TimeSpan.FromMinutes(60);   // default is 14 days
            });

            //ConfigureSchedulers(services);
        }

        private static void ConfigureService(IServiceCollection services)
        {
            services.AddTransient<ITelegramService, TelegramService>();
            services.AddTransient<IPttService, PttService>();
            services.AddTransient<IStockService, StockService>();
            services.AddTransient<IStockMarketChecker, StockMarketChecker>();
            services.AddTransient<IStockTransactionService, StockTransactionService>();
        }

        private static void ConfigureRepo(IServiceCollection services)
        {
            services.AddTransient<IStockTransactionRepo, StockTransactionRepo>();
            services.AddTransient<IStockInfoRepo, StockInfoRepo>();
        }

        private static void ConfigureSchedulers(IServiceCollection services)
        {
            services.AddTransient<FetchStockTickerJob>();

            var container = services.BuildServiceProvider();

            var jobFactory = new QuartzJobFactory(container);

            // Create a Quartz.NET scheduler
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();

            // Tell the scheduler to use the custom job factory
            scheduler.JobFactory = jobFactory;

            var job = JobBuilder.Create<FetchStockTickerJob>()
                .WithIdentity("job1", "group1")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger);
            scheduler.Start().GetAwaiter().GetResult();
        }

        private static void ConfigureHttpClient(IServiceCollection services)
        {
            services.AddHttpClient("Ptt", c => { c.BaseAddress = new Uri("https://www.ptt.cc"); });
            services.AddHttpClient("Telegram", c => { c.BaseAddress = new Uri("https://api.telegram.org"); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
