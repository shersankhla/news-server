using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace news_app_server.Controllers
{
    [ApiController]
    [Route("api/news")]
    public class NewsApiController : ControllerBase
    {
         private HttpClient _client;
        private readonly IMemoryCache _memoryCache;
        public string cacheKey = "newsarticles";

         public NewsApiController(HttpClient client, IMemoryCache memoryCache)
        {
            _client = client;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("articles")]
        public async Task<IActionResult> Get()
        {
            string inputs;
            if (!_memoryCache.TryGetValue(cacheKey, out inputs))
            {
                HttpResponseMessage response = await _client.GetAsync("https://fakenews.squirro.com/news/sport");
                Thread.Sleep(1000);
                var res = await response.Content.ReadAsStringAsync();
                inputs = res;
                _memoryCache.Set(cacheKey, inputs,
                        new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(60)));
            }
            return Ok(inputs);
        }
    }
}