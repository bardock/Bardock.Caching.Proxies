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
            object locker,
            ICache cache, 
            string key, 
            TimeSpan expiration = default(TimeSpan))
            : this(dataLoadFunc, locker, cache, key, x => expiration) 
        { }

        public CacheProxy(
            Func<T> dataLoadFunc,
            object locker,
            ICache cache, 
            string key, 
            Func<T, TimeSpan> expiration)
            : base(cache, key, expiration)
        {
            _dataLoadFunc = dataLoadFunc;
            _locker = locker;
        }

        public T GetData()
        {
            return base.GetData(_dataLoadFunc, _locker);
        }

        public void SetData(T value)
        {
            base.SetData(value);
        }
    }
}
    