using System;
using System.Threading.Tasks;
using Moq;
using Sixeyed.Caching.Serialization;
using Xunit;

namespace Bardock.Caching.Proxies.Tests
{
    public class DeferredCacheProxyTest
    {
        private const string KEY = "KEY";

        [Fact]
        public void GetData_DefaultExpiration()
        {
            var cache = new Mock<CacheMock>() { CallBase = true };

            var proxy = new DeferredCacheProxy<DateTime>(cache.Object, KEY);

            var value = new DateTime(2014, 1, 2, 3, 4, 5);

            var v1 = proxy.GetData(() => value);
            var v2 = proxy.GetData(() => DateTime.Now);
            var v3 = proxy.GetData(() => DateTime.Now);

            Assert.Equal(value, v1);
            Assert.Equal(v1, v2);
            Assert.Equal(v2, v3);

            cache.Verify(c => c.Set(KEY, value, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Get<DateTime>(KEY, SerializationFormat.Null), Times.Exactly(3));
        }

        [Fact]
        public void GetData_CustomExpiration()
        {
            var cache = new Mock<CacheMock>() { CallBase = true };

            var expiration = TimeSpan.FromMinutes(1);
            var proxy = new DeferredCacheProxy<DateTime>(cache.Object, KEY, expiration);

            var value = new DateTime(2014, 1, 2, 3, 4, 5);

            var v1 = proxy.GetData(() => value);
            var v2 = proxy.GetData(() => DateTime.Now);
            var v3 = proxy.GetData(() => DateTime.Now);

            Assert.Equal(value, v1);
            Assert.Equal(v1, v2);
            Assert.Equal(v2, v3);

            cache.Verify(c => c.Set(KEY, value, expiration, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Get<DateTime>(KEY, SerializationFormat.Null), Times.Exactly(3));
        }

        [Fact]
        public void GetData_Multithreaded()
        {
            var cache = new Mock<CacheMock>() { CallBase = true };

            var proxy = new DeferredCacheProxy<DateTime>(cache.Object, KEY);

            var value = new DateTime(2014, 1, 2, 3, 4, 5);

            DateTime v1 = default(DateTime);
            DateTime v2 = default(DateTime);
            DateTime v3 = default(DateTime);
            Parallel.Invoke(
                () => { v1 = proxy.GetData(() => value); },
                () => { v2 = proxy.GetData(() => value); },
                () => { v3 = proxy.GetData(() => value); });

            Assert.Equal(value, v1);
            Assert.Equal(v1, v2);
            Assert.Equal(v2, v3);

            cache.Verify(c => c.Set(KEY, value, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Get<DateTime>(KEY, SerializationFormat.Null), Times.Exactly(3));
        }

        [Fact]
        public void GetData_Clear()
        {
            var cache = new Mock<CacheMock>() { CallBase = true };

            var proxy = new DeferredCacheProxy<DateTime>(cache.Object, KEY);

            var value = new DateTime(2014, 1, 2, 3, 4, 5);
            var value2 = new DateTime(2014, 6, 7, 8, 9, 10);

            var v1 = proxy.GetData(() => value);
            var v2 = proxy.GetData(() => DateTime.Now);
            proxy.Clear();
            var v3 = proxy.GetData(() => value2);

            Assert.Equal(value, v1);
            Assert.Equal(v1, v2);
            Assert.NotEqual(v2, v3);
            Assert.Equal(value2, v3);

            cache.Verify(c => c.Set(KEY, value, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Set(KEY, value2, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Get<DateTime>(KEY, SerializationFormat.Null), Times.Exactly(3));
        }
    }
}