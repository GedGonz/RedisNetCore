using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisNetCore.Cache
{
    public interface ICacheService
    {
        Task addToCache<T>(string key, T data);
        Task<IEnumerable<T>> FromByteToList<T>(byte[] data);
        Task<T> FromByteArray<T>(byte[] data);
        Task<byte[]> ToByteArray<T>(T _post);
        Task clearToCache(string key);
        Task<byte[]> getToCache(string key);
        bool existCache();
        bool existErrorInCaching();
    }
}
