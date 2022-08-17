using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                Id = id,
                Name = "Fast Sample Payload",
                Data = "Some data lives here"
            };
        }

        [HttpGet]
        [Route("delayed/{id}/{delay}")]
        public async Task<SamplePayload> GetDelayed(int id, int delayMs)
        {
            _logger.LogInformation($"Get Delayed {id}");

            // delay between delayMs and 2 + delayMs
            await Task.Delay(1000 + delayMs);
            return new SamplePayload
            {
               Id = id,
               Name = "Delayed Sample Payload",
               Data = "Some data lives here"
            };
        }

        [HttpGet]
        [Route("slow/{id}")]
        public async Task GetSlow(int id)
        {
            _logger.LogInformation($"Get Slow {id}");
            
            Stopwatch chunkTimer = Stopwatch.StartNew();
            const int contentLength = 1024 * 1024 * 1;  // 1 MB
            const int chunkSize = 1024 * 16;            // in 16kb chunks
            const int responseTimeMs = 10000;           // Response to take 10 sec

            Debug.Assert(chunkSize % 128 == 0 && contentLength % chunkSize == 0, "chunkSize must be multiple of 128 and divide into contentLength");
            
            HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            HttpContext.Response.ContentLength = contentLength;
            HttpContext.Response.ContentType = "application/octet-stream";

            int bytesWritten = 0;
            while (bytesWritten < contentLength)
            {
                StringBuilder content = new StringBuilder(chunkSize);
                while (content.Length < chunkSize)
                {
                    string txt = $"{bytesWritten + 128} bytes".PadRight(127, '-') + "\n";
                    content.Append(txt);
                    bytesWritten += 128;
                }
                byte[] chunk = Encoding.UTF8.GetBytes(content.ToString());
                Debug.Assert(chunk.Length == chunkSize);

                await HttpContext.Response.BodyWriter.WriteAsync(chunk.AsMemory());
                
                double percentComplete = (double)bytesWritten / contentLength;
                long durationSoFarMs = chunkTimer.ElapsedMilliseconds;
                int delayMs = (int)(percentComplete * responseTimeMs - durationSoFarMs);
                if (delayMs > 0) 
                    await Task.Delay(delayMs);
            }

            await HttpContext.Response.CompleteAsync();

            _logger.LogInformation($"Get Slow {id} - Complete");
        }
        
        [HttpGet]
        [Route("unreliable/{id}")]
        public async Task<SamplePayload> GetUnreliable(int id)
        {
            _logger.LogInformation($"Unreliable {id}");

            if (_random.Next(1, 3) == 1)
            {
                _logger.LogInformation($"--- Delaying Request!");
                await Task.Delay(TimeSpan.FromSeconds(4));
            }

            return new SamplePayload
            {
                Id = id,
                Name = "Unreliable Sample Payload",
                Data = "Some data lives here"
            };
        }

        [HttpGet]
        [Route("exception/{id}")]
        public async Task<SamplePayload> GetException(int id)
        {
            _logger.LogInformation($"Exception {id}");
            
            await Task.Delay(1000);
            
            _logger.LogInformation($"Throwing Exception!");
            throw new HttpRequestException("This endpoint must be unreliable!");
        }


        [HttpPost]
        [Route("post")]
        public async Task<SamplePayload> Post(SamplePayload payload)
        {
            _logger.LogInformation($"Post (Id: {payload.Id})");
            payload.Id++;
            await Task.Delay(1000); // simulate a slow datalayer operation
            return payload;
        }

        [HttpPut]
        [Route("put")]
        public async Task<SamplePayload> Put(SamplePayload payload)
        {
            _logger.LogInformation($"Put (Id: {payload.Id})");
            payload.Id++;
            await Task.Delay(1000); // simulate a slow datalayer operation
            return payload;
        }
    }
}
