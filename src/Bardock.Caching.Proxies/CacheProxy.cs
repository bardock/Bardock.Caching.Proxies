using System;
using Sixeyed.Caching;

namespace Bardock.Caching.Proxies
{
    public class CacheProxy<T> : DeferredCacheProxy<T>
    {
        private Func<T> _dataLoadFunc;
        private object _locker;

        public CacheProxy(
            Func<T> dataLoadFunc,
            ICache cache,
            string key,
            TimeSpan expiration = default(TimeSpan),
            object locker = null)
            : this(dataLoadFunc, cache, key, x => expiration, locker)
        { }

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

        public T GetData()
        {
            return base.GetData(_dataLoadFunc, _locker);
        }
    }
}