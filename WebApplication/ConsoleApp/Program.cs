using Beanstalk.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
			var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

			using var serviceProvider = serviceCollection.BuildServiceProvider();

			var configuration = serviceProvider.GetRequiredService<IConfiguration>();
			var queueType = configuration.GetValue<int>("QueueType");
			var cancelTokenSource = new CancellationTokenSource();

			switch (queueType)
            {
				case 0:
					Task.Run(async () => await RunRedisStack(serviceProvider, configuration, cancelTokenSource.Token), cancelTokenSource.Token);
					break;
				case 1:
					Task.Run(async () => await RunRedisQueue(serviceProvider, configuration, cancelTokenSource.Token), cancelTokenSource.Token);
					break;
				case 2:
					Task.Run(async () => await RunBeanstalkQueue(serviceProvider, configuration, cancelTokenSource.Token), cancelTokenSource.Token);
					break;
            }

			while (true)
            {
				Task.Delay(1000).Wait();
            }
			cancelTokenSource.Cancel();
		}

		private static async Task RunRedisStack(IServiceProvider serviceProvider, IConfiguration configuration, CancellationToken cancellationToken)
        {
			var stackName = configuration?.GetValue<string>("StackName");

			var client = serviceProvider.GetRequiredService<IConnectionMultiplexer>().GetDatabase(0);
			Console.WriteLine("Start listening queue with name '{0}'", stackName);
			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();

				var data = await client.ListRightPopAsync(stackName);
				if (data.HasValue) WriteResultToConsole(long.Parse(data));
			}
		}

		private static async Task RunRedisQueue(IServiceProvider serviceProvider, IConfiguration configuration, CancellationToken cancellationToken)
		{
			var queueName = configuration?.GetValue<string>("QueueName");

			var client = serviceProvider.GetRequiredService<IConnectionMultiplexer>().GetDatabase(0);
			Console.WriteLine("Start listening queue with name '{0}'", queueName);
			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();

				var data = await client.ListLeftPopAsync(queueName);
				if (data.HasValue)  WriteResultToConsole(long.Parse(data));
			}
		}

		private static async Task RunBeanstalkQueue(IServiceProvider serviceProvider, IConfiguration configuration, CancellationToken cancellationToken)
		{
			var queueName = configuration?.GetValue<string>("QueueName");

			var client = serviceProvider.GetRequiredService<BeanstalkConnection>();
			await client.Watch(queueName);
			Console.WriteLine("Start listening queue with name '{0}'", queueName);
			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();

				var data = await client.Reserve(TimeSpan.FromSeconds(15));

				if (data?.Id != null)
				{
					WriteResultToConsole(long.Parse(data.Data));
					await client.Delete(data.Id);
				}
			}
		}

		private static void WriteResultToConsole(long timestampFromQueue)
        {
			var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			var elapsedTime = timestamp - timestampFromQueue;
			Console.WriteLine("Elapsed time since recording: {0} sec", elapsedTime);
		}

		private static void ConfigureServices(IServiceCollection services)
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
				.AddJsonFile("appsettings.json", false)
				.Build();

			services.AddSingleton<IConfiguration>(configuration);
			services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
			{
				var options = ConfigurationOptions.Parse(serviceProvider.GetRequiredService<IConfiguration>().GetValue<string>("Redis:ConnectionString"));
				options.AllowAdmin = true;
				options.CommandMap = CommandMap.Default;
				return ConnectionMultiplexer.Connect(options);
			});
			services.AddSingleton((serviceProvider) =>
			{
				var configuration = serviceProvider.GetRequiredService<IConfiguration>();
				return new BeanstalkConnection(configuration.GetValue<string>("Beanstalk:Host"), configuration.GetValue<ushort>("Beanstalk:Port"));
			});
		}
	}
}
