using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RedisNetCore.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace RedisNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IDistributedCache _cash;
        private readonly ICacheService _cacheService;
        private readonly IHttpClientFactory _httpClient;

        public RedisController(IDistributedCache _cash, ICacheService _cacheService, IHttpClientFactory _httpClient)
        {
            this._cacheService = _cacheService;
            this._cash = _cash;
            this._httpClient = _httpClient;
        }
        [HttpGet("{id}")]

        public async Task<IActionResult> Get(int id)
        {
            var value = await _cacheService.getToCache(id.ToString());

            if (value==null) 
            {
                var post = await getPost(id);
                if (post != null)
                    await addToCache(post);
                return Ok(post);

            }

            var valor = _cacheService.FromByteArray<Post>(value);
            return Ok(valor);
        }
        private async Task<Post> getPost(int id) {

            HttpClient client = _httpClient.CreateClient("post");
            return await client.GetFromJsonAsync<Post>($"posts/{id}");
        }

        private async Task addToCache(Post _post)
        {
            await _cacheService.addToCache(_post.id.ToString(), _cacheService.ToByteArray(_post));
        }
    }
}
