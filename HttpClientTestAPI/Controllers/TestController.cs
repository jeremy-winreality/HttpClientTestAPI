using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpClientTestAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HttpClientTestAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly Random _random = new Random();

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("status")]
        public string GetStatus()
        {
            return "HttpClientAPI is running";
        }


        [HttpGet]
        [Route("fast/{id}")]
        public async Task<SamplePayload> GetFast(int id)
        {
            _logger.LogInformation($"Get Fast {id}");
            await Task.Delay(10);

            return new SamplePayload
            {
                Id = 0,
                Name = "Fast Sample Payload",
                Data = "Some data lives here"
            };
        }

        [HttpGet]
        [Route("slow/{id}/{delay}")]
        //public async Task<IEnumerable<SamplePayload>> GetSlow(int delayMs)
        public async Task<SamplePayload> GetSlow(int id, int delayMs)
        {
            //TODO: refactor this so it's a stream instead of a delayed response - doesn't really mimic a slow connection in this form.
            _logger.LogInformation($"Get Slow {id}");

            // delay between delayMs and 2 + delayMs
            await Task.Delay(1000 + delayMs);
            return new SamplePayload
            {
               Id = 0,
               Name = "Slow Sample Payload",
               Data = "Some data lives here"
            };
        }
        
        // TODO: build out the _unreliable_ endpoints to test the retry policies
        [HttpGet]
        [Route("unreliable")]
        public async Task<string> GetUnreliable()
        {
            //TODO: psuedo-randonly throw exception to trigger a failed response
            await Task.Delay(TimeSpan.FromSeconds(121));
            return "slow";
        }
    }
}
