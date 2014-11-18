using System;
using Newtonsoft.Json;
using Sixeyed.Caching;

namespace Bardock.Caching.Proxies
{
    /// <summary>
    /// This class manages a set of proxies with the same data type.
    /// It identifies each proxy building a key by given variable params
    /// </summary>
    /// <typeparam name="TData">Type of cached data</typeparam>
    /// <typeparam name="TData">Type of params that identifies a cached item</typeparam>
    public class DeferredCacheProxyCollection<TData, TParams>
    {
        protected const string KEY_SEPARATOR = "_";

        protected ICache _cache;
        protected string _keyPrefix;
        protected Func<TData, TimeSpan> _expiration;

        public DeferredCacheProxyCollection(
            ICache cache,
            string keyPrefix,
            TimeSpan expiration = default(TimeSpan))
            : this(cache, keyPrefix, x => expiration)
        { }

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
        public TData GetData(
            Func<TParams, TData> dataLoadFunc,
            TParams @params,
            object locker = null)
        {
            string key = BuildKey(@params);
            return GetProxy(key).GetData(() => dataLoadFunc(@params), locker);
        }

        public void SetData(TData data, TParams @params)
        {
            string key = BuildKey(@params);
            GetProxy(key).SetData(data);
        }

        /// <summary>
        /// Clear a cached item by specified params
        /// </summary>
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
            return _keyPrefix + KEY_SEPARATOR + JsonConvert.SerializeObject(@params);
        }

        protected DeferredCacheProxy<TData> GetProxy(string key)
        {
            return new DeferredCacheProxy<TData>(_cache, key, _expiration);
        }
    }
}