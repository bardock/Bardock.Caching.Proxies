using System;
using System.Threading;
using Moq;
using Sixeyed.Caching.Serialization;
using Xunit;

namespace Bardock.Caching.Proxies.Tests
{
    public class DeferredCacheProxyCollectionTest
    {
        private const string KEY = "KEY";

        [Fact]
        public void GetData_DefaultExpiration()
        {
            var cache = new Mock<CacheMock>() { CallBase = true };

            var proxy = new DeferredCacheProxyCollection<DateTime>(cache.Object, KEY);

            var v1_1 = proxy.GetData(() => DateTime.Now, @params: 1);
            Thread.Sleep(50);
            var v2_1 = proxy.GetData(() => DateTime.Now, @params: 2);
            Thread.Sleep(50);
            var v1_2 = proxy.GetData(() => DateTime.Now, @params: 1);

            Assert.Equal(v1_1, v1_2);
            Assert.NotEqual(v1_1, v2_1);

            cache.Verify(c => c.Set(KEY + "_1", v1_1, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Set(KEY + "_2", v2_1, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Get<DateTime>(KEY + "_1", SerializationFormat.Null), Times.Exactly(2));
            cache.Verify(c => c.Get<DateTime>(KEY + "_2", SerializationFormat.Null), Times.Once);
        }

        [Fact]
        public void GetData_Clear()
        {
            var cache = new Mock<CacheMock>() { CallBase = true };

            var proxy = new DeferredCacheProxyCollection<DateTime>(cache.Object, KEY);

            var v1_1 = proxy.GetData(() => DateTime.Now, @params: 1);
            Thread.Sleep(50);
            var v2_1 = proxy.GetData(() => DateTime.Now, @params: 2);
            Thread.Sleep(50);
            var v1_2 = proxy.GetData(() => DateTime.Now, @params: 1);
            proxy.Clear(1);
            Thread.Sleep(50);
            var v1_3 = proxy.GetData(() => DateTime.Now, @params: 1);
            var v2_2 = proxy.GetData(() => DateTime.Now, @params: 2);

            Assert.Equal(v1_1, v1_2);
            Assert.NotEqual(v1_1, v1_3);
            Assert.Equal(v2_1, v2_2);

            cache.Verify(c => c.Set(KEY + "_1", v1_1, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Set(KEY + "_2", v2_1, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Set(KEY + "_1", v1_2, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Get<DateTime>(KEY + "_1", SerializationFormat.Null), Times.Exactly(3));
            cache.Verify(c => c.Get<DateTime>(KEY + "_2", SerializationFormat.Null), Times.Exactly(2));
            cache.Verify(c => c.Remove(KEY + "_1"), Times.Once);
        }

        [Fact]
        public void GetData_ClearAll()
        {
            var cache = new Mock<CacheMock>() { CallBase = true };

            var proxy = new DeferredCacheProxyCollection<DateTime>(cache.Object, KEY);

            var v1_1 = proxy.GetData(() => DateTime.Now, @params: 1);
            Thread.Sleep(50);
            var v2_1 = proxy.GetData(() => DateTime.Now, @params: 2);
            Thread.Sleep(50);
            var v1_2 = proxy.GetData(() => DateTime.Now, @params: 1);
            proxy.ClearAll();
            Thread.Sleep(50);
            var v1_3 = proxy.GetData(() => DateTime.Now, @params: 1);
            var v2_2 = proxy.GetData(() => DateTime.Now, @params: 2);

            Assert.Equal(v1_1, v1_2);
            Assert.NotEqual(v1_1, v1_3);
            Assert.NotEqual(v2_1, v2_2);

            cache.Verify(c => c.Set(KEY + "_1", v1_1, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Set(KEY + "_2", v2_1, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Set(KEY + "_1", v1_2, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Set(KEY + "_2", v2_2, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Get<DateTime>(KEY + "_1", SerializationFormat.Null), Times.Exactly(3));
            cache.Verify(c => c.Get<DateTime>(KEY + "_2", SerializationFormat.Null), Times.Exactly(2));
            cache.Verify(c => c.RemoveAll(KEY), Times.Once);
        }
    }
}