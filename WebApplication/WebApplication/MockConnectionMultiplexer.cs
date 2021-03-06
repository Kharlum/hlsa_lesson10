using StackExchange.Redis;
using StackExchange.Redis.Profiling;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace WebApplication
{
    public class MockConnectionMultiplexer: IConnectionMultiplexer
    {
        public void RegisterProfiler(Func<ProfilingSession> profilingSessionProvider)
        {
            throw new NotImplementedException();
        }

        public ServerCounters GetCounters()
        {
            throw new NotImplementedException();
        }

        public EndPoint[] GetEndPoints(bool configuredOnly = false)
        {
            throw new NotImplementedException();
        }

        public void Wait(Task task)
        {
            throw new NotImplementedException();
        }

        public T Wait<T>(Task<T> task)
        {
            throw new NotImplementedException();
        }

        public void WaitAll(params Task[] tasks)
        {
            throw new NotImplementedException();
        }

        public int HashSlot(RedisKey key)
        {
            throw new NotImplementedException();
        }

        public ISubscriber GetSubscriber(object asyncState = null)
        {
            throw new NotImplementedException();
        }

        public IDatabase GetDatabase(int db = -1, object asyncState = null)
        {
            throw new NotImplementedException();
        }

        public IServer GetServer(string host, int port, object asyncState = null)
        {
            throw new NotImplementedException();
        }

        public IServer GetServer(string hostAndPort, object asyncState = null)
        {
            throw new NotImplementedException();
        }

        public IServer GetServer(IPAddress host, int port)
        {
            throw new NotImplementedException();
        }

        public IServer GetServer(EndPoint endpoint, object asyncState = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ConfigureAsync(TextWriter log = null)
        {
            throw new NotImplementedException();
        }

        public bool Configure(TextWriter log = null)
        {
            throw new NotImplementedException();
        }

        public string GetStatus()
        {
            throw new NotImplementedException();
        }

        public void GetStatus(TextWriter log)
        {
            throw new NotImplementedException();
        }

        public void Close(bool allowCommandsToComplete = true)
        {
            throw new NotImplementedException();
        }

        public Task CloseAsync(bool allowCommandsToComplete = true)
        {
            throw new NotImplementedException();
        }

        public string GetStormLog()
        {
            throw new NotImplementedException();
        }

        public void ResetStormLog()
        {
            throw new NotImplementedException();
        }

        public long PublishReconfigure(CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public Task<long> PublishReconfigureAsync(CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public int GetHashSlot(RedisKey key)
        {
            throw new NotImplementedException();
        }

        public void ExportConfiguration(Stream destination, ExportOptions options = (ExportOptions)(-1))
        {
            throw new NotImplementedException();
        }

        public string ClientName { get; }
        public string Configuration { get; }
        public int TimeoutMilliseconds { get; }
        public long OperationCount { get; }
        public bool PreserveAsyncOrder { get; set; }
        public bool IsConnected { get; }
        public bool IsConnecting { get; }
        public bool IncludeDetailInExceptions { get; set; }
        public int StormLogThreshold { get; set; }

        public event EventHandler<RedisErrorEventArgs> ErrorMessage;
        public event EventHandler<ConnectionFailedEventArgs> ConnectionFailed;
        public event EventHandler<InternalErrorEventArgs> InternalError;
        public event EventHandler<ConnectionFailedEventArgs> ConnectionRestored;
        public event EventHandler<EndPointEventArgs> ConfigurationChanged;
        public event EventHandler<EndPointEventArgs> ConfigurationChangedBroadcast;
        public event EventHandler<HashSlotMovedEventArgs> HashSlotMoved;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
