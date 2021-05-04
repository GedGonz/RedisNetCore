using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IHttpClientFactory _httpClient;

        public RedisController(IDistributedCache _cash, IHttpClientFactory _httpClient)
        {
            this._cash = _cash;
            this._httpClient = _httpClient;
        }
        [HttpGet("{id}")]

        public async Task<IActionResult> Get(int id)
        {
            var value = await _cash.GetAsync(id.ToString());

            if (value==null) 
            {
                var post = await getPost(id);
                if (post != null)
                    await addToCache(post);
                return Ok(post);

            }

            return Ok(FromByteArray(value));
        }
        private async Task<Post> getPost(int id) {

            HttpClient client = _httpClient.CreateClient("post");
            return await client.GetFromJsonAsync<Post>($"posts/{id}");
        }

        private async Task addToCache(Post _post)
        {
            await _cash.SetAsync(_post.id.ToString(), ToByteArray(_post));
        }

        private byte[] ToByteArray(Post _post) 
        {
            return JsonSerializer.SerializeToUtf8Bytes(_post);
        }

        private Post FromByteArray(byte[] data) 
        {
            return JsonSerializer.Deserialize<Post>(data);
        }
    }
}
