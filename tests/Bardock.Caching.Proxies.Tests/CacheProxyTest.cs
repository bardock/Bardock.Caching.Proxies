using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Sixeyed.Caching.Serialization;
using Xunit;

namespace Bardock.Caching.Proxies.Tests
{
    public class CacheProxyTest
    {
        private const string KEY = "KEY";

        [Fact]
        public void GetData_DefaultExpiration()
        {
            var cache = new Mock<CacheMock>() { CallBase = true };

            var proxy = new CacheProxy<DateTime>(() => DateTime.Now, cache: cache.Object, key: KEY);

            var v1 = proxy.GetData();
            Thread.Sleep(50);
            var v2 = proxy.GetData();
            Thread.Sleep(50);
            var v3 = proxy.GetData();

            Assert.Equal(v1, v2);
            Assert.Equal(v2, v3);

            cache.Verify(c => c.Set(KEY, v1, CacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Get<DateTime>(KEY, SerializationFormat.Null), Times.Exactly(3));
        }

        [Fact]
        public void GetData_CustomExpiration()
        {
            var cache = new Mock<CacheMock>() { CallBase = true };

            var expiration = TimeSpan.FromMinutes(1);
            var proxy = new CacheProxy<DateTime>(() => DateTime.Now, cache: cache.Object, key: KEY, expiration: expiration);

            var v1 = proxy.GetData();
            Thread.Sleep(50);
            var v2 = proxy.GetData();
            Thread.Sleep(50);
            var v3 = proxy.GetData();

            Assert.Equal(v1, v2);
            Assert.Equal(v2, v3);

            cache.Verify(c => c.Set(KEY, v1, expiration, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Get<DateTime>(KEY, SerializationFormat.Null), Times.Exactly(3));
        }

        [Fact]
        public void GetData_Multithreaded()
        {
            var cache = new Mock<CacheMock>() { CallBase = true };

            var proxy = new CacheProxy<DateTime>(() => DateTime.Now, cache: cache.Object, key: KEY);

            DateTime v1 = default(DateTime);
            DateTime v2 = default(DateTime);
            DateTime v3 = default(DateTime);
            Parallel.Invoke(
                () => { v1 = proxy.GetData(); },
                () => { v2 = proxy.GetData(); },
                () => { v3 = proxy.GetData(); });

            Assert.Equal(v1, v2);
            Assert.Equal(v2, v3);

            cache.Verify(c => c.Set(KEY, v1, CacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Get<DateTime>(KEY, SerializationFormat.Null), Times.Exactly(3));
        }

        [Fact]
        public void GetData_Clear()
        {
            var cache = new Mock<CacheMock>() { CallBase = true };

            var proxy = new CacheProxy<DateTime>(() => DateTime.Now, cache: cache.Object, key: KEY);

            var v1 = proxy.GetData();
            Thread.Sleep(50);
            var v2 = proxy.GetData();
            
            proxy.Clear();

            Thread.Sleep(50);
            var v3 = proxy.GetData();

            Assert.Equal(v1, v2);
            Assert.NotEqual(v2, v3);

            cache.Verify(c => c.Set(KEY, v1, CacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Set(KEY, v3, CacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Get<DateTime>(KEY, SerializationFormat.Null), Times.Exactly(3));
        }
    }
}