using Beanstalk.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

using WebApplication.Services;

namespace WebApplication
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var options = ConfigurationOptions.Parse(configuration.GetValue<string>("Redis:ConnectionString"));
                options.AllowAdmin = true;
                options.AbortOnConnectFail = false;
                options.CommandMap = CommandMap.Default;

                var queueType = configuration?.GetValue<int>("QueueType") ?? 0;

                if (queueType == 0 || queueType == 2) return ConnectionMultiplexer.Connect(options);
                else return new MockConnectionMultiplexer();
            });
            services.AddScoped((serviceProvider) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                return new BeanstalkConnection(configuration.GetValue<string>("Beanstalk:Host"), configuration.GetValue<ushort>("Beanstalk:Port"));
            });
            services.AddSingleton(new StatsService());
            services.AddControllers();
            services.AddHostedService<QueueReaderService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
                endpoints.MapControllers();
            });
        }
    }
}
