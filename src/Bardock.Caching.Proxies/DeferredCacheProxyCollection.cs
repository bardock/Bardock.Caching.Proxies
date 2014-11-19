using System;
using Newtonsoft.Json;
using Sixeyed.Caching;

namespace Bardock.Caching.Proxies
{
    /// <summary>
    /// This class manages a set of proxies with the same data type.
    /// It identifies each proxy building a key by given variable params
    /// </summary>
    /// <typeparam name="TParams">Type of params that identifies a cached item</typeparam>
    /// <typeparam name="TData">Type of cached data</typeparam>
    public class DeferredCacheProxyCollection<TParams, TData>
    {
        protected const string KEY_SEPARATOR = "_";

        protected ICache _cache;
        protected string _keyPrefix;
        protected Func<TData, TimeSpan> _expiration;

        /// <param name="cache">ICache implementation</param>
        /// <param name="keyPrefix">Cache key prefix</param>
        /// <param name="expiration">Cache expiration</param>
        public DeferredCacheProxyCollection(
            ICache cache,
            string keyPrefix,
            TimeSpan expiration = default(TimeSpan))
            : this(cache, keyPrefix, x => expiration)
        { }

        /// <param name="cache">ICache implementation</param>
        /// <param name="keyPrefix">Cache key prefix</param>
        /// <param name="expiration">A function that recives the object that is going to be inserted in cache and returs the cache expiration to use</param>
        public DeferredCacheProxyCollection(
            ICache cache,
            string keyPrefix,
            Func<TData, TimeSpan> expiration)
        {
            _cache = cache;
            if (keyPrefix == null)
                throw new ArgumentException("key cannot be null");
            _keyPrefix = keyPrefix;
            _expiration = expiration;
        }

        /// <summary>
        /// Get proxy data by specified params
        /// </summary>
        /// <param name="params">Params that identifies an item</param>
        /// <param name="dataLoadFunc">Gets data from original source</param>
        /// <param name="locker">An optional object which will be used to lock parallel invocations of dataLoadFunc. E.g. it could be a database context that does not allow run queries in parallel.</param>
        public TData GetData(
            TParams @params,
            Func<TParams, TData> dataLoadFunc,
            object locker = null)
        {
            string key = BuildKey(@params);
            return GetProxy(key).GetData(() => dataLoadFunc(@params), locker);
        }

        /// <summary>
        /// Manually set data by specified params. This is useful when you just created or updated the data and want to store it in cache.
        /// </summary>
        /// <param name="params">Params that identifies an item</param>
        /// <param name="data">Data to be cached</param>
        public void SetData(TParams @params, TData data)
        {
            string key = BuildKey(@params);
            GetProxy(key).SetData(data);
        }

        /// <summary>
        /// Clear a cached item by specified params
        /// </summary>
        /// <param name="params">Params that identifies an item</param>
        public void Clear(TParams @params)
        {
            string key = BuildKey(@params);
            GetProxy(key).Clear();
        }

        /// <summary>
        /// Clear all cached items
        /// </summary>
        public void ClearAll()
        {
            _cache.RemoveAll(keyPrefix: _keyPrefix + KEY_SEPARATOR);
            // TODO: partial remove building a key prefix
        }

        protected string BuildKey(TParams @params)
        {
            var serializedParams = (@params != null && @params is string)
                ? @params.ToString()
                : JsonConvert.SerializeObject(@params);

            return _keyPrefix + KEY_SEPARATOR + serializedParams;
        }

        protected DeferredCacheProxy<TData> GetProxy(string key)
        {
            return new DeferredCacheProxy<TData>(_cache, key, _expiration);
        }
    }
}