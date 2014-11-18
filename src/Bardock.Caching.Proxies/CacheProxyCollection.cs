using System;
using Sixeyed.Caching;

namespace Bardock.Caching.Proxies
{
    /// <summary>
    /// Abstracts access to a collection of cache items.
    /// This class manages a set of proxies with same data type.
    /// It identifies each proxy building a key by given variable params
    /// </summary>
    /// <typeparam name="TData">Type of cached data</typeparam>
    /// <typeparam name="TParams">Type of params that identifies a cached item</typeparam>
    public class CacheProxyCollection<TData, TParams> : DeferredCacheProxyCollection<TData, TParams>
    {
        private Func<TParams, TData> _dataLoadFunc;
        private object _locker;

        /// <param name="dataLoadFunc">Gets data from original source</param>
        /// <param name="cache">ICache implementation</param>
        /// <param name="keyPrefix">Cache key prefix</param>
        /// <param name="expiration">Cache expiration</param>
        /// <param name="locker">An optional object which will be used to lock parallel invocations of dataLoadFunc. E.g. it could be a database context that does not allow run queries in parallel.</param>
        public CacheProxyCollection(
            Func<TParams, TData> dataLoadFunc,
            ICache cache,
            string keyPrefix,
            TimeSpan expiration = default(TimeSpan),
            object locker = null)
            : this(dataLoadFunc, cache, keyPrefix, x => expiration, locker)
        { }

        /// <param name="dataLoadFunc">Gets data from original source</param>
        /// <param name="cache">ICache implementation</param>
        /// <param name="keyPrefix">Cache key prefix</param>
        /// <param name="expiration">A function that recives the object that is going to be inserted in cache and returs the cache expiration to use</param>
        /// <param name="locker">An optional object which will be used to lock parallel invocations of dataLoadFunc. E.g. it could be a database context that does not allow run queries in parallel.</param>
        public CacheProxyCollection(
            Func<TParams, TData> dataLoadFunc,
            ICache cache,
            string keyPrefix,
            Func<TData, TimeSpan> expiration,
            object locker = null)
            : base(cache, keyPrefix, expiration)
        {
            _dataLoadFunc = dataLoadFunc;
            _locker = locker;
        }

        /// <summary>
        /// Get proxy data by specified params
        /// </summary>
        public TData GetData(TParams @params)
        {
            return base.GetData(_dataLoadFunc, @params: @params, locker: _locker);
        }
    }
}