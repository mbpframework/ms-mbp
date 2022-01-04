using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shouldly;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading;
using Mbp.Caching;
using Xunit;

namespace Mbp.Caching.Test
{
    public class Caching_Test
    {
        private IMbpCache _ngCacheMemory = null;
        private IMbpCache _ngCacheRedis = null;

        [Fact]
        public void NgMemoryCache_Test()
        {
            var services = new ServiceCollection();

            var cachingOptionsForeMemory = new CachingModuleOptions_Test()
            {
                Provider = "memory",
                AbsoluteExpirationRelativeToNow = 2,
                Memory = new MemoryOptions_Test()
                {
                    CompactionPercentage = 0.05,
                    ExpirationScanFrequency = 5,
                    SizeLimit = 0
                },
                Redis = new RedisOptions_Test(),
                SlidingExpiration = 1
            };

            services.AddMemoryCache(options =>
            {
                options.SizeLimit = cachingOptionsForeMemory.Memory.SizeLimit.Value == 0 ? null : cachingOptionsForeMemory.Memory.SizeLimit;// 缓存条目
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(cachingOptionsForeMemory.Memory.ExpirationScanFrequency);// 过期缓存移除间隔
                options.CompactionPercentage = cachingOptionsForeMemory.Memory.CompactionPercentage;// 缓存条目数超过最大限制时候压缩率
            });

            var memoryOptions = Options.Create<CachingModuleOptions_Test>(cachingOptionsForeMemory);

            services.AddSingleton(memoryOptions);

            services.AddSingleton(typeof(IMbpCache), typeof(NgMemoryCache_Test));

            var provider = services.BuildServiceProvider();

            _ngCacheMemory = provider.GetService<IMbpCache>();

            _ngCacheMemory.Set("a", "aa");
            _ngCacheMemory.Get<string>("a").ShouldBe("aa");

            _ngCacheMemory.Refresh("a", "bb");
            _ngCacheMemory.Get<string>("a").ShouldBe("bb");

            object value;
            _ngCacheMemory.TryGetValue("a", out value).ShouldBeTrue();

            _ngCacheMemory.GetOrAdd("b", "abc");
            _ngCacheMemory.Get<string>("b").ShouldBe("abc");

            _ngCacheMemory.Remove("a");
            _ngCacheMemory.Get<string>("a").ShouldBeNull();

            _ngCacheMemory.Clear();
            _ngCacheMemory.Get<string>("b").ShouldBeNull();

            // 模拟多线程并发测试

            // 并发起点 1
            new Thread(() =>
            {
                Thread.Sleep(1000);
                // 2
                _ngCacheMemory.Set("c", "c1");
                _ngCacheMemory.Get<string>("c").ShouldBe("c1");

                Thread.Sleep(10000);
                // 5
                _ngCacheMemory.Get<string>("c").ShouldBe("c2");
            }).Start();

            // 并发起点 1
            new Thread(() =>
            {
                // 3
                Thread.Sleep(6000);
                _ngCacheMemory.Set("c", "c2");
            }).Start();

            // 并发起点 1

            var c1 = _ngCacheMemory.Get<string>("c");
            if (c1 != null)
            {
                c1.ShouldBe("c1");
            }

            Thread.Sleep(6500);
            // 4
            _ngCacheMemory.Get<string>("c").ShouldBe("c2");
        }

        [Fact]
        public void NgMemoryCacheDisable_Test()
        {
            var services = new ServiceCollection();

            var cachingOptionsForeMemory = new CachingModuleOptions_Test()
            {
                Enable = false,
                Provider = "memory",
                AbsoluteExpirationRelativeToNow = 2,
                Memory = new MemoryOptions_Test()
                {
                    CompactionPercentage = 0.05,
                    ExpirationScanFrequency = 5,
                    SizeLimit = 0
                },
                Redis = new RedisOptions_Test(),
                SlidingExpiration = 1
            };

            services.AddMemoryCache(options =>
            {
                options.SizeLimit = cachingOptionsForeMemory.Memory.SizeLimit.Value == 0 ? null : cachingOptionsForeMemory.Memory.SizeLimit;// 缓存条目
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(cachingOptionsForeMemory.Memory.ExpirationScanFrequency);// 过期缓存移除间隔
                options.CompactionPercentage = cachingOptionsForeMemory.Memory.CompactionPercentage;// 缓存条目数超过最大限制时候压缩率
            });

            var memoryOptions = Options.Create<CachingModuleOptions_Test>(cachingOptionsForeMemory);

            services.AddSingleton(memoryOptions);

            services.AddSingleton(typeof(IMbpCache), typeof(NgMemoryCache_Test));

            var provider = services.BuildServiceProvider();

            _ngCacheMemory = provider.GetService<IMbpCache>();

            _ngCacheMemory.Set("a", "aa");
            _ngCacheMemory.Get<string>("a").ShouldBeNull();

            _ngCacheMemory.Refresh("a", "bb");
            _ngCacheMemory.Get<string>("a").ShouldBeNull();

            object value;
            _ngCacheMemory.TryGetValue("a", out value).ShouldBeFalse();

            _ngCacheMemory.GetOrAdd("b", "abc");
            _ngCacheMemory.Get<string>("b").ShouldBeNull();
        }

        [Fact]
        public void NgDistributedCache_Test()
        {
            var services = new ServiceCollection();

            var cachingOptionsForeRedis = new CachingModuleOptions_Test()
            {
                Provider = "redis",
                AbsoluteExpirationRelativeToNow = 2,
                Memory = new MemoryOptions_Test(),
                Redis = new RedisOptions_Test()
                {
                    AppName = "Demo",
                    Environment = "Test",
                    StampKey = "STAMP",
                    ClientName = "Mbp-Demo",
                    ConnectTimeout = 5000,
                    DefaultVersion = "6.0.6",
                    EndPoints = new List<string>() { "172.18.35.61:6379" },
                    KeepAlive = 180,
                    Password = "Mbp"
                },
                SlidingExpiration = 1
            };

            var redisConfigOptions = new ConfigurationOptions()
            {
                CommandMap = CommandMap.Create(new HashSet<string>
                                { // EXCLUDE a few commands
                                "INFO", "CONFIG", "CLUSTER",
                                "PING", "ECHO", "CLIENT","UNLINK"
                                }, available: false),
                KeepAlive = cachingOptionsForeRedis.Redis.KeepAlive,
                DefaultVersion = new Version(cachingOptionsForeRedis.Redis.DefaultVersion),
                Password = cachingOptionsForeRedis.Redis.Password
            };
            cachingOptionsForeRedis.Redis.EndPoints.ForEach(hostAndPort =>
            {
                redisConfigOptions.EndPoints.Add(hostAndPort);
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = redisConfigOptions;
            });

            var redisOptions = Options.Create<CachingModuleOptions_Test>(cachingOptionsForeRedis);

            services.AddSingleton(redisOptions);

            services.AddSingleton(typeof(IMbpCache), typeof(NgDistributedCache_Test));

            var provider = services.BuildServiceProvider();

            _ngCacheRedis = services.BuildServiceProvider().GetService<IMbpCache>();

            // 设置时间戳
            var redisConfig = redisOptions.Value.Redis;
            var key = $"NG_{redisConfig.AppName}:{redisConfig.Environment}:STAMP";
            _ngCacheRedis.Set(key, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString(), absoluteExpirationRelativeToNow: TimeSpan.FromDays(3600));

            _ngCacheRedis.Set("a", "aa");
            _ngCacheRedis.Get<string>("a").ShouldBe("aa");

            _ngCacheRedis.Refresh("a", "bb");
            _ngCacheRedis.Get<string>("a").ShouldBe("bb");

            object value;
            _ngCacheRedis.TryGetValue("a", out value).ShouldBeTrue();

            _ngCacheRedis.GetOrAdd("b", "abc");
            _ngCacheRedis.Get<string>("b").ShouldBe("abc");

            _ngCacheRedis.Remove("a");
            _ngCacheRedis.Get<string>("a").ShouldBeNull();


            _ngCacheRedis.Clear();

            _ngCacheRedis.Get<string>("b").ShouldBeNull();
            _ngCacheRedis.Get<string>("a").ShouldBeNull();

            _ngCacheRedis.Set("a", "aa", absoluteExpirationRelativeToNow: TimeSpan.FromSeconds(3));
            _ngCacheRedis.Get<string>("a").ShouldBe("aa");

            Thread.Sleep(5000);
            _ngCacheRedis.Get<string>("a").ShouldBeNull();

            _ngCacheRedis.Set("a", "aa", absoluteExpirationRelativeToNow: TimeSpan.FromSeconds(3));

            _ngCacheRedis.Refresh("a", "bb", absoluteExpirationRelativeToNow: TimeSpan.FromSeconds(3));
            _ngCacheRedis.Get<string>("a").ShouldBe("bb");

            Thread.Sleep(5000);
            _ngCacheRedis.Get<string>("a").ShouldBeNull();
        }

        [Fact]
        public void NgDistributedCacheDisable_Test()
        {
            var services = new ServiceCollection();

            var cachingOptionsForeRedis = new CachingModuleOptions_Test()
            {
                Enable = false,
                Provider = "redis",
                AbsoluteExpirationRelativeToNow = 2,
                Memory = new MemoryOptions_Test(),
                Redis = new RedisOptions_Test()
                {
                    AppName = "Demo",
                    Environment = "Test",
                    StampKey = "STAMP",
                    ClientName = "Mbp-Demo",
                    ConnectTimeout = 5000,
                    DefaultVersion = "6.0.6",
                    EndPoints = new List<string>() { "172.18.35.61:6379" },
                    KeepAlive = 180,
                    Password = "Mbp"
                },
                SlidingExpiration = 1
            };

            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = new ConfigurationOptions()
                {
                    CommandMap = CommandMap.Create(new HashSet<string>
                                { // EXCLUDE a few commands
                                "INFO", "CONFIG", "CLUSTER",
                                "PING", "ECHO", "CLIENT","UNLINK"
                                }, available: false),
                    KeepAlive = cachingOptionsForeRedis.Redis.KeepAlive,
                    DefaultVersion = new Version(cachingOptionsForeRedis.Redis.DefaultVersion),
                    Password = cachingOptionsForeRedis.Redis.Password
                };
                cachingOptionsForeRedis.Redis.EndPoints.ForEach(hostAndPort =>
                {
                    options.ConfigurationOptions.EndPoints.Add(hostAndPort);
                });
            });

            var redisOptions = Options.Create<CachingModuleOptions_Test>(cachingOptionsForeRedis);

            services.AddSingleton(redisOptions);

            services.AddSingleton(typeof(IMbpCache), typeof(NgDistributedCache_Test));

            var provider = services.BuildServiceProvider();

            _ngCacheRedis = services.BuildServiceProvider().GetService<IMbpCache>();

            // 设置时间戳
            var redisConfig = redisOptions.Value.Redis;
            var key = $"NG_{redisConfig.AppName}:{redisConfig.Environment}:STAMP";
            _ngCacheRedis.Set(key, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString(), absoluteExpirationRelativeToNow: TimeSpan.FromDays(3600));

            _ngCacheRedis.Set("a", "aa");
            _ngCacheRedis.Get<string>("a").ShouldBeNull();

            _ngCacheRedis.Refresh("a", "bb");
            _ngCacheRedis.Get<string>("a").ShouldBeNull();

            object value;
            _ngCacheRedis.TryGetValue("a", out value).ShouldBeFalse();

            _ngCacheRedis.GetOrAdd("b", "abc");
            _ngCacheRedis.Get<string>("b").ShouldBeNull();
        }
    }

    public class CachingModuleOptions_Test
    {
        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 缓存提供程序
        /// </summary>
        public string Provider { get; set; }

        public int AbsoluteExpirationRelativeToNow { get; set; }

        public int SlidingExpiration { get; set; }

        public MemoryOptions_Test Memory { get; set; }

        public RedisOptions_Test Redis { get; set; }
    }

    public class MemoryOptions_Test
    {
        /// <summary>
        /// 最大条目限制
        /// </summary>
        public long? SizeLimit { get; set; }

        /// <summary>
        /// 过期缓存移除间隔 单位分钟
        /// </summary>
        public int ExpirationScanFrequency { get; set; }

        /// <summary>
        /// 压缩率
        /// </summary>
        public double CompactionPercentage { get; set; }
    }

    public class RedisOptions_Test
    {
        public string AppName { get; set; }

        public string Environment { get; set; }

        public string StampKey { get; set; }

        public List<string> EndPoints { get; set; }

        public string Password { get; set; }

        public int ConnectTimeout { get; set; }

        public string ClientName { get; set; }

        public int KeepAlive { get; set; }

        public string DefaultVersion { get; set; }
    }

    public class NgMemoryCache_Test : IMbpCache
    {
        // 保存当前缓存实例下的所有key.
        private readonly Stack<string> _keys;

        private static readonly object s_internalSyncObject = new object();

        private IMemoryCache _cache;

        private IOptions<CachingModuleOptions_Test> _options;

        public NgMemoryCache_Test(IMemoryCache memoryCache, IOptions<CachingModuleOptions_Test> options)
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

    public class NgDistributedCache_Test : IMbpCache
    {
        private readonly IDistributedCache _cache;

        private readonly string _keyTimeStamp = "STAMP";

        private readonly IOptions<CachingModuleOptions_Test> _options;

        public NgDistributedCache_Test(IDistributedCache distributedCache, IOptions<CachingModuleOptions_Test> options)
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
                //AbsoluteExpiration = absoluteExpiration,
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
                //SlidingExpiration = slidingExpiration
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
