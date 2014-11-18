using System;
using Bardock.Utils.Sync;
using Sixeyed.Caching;

namespace Bardock.Caching.Proxies
{
    /// <summary>
    /// Abstracts access to a cache item.
    /// The data load function is not required for creation.
    /// </summary>
    /// <typeparam name="T">Type of cached data</typeparam>
    public class DeferredCacheProxy<T>
    {
        public static readonly TimeSpan EXPIRATION_DEFAULT = TimeSpan.FromHours(2);
        private static readonly StringLocker _locker = new StringLocker();

        protected ICache _cache;
        protected string _key;
        protected Func<T, TimeSpan> _expiration;

        /// <param name="cache">ICache implementation</param>
        /// <param name="key">Cache key</param>
        /// <param name="expiration">Cache expiration</param>
        public DeferredCacheProxy(
            ICache cache,
            string key,
            TimeSpan expiration = default(TimeSpan))
            : this(cache, key, x => expiration)
        { }

        /// <param name="cache">ICache implementation</param>
        /// <param name="key">Cache key</param>
        /// <param name="expiration">A function that recives the object that is going to be inserted in cache and returs the cache expiration to use</param>
        public DeferredCacheProxy(
            ICache cache,
            string key,
            Func<T, TimeSpan> expiration)
        {
            _cache = cache;
            if (key == null)
                throw new ArgumentException("key cannot be null");
            _key = key;
            _expiration = expiration;
        }

        /// <summary>
        /// Gets data from cache
        /// </summary>
        /// <param name="dataLoadFunc">Gets data from original source</param>
        /// <param name="locker">An optional object which will be used to lock parallel invocations of dataLoadFunc. E.g. it could be a database context that does not allow run queries in parallel.</param>
        public T GetData(Func<T> dataLoadFunc, object locker = null)
        {
            // Avoid multiple invocations to the data load function.
            // Always lock by key because this is the recommended way using distributed cache.
            lock (_locker.GetLockObject(_key))
            {
                T data;
                try
                {
                    data = _cache.Get<T>(_key);
                }
                catch (CacheKeyNotFoundException)
                {
                    data = InvokeFunc(dataLoadFunc, locker: locker);
                    SetData(data);
                }
                return data;
            }
        }

        private T InvokeFunc(Func<T> f, object locker)
        {
            if (locker == null)
                return f();

            lock (locker)
            {
                return f();
            }
        }

        /// <summary>
        /// Manually set data. This is useful when you just created or updated the data and want to store it in cache.
        /// </summary>
        /// <param name="data">Data to be cached</param>
        public void SetData(T data)
        {
            _cache.Set(_key, data, GetExpiration(data));
        }

        protected TimeSpan GetExpiration(T obj)
        {
            var timespan = _expiration(obj);
            return timespan == default(TimeSpan) ? EXPIRATION_DEFAULT : timespan;
        }

        /// <summary>
        /// Clears cache item
        /// </summary>
        public void Clear()
        {
            _cache.Remove(_key);
        }
    }
}