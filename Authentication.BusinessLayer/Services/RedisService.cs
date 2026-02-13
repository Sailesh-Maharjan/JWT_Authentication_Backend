using Authentication.BusinessLayer.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Authentication.BusinessLayer.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;
        private readonly ILogger<RedisService> _logger;

        public RedisService(IConnectionMultiplexer redis, ILogger<RedisService> logger)
        {
            _database = redis.GetDatabase(); 
            _logger = logger;
        }

        public async Task<string?> GetStringAsync(string key)
        {
            try
            {
                return await _database.StringGetAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting Redis key: {key}");
                return null;
            }
        }

        public async Task SetStringAsync(string key, string value, TimeSpan? expiry = null)
        {
            try
            {
                await _database.StringSetAsync(key, value, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting Redis key :{key}");
            }

        }

        public async Task<bool> DeleteAsync(string key)
        {
            try
            {
                return await _database.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting Redis key: {key}");
                return false;
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return await _database.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking Redis key existence: {key}");
                return false;
            }
        }

        public async Task<T?> GetObjectAsync<T>(string key)
        {
            try
            {
                var value = await GetStringAsync(key);
                return value != null ? JsonSerializer.Deserialize<T>(value) : default; // default means returning the default value for the generic datatype
                                                                                       // like if datatype is  int then return 0 ,if string then  null....
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deserializing Redis Object for Key: {key}");
                return default;
            }
        }

        public async Task SetObjectAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var serializedObject = JsonSerializer.Serialize(value);
                await SetStringAsync(key, serializedObject, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Serializing Redis Object for key: {key}");
                throw;
            }
        }

        public async Task<long> IncrementAsync(string key, TimeSpan? expiry = null)
        {
            try
            {
                var value = await _database.StringIncrementAsync(key);
                if (expiry.HasValue)
                {
                    await _database.KeyExpireAsync(key, expiry);
                }
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error incrementing Redis key: {key}");
                throw;
            }
        }

        public async Task<bool> KeyExpireAsync(string key, TimeSpan expiry)
        {
            try
            {
                return await _database.KeyExpireAsync(key, expiry);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error setting expiry time for Redis key: {key}");
                throw;
            }
        }

    }
}
