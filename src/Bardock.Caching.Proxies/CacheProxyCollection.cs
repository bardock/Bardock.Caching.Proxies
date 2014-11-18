using System;
using Sixeyed.Caching;

namespace Bardock.Caching.Proxies
{
    /// <summary>
    /// This class manages a set of proxies with same data type.
    /// It identifies each proxy building a key by given variable params
    /// </summary>
    /// <typeparam name="TData">Type of cached data</typeparam>
    /// <typeparam name="TData">Type of params that identifies a cached item</typeparam>
    public class CacheProxyCollection<TData, TParams> : DeferredCacheProxyCollection<TData, TParams>
    {
        private Func<TParams, TData> _dataLoadFunc;
        private object _locker;

        public CacheProxyCollection(
            Func<TParams, TData> dataLoadFunc,
            ICache cache,
            string keyPrefix,
            TimeSpan expiration = default(TimeSpan),
            object locker = null)
            : this(dataLoadFunc, cache, keyPrefix, x => expiration, locker)
        { }

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