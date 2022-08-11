using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HttpClientTestAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

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
        [Route("fast")]
        public async Task<string> GetFast()
        {
            await Task.CompletedTask; // just to get rid of the warning
            return "fast";
        }

        [HttpGet]
        [Route("slow")]
        public async Task<string> GetSlow()
        {
            await Task.Delay(TimeSpan.FromSeconds(45));
            return "slow";
        }

        [HttpGet]
        [Route("unreliable")]
        public async Task<string> GetUnreliable()
        {
            //TODO: randomly fail
            await Task.Delay(TimeSpan.FromSeconds(45));
            return "slow";
        }
    }
}
