using Beanstalk.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication.Services
{
    public class QueueReaderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public QueueReaderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            using var scope = _serviceProvider.CreateScope();
            var configuration = scope.ServiceProvider.GetService<IConfiguration>();
            var queueType = configuration?.GetValue<int>("QueueType") ?? 0;
            var queueName = configuration.GetValue<string>("QueueName");
            var statsService = scope.ServiceProvider.GetService<StatsService>();

            switch (queueType)
            {
                case 0:
                    return RunRedisQueue(scope.ServiceProvider.GetService<IConnectionMultiplexer>().GetDatabase(0), statsService, queueName, stoppingToken);
                case 1:
                    var beanstalkConnection = new BeanstalkConnection(configuration.GetValue<string>("Beanstalk:Host"), configuration.GetValue<ushort>("Beanstalk:Port"));
                    return RunBeanstalkQueue(beanstalkConnection, statsService, queueName, stoppingToken);
                case 2:
                    return RunRedisStack(scope.ServiceProvider.GetService<IConnectionMultiplexer>().GetDatabase(0), statsService, queueName, stoppingToken);
                default:
                    return Task.CompletedTask;
            }
        }

        private async Task RunRedisStack(IDatabase db, StatsService statsService, string queueName, CancellationToken cancellationToken)
        {
            Console.WriteLine("Start listening queue with name '{0}'", queueName);
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var data = await db.ListRightPopAsync(queueName);
                if (data.HasValue) WriteResultToConsole(long.Parse(data), statsService, cancellationToken);
            }
        }

        private async Task RunRedisQueue(IDatabase db, StatsService statsService, string queueName, CancellationToken cancellationToken)
        {
            Console.WriteLine("Start listening queue with name '{0}'", queueName);
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var data = await db.ListLeftPopAsync(queueName);
                if (data.HasValue) WriteResultToConsole(long.Parse(data), statsService, cancellationToken);
            }
        }

        private async Task RunBeanstalkQueue(BeanstalkConnection beanstalkConnection, StatsService statsService, string queueName, CancellationToken cancellationToken)
        {
            await beanstalkConnection.Watch(queueName);
            Console.WriteLine("Start listening queue with name '{0}'", queueName);
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var data = await beanstalkConnection.Reserve(TimeSpan.FromMinutes(5));
                    WriteResultToConsole(long.Parse(data.Data), statsService, cancellationToken);
                    await beanstalkConnection.Delete(data.Id);
                }
                catch { }
            }
        }

        private void WriteResultToConsole(long timestampFromQueue, StatsService statsService, CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var elapsedTime = timestamp - timestampFromQueue;
                Console.WriteLine("Elapsed time since recording: {0} ms", elapsedTime);
                statsService.AddNumberOfReadMessages();
                statsService.AddElapsedTime(elapsedTime);
            }, cancellationToken);
        }
    }
}
