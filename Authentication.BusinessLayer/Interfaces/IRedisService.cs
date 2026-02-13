using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.BusinessLayer.Interfaces
{
    public interface IRedisService
    {
        Task SetStringAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetStringAsync(string key);
        Task<bool> DeleteAsync(string key);
        Task<long> IncrementAsync(string key, TimeSpan? expiry = null);
        Task<bool> ExistsAsync(string key);
        Task<T?> GetObjectAsync<T>(string key);
        Task SetObjectAsync<T>(string key, T value, TimeSpan? expiry =null);
        Task<bool> KeyExpireAsync(string key, TimeSpan expiry);

    }
}
