using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bardock.Utils.Sync;
using Sixeyed.Caching;

namespace Bardock.Caching.Proxies
{
    /// <summary>
    /// This class manages a set of proxies with the same data type. 
    /// It identifies each proxy building a key by given variable params
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks></remarks>
    public class DeferredCacheProxyCollection<T>
    {
        protected const string KEY_PARAM_NULL = "NULL";
        protected const string KEY_PARAM_SEPARATOR = "_";

        protected ICache _cache;
        protected string _keyPrefix;
        protected Func<T, TimeSpan> _expiration;
        protected Dictionary<string, DeferredCacheProxy<T>> _proxies = new Dictionary<string, DeferredCacheProxy<T>>();

        public DeferredCacheProxyCollection(
            ICache cache, 
            string keyPrefix, 
            TimeSpan expiration = default(TimeSpan))
            : this(cache, keyPrefix, x => expiration) 
        { }

        public DeferredCacheProxyCollection(
            ICache cache, 
            string keyPrefix, 
            Func<T, TimeSpan> expiration)
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
        /// <remarks>
        /// You must specified the @params in the order expected by dataLoadFunc
        /// </remarks>
        public T GetData(Func<T> dataLoadFunc, object locker, params object[] @params)
        {
            string key = BuildKey(@params);
            return GetProxy(key).GetData(dataLoadFunc, locker);
        }

        public void SetData(T data, params object[] @params)
        {
            string key = BuildKey(@params);
            GetProxy(key).SetData(data);
        }

        public void Clear(params object[] @params)
        {
            string key = BuildKey(@params);
            GetProxy(key).Clear();
        }

        public void Clear()
        {
            // TODO: Get all keys from ICache and remove them
            _proxies.Values.ToList().ForEach(x => x.Clear());
        }

        protected string BuildKey(params object[] @params)
        {
            StringBuilder keyBuilder = new System.Text.StringBuilder();
            keyBuilder.Append(_keyPrefix);

            foreach (object param in @params)
            {
                keyBuilder
                    .Append(KEY_PARAM_SEPARATOR)
                    .Append(param == null ? KEY_PARAM_NULL : param.ToString());
            }
            return keyBuilder.ToString();
        }

        protected DeferredCacheProxy<T> GetProxy(string key)
        {
            if (!_proxies.ContainsKey(key))
            {
                _proxies[key] = new DeferredCacheProxy<T>(_cache, key, _expiration);
            }
            return _proxies[key];
        }
    }
}
