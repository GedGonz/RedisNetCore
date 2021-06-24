using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RedisNetCore.Cache
{
    public class CacheService : ICacheService
    {

        private byte[] _datacache;
        private bool existError = false;
        private readonly IDistributedCache _cash;
        public CacheService(IDistributedCache _cash)
        {
            this._cash = _cash;
        }
        /// <summary>
        /// add object to cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task addToCache<T>(string key, T data)
        {
            try
            {
                var _data = JsonSerializer.SerializeToUtf8Bytes(data);
                await _cash.SetAsync(key, _data);
            }
            catch (RedisConnectionException) { }
        }
        /// <summary>
        /// get cache
        /// </summary>
        /// <param name="key">key cache</param>
        /// <returns>array in byte</returns>
        public async Task<byte[]> getToCache(string key)
        {
            try
            {
                _datacache = await _cash.GetAsync(key);
                return _datacache;
            }
            catch (RedisConnectionException)
            {
                existError = true;
                return _datacache;
            }
        }
        /// <summary>
        /// clear cache from key
        /// </summary>
        /// <param name="key">key cache</param>
        /// <returns></returns>
        public async Task clearToCache(string key)
        {
            try
            {
                await _cash.RemoveAsync(key);
            }
            catch (RedisConnectionException) { }

        }

 
        public bool existCache()
        {
            return (_datacache != null) ? _datacache.Any() : false;
        }

        public bool existErrorInCaching()
        {
            return existError;
        }

        public async Task<IEnumerable<T>> FromByteToList<T>(byte[] data)
        {
            await Task.Delay(1000);
            return JsonSerializer.Deserialize<IEnumerable<T>>(data);
        }
    }
}
