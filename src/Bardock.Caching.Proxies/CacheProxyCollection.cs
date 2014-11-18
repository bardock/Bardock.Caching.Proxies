using System;
using Sixeyed.Caching;

namespace Bardock.Caching.Proxies
{
    /// <summary>
    /// This class manages a set of proxies with same data type.
    /// It identifies each proxy building a key by given variable params
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks></remarks>
    public class CacheProxyCollection<T> : DeferredCacheProxyCollection<T>
    {
        private Func<object[], Func<T>> _dataLoadFunc;
        private object _locker;

        public CacheProxyCollection(
            Func<object[], Func<T>> dataLoadFunc,
            ICache cache,
            string keyPrefix,
            TimeSpan expiration = default(TimeSpan),
            object locker = null)
            : this(dataLoadFunc, cache, keyPrefix, x => expiration, locker)
        { }

        public CacheProxyCollection(
            Func<object[], Func<T>> dataLoadFunc,
            ICache cache,
            string keyPrefix,
            Func<T, TimeSpan> expiration,
            object locker = null)
            : base(cache, keyPrefix, expiration)
        {
            _dataLoadFunc = dataLoadFunc;
            _locker = locker;
        }

        /// <summary>
        /// Get proxy data by specified params
        /// </summary>
        /// <remarks>
        /// You must specified the @params in the order expected by dataLoadFunc
        /// </remarks>
        public T GetData(params object[] @params)
        {
            return base.GetData(_dataLoadFunc(@params), _locker, @params: @params);
        }
    }
}