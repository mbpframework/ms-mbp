using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading;
using Mbp.Caching;

namespace Mbp.Caching
{
    /// <summary>
    /// 分布式缓存，所有公共方法只接受未格式化的key
    /// _cache必须接受格式化的key
    /// 代办：优化此类的结构设计，剥离全局key的管理
    /// </summary>
    internal class MbpDistributedCache : IMbpCache
    {
        private readonly IDistributedCache _cache;

        private readonly string _keyTimeStamp;

        private readonly IOptions<MbpCachingModuleOptions> _options;

        public MbpDistributedCache(IDistributedCache distributedCache, IOptions<MbpCachingModuleOptions> options)
        {
            _cache = distributedCache;
            _options = options;
            _keyTimeStamp = _options.Value.Redis.StampKey;
        }

        public void Clear()
        {
            if (!_options.Value.Enable) return;

            Thread.Sleep(1000);

            var redisConfig = _options.Value.Redis;
            var key = $"NG_{redisConfig.AppName}:{redisConfig.Environment}:{_keyTimeStamp}";

            _cache.Remove(key);
            // 只有时间戳的key是个特例，其他key必须传递未格式化的
            Set(key, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString(), absoluteExpirationRelativeToNow: TimeSpan.FromDays(3600));
        }

        public TItem Get<TItem>(string key)
        {
            if (!_options.Value.Enable) return default;

            if (!TryGetValue(key, out TItem value))
            {
                return default;
            }
            return (TItem)value;
        }

        public TItem GetOrAdd<TItem>(string key, TItem item, DateTimeOffset? absoluteExpiration = null,
            TimeSpan? absoluteExpirationRelativeToNow = null,
            TimeSpan? slidingExpiration = null)
        {
            if (!_options.Value.Enable) return default;

            if (!TryGetValue(key, out object value))
            {
                Set<TItem>(key, item, absoluteExpiration, absoluteExpirationRelativeToNow, slidingExpiration);
                return item;
            }
            return (TItem)value;
        }

        public void Refresh<TItem>(string key, TItem item, DateTimeOffset? absoluteExpiration = null,
            TimeSpan? absoluteExpirationRelativeToNow = null,
            TimeSpan? slidingExpiration = null)
        {
            if (!_options.Value.Enable) return;

            // remove old
            _cache.Remove(key);

            // add new
            Set(key, item, absoluteExpiration, absoluteExpirationRelativeToNow, slidingExpiration);
        }

        public void Remove(string key)
        {
            if (!_options.Value.Enable) return;

            _cache.Remove(MakeKey(key));
        }

        public void Set<TItem>(string key, TItem item, DateTimeOffset? absoluteExpiration = null, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpiration = null)
        {
            if (!_options.Value.Enable) return;

            key = MakeKey(key);

            if (absoluteExpirationRelativeToNow == null)
                absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.Value.AbsoluteExpirationRelativeToNow);
            if (slidingExpiration == null)
                slidingExpiration = TimeSpan.FromMinutes(_options.Value.SlidingExpiration);

            // 经过一次序列化
            _cache.SetString(key, JsonConvert.SerializeObject(item), new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
            });
        }

        public bool TryGetValue<TItem>(string key, out TItem value)
        {
            if (!_options.Value.Enable)
            {
                value = default;
                return false;
            }

            key = MakeKey(key);

            return TryGetValueInternal(key, out value);
        }


        public bool TryGetValue(string key, out object value)
        {
            if (!_options.Value.Enable)
            {
                value = null;
                return false;
            }

            key = MakeKey(key);

            return TryGetValueInternal(key, out value);
        }

        // 格式化key，只处理未格式化的key
        private string MakeKey(string key)
        {
            if (key.EndsWith(_keyTimeStamp) && key.StartsWith("NG_")) return key;

            var redisConfig = _options.Value.Redis;

            // 缓存key模板 0 应用唯一标志，1 环境标识 2 业务key标识 3 版本（使用时间戳）
            string keyformate = $"NG_{redisConfig.AppName}:{redisConfig.Environment}" + @":{0}_" + GetStamp();

            return string.Format(keyformate, key);
        }

        // 获取时间戳
        private string GetStamp()
        {
            var redisConfig = _options.Value.Redis;
            var key = $"NG_{redisConfig.AppName}:{redisConfig.Environment}:{_keyTimeStamp}";

            if (!TryGetValueInternal(key, out object value))
            {
                var stamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString();
                Set(key, stamp, absoluteExpirationRelativeToNow: TimeSpan.FromDays(3600));
                return stamp;
            }

            return value.ToString();
        }

        // 尝试获取缓存，直接使用key，调用者需要将key格式化
        private bool TryGetValueInternal<TItem>(string key, out TItem value)
        {
            value = default;
            var result = _cache.GetString(key);
            if (result == null) return false;

            value = JsonConvert.DeserializeObject<TItem>(result.ToString());

            return true;
        }

    }
}
