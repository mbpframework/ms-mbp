using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Mbp.Caching;

namespace Mbp.Caching
{
    /// <summary>
    /// 本地缓存
    /// </summary>
    internal class MbpMemoryCache : IMbpCache
    {
        // 保存当前缓存实例下的所有key.
        private readonly Stack<string> _keys;

        private static readonly object s_internalSyncObject = new object();

        private IMemoryCache _cache;

        private IOptions<MbpCachingModuleOptions> _options;

        public MbpMemoryCache(IMemoryCache memoryCache, IOptions<MbpCachingModuleOptions> options)
        {
            _cache = memoryCache;
            _keys = new Stack<string>();
            _options = options;
        }

        public void Clear()
        {
            if (!_options.Value.Enable) return;

            while (_keys.TryPop(out string key))
            {
                _cache.Remove(key);
            }
        }

        public TItem Get<TItem>(string key)
        {
            if (!_options.Value.Enable) return default;

            if (!TryGetValue(key, out object value))
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

            _cache.Remove(key);
        }

        public void Set<TItem>(string key, TItem item, DateTimeOffset? absoluteExpiration = null, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpiration = null)
        {
            if (!_options.Value.Enable) return;

            if (absoluteExpirationRelativeToNow == null)
                absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.Value.AbsoluteExpirationRelativeToNow);
            if (slidingExpiration == null)
                slidingExpiration = TimeSpan.FromMinutes(_options.Value.SlidingExpiration);

            // 需要同步Set操作和Push操作，所以选择用锁，如果不考虑clear设计，可以取消锁的设计，因为.net core runtime 处理了缓存的原子操作
            lock (s_internalSyncObject)
            {
                _cache.Set(key, item, new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = absoluteExpiration,
                    AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
                    SlidingExpiration = slidingExpiration
                });

                if (!_keys.Contains(key))
                    _keys.Push(key);
            }
        }

        public bool TryGetValue<TItem>(string key, out TItem value)
        {
            if (!_options.Value.Enable)
            {
                value = default;
                return false;
            }

            return _cache.TryGetValue(key, out value);
        }

        public bool TryGetValue(string key, out object value)
        {
            if (!_options.Value.Enable)
            {
                value = null;
                return false;
            }

            return _cache.TryGetValue(key, out value);
        }
    }
}
