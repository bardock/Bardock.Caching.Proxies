using System;
using Sixeyed.Caching;

namespace Bardock.Caching.Proxies
{
    /// <summary>
    /// Abstracts access to a cache item
    /// </summary>
    /// <typeparam name="T">Type of cached data</typeparam>
    public class CacheProxy<T> : DeferredCacheProxy<T>
    {
        private Func<T> _dataLoadFunc;
        private object _locker;

        /// <param name="dataLoadFunc">Gets data from original source</param>
        /// <param name="cache">ICache implementation</param>
        /// <param name="key">Cache key</param>
        /// <param name="expiration">Cache expiration</param>
        /// <param name="locker">An optional object which will be used to lock parallel invocations of dataLoadFunc. E.g. it could be a database context that does not allow run queries in parallel.</param>
        public CacheProxy(
            Func<T> dataLoadFunc,
            ICache cache,
            string key,
            TimeSpan expiration = default(TimeSpan),
            object locker = null)
            : this(dataLoadFunc, cache, key, x => expiration, locker)
        { }

        /// <param name="dataLoadFunc">Gets data from original source</param>
        /// <param name="cache">ICache implementation</param>
        /// <param name="key">Cache key</param>
        /// <param name="expiration">A function that recives the object that is going to be inserted in cache and returs the cache expiration to use</param>
        /// <param name="locker">An optional object which will be used to lock parallel invocations of dataLoadFunc. E.g. it could be a database context that does not allow run queries in parallel.</param>
        public CacheProxy(
            Func<T> dataLoadFunc,
            ICache cache,
            string key,
            Func<T, TimeSpan> expiration,
            object locker = null)
            : base(cache, key, expiration)
        {
            _dataLoadFunc = dataLoadFunc;
            _locker = locker;
        }

        /// <summary>
        /// Gets data from cache
        /// </summary>
        public T GetData()
        {
            return base.GetData(_dataLoadFunc, _locker);
        }
    }
}