using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace WebApplication.Services
{
    public class StatsService
    {
        private long numberOfReadMessages = 0;
        private ConcurrentBag<long> store;

        public StatsService()
        {
            store = new ConcurrentBag<long>();
        }

        public void AddNumberOfReadMessages()
        {
            Interlocked.Increment(ref numberOfReadMessages);
        }

        public void AddElapsedTime(long ms)
        {
            store.Add(ms);
        }

        public object GetStats()
        {
            var elapsedTimeList = store.ToArray();

            return new
            {
                numberOfReadMessages = numberOfReadMessages,
                avgElapsedTimeFromWriteToRead = $"{(numberOfReadMessages > 0 && elapsedTimeList.Length > 0 ? elapsedTimeList.Sum() / numberOfReadMessages : 0)} ms",
                maxElapsedTimeFromWriteToRead = $"{(elapsedTimeList.Length > 0 ? elapsedTimeList.Max() : 0)} ms",
                minElapsedTimeFromWriteToRead = $"{(elapsedTimeList.Length > 0 ? elapsedTimeList.Min() : 0)} ms",
            };
        }

        public void ResetStats()
        {
            numberOfReadMessages = 0;
            store.Clear();
        }
    }
}
