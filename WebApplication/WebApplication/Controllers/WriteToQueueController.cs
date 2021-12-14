using Beanstalk.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

using WebApplication.Services;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WriteToQueueController : ControllerBase
    {
        private readonly IDatabase _db;
        private readonly string _queueName;
        private readonly BeanstalkConnection _beanstalkConnection;
        private readonly int _queueType;
        private readonly StatsService _statsService;

        public WriteToQueueController(IConnectionMultiplexer multiplexer, IConfiguration configuration, BeanstalkConnection beanstalkConnection, StatsService statsService)
        {
            _queueType = configuration?.GetValue<int>("QueueType") ?? 0;
            _queueName = configuration?.GetValue<string>("QueueName");
            _statsService = statsService;

            if (_queueType == 0 || _queueType == 2) _db = multiplexer?.GetDatabase(0);
            if (_queueType == 1) _beanstalkConnection = beanstalkConnection;
        }

        [HttpGet("Push")]
        public async Task<IActionResult> Push()
        {
            try
            {
                if (_queueType == 0 || _queueType == 2) await _db.ListRightPushAsync(_queueName, GetTimestamp());
                else
                {
                    await _beanstalkConnection.Use(_queueName);
                    await _beanstalkConnection.Put(GetTimestamp());
                }

                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("Stats")]
        public IActionResult Stats(bool isReset = false)
        {
            if (isReset)
            {
                _statsService.ResetStats();
            }

            return new JsonResult(_statsService.GetStats());
        }

        private string GetTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        }
    }
}
