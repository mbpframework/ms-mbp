using Mbp.Dependency;
using System;

namespace Mbp.Caching
{
    /// <summary>
    /// 缓存服务
    /// </summary>
    public interface IMbpCache : ISingletonDependency
    {
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <param name="absoluteExpiration"></param>
        /// <param name="absoluteExpirationRelativeToNow"></param>
        /// <param name="slidingExpiration"></param>
        void Set<TItem>(string key, TItem item,
            DateTimeOffset? absoluteExpiration = null,
            TimeSpan? absoluteExpirationRelativeToNow = null,
            TimeSpan? slidingExpiration = null);

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="key"></param>
        /// <param name="item"></param>
        void Refresh<TItem>(string key, TItem item, DateTimeOffset? absoluteExpiration = null,
            TimeSpan? absoluteExpirationRelativeToNow = null,
            TimeSpan? slidingExpiration = null);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Obsolete("请使用TryGetValue<TItem>")]
        bool TryGetValue(string key, out object value);

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetValue<TItem>(string key, out TItem value);

        /// <summary>
        /// 获取缓存 不存在返回null
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        TItem Get<TItem>(string key);

        /// <summary>
        /// 获取缓存 GetOrAdd
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        TItem GetOrAdd<TItem>(string key, TItem item, DateTimeOffset? absoluteExpiration = null,
            TimeSpan? absoluteExpirationRelativeToNow = null,
            TimeSpan? slidingExpiration = null);

        /// <summary>
        /// 清理所有缓存
        /// </summary>
        void Clear();
    }
}
