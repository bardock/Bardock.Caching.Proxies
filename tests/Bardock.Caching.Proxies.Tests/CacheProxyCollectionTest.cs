using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using Sixeyed.Caching.Serialization;
using Xunit;

namespace Bardock.Caching.Proxies.Tests
{
    public class CacheProxyCollectionTest
    {
        private const string KEY = "KEY";

        [Fact]
        public void GetData_DefaultExpiration()
        {
            var cache = new Mock<CacheMock>() { CallBase = true };

            var proxy = new CacheProxyCollection<int, DateTime>(num => DateTime.Now, cache: cache.Object, keyPrefix: KEY);

            var v1_1 = proxy.GetData(1);
            Thread.Sleep(50);
            var v2_1 = proxy.GetData(2);
            Thread.Sleep(50);
            var v1_2 = proxy.GetData(1);

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

            var proxy = new CacheProxyCollection<int, DateTime>(num => DateTime.Now, cache: cache.Object, keyPrefix: KEY);

            var v1_1 = proxy.GetData(1);
            Thread.Sleep(50);
            var v2_1 = proxy.GetData(2);
            Thread.Sleep(50);
            var v1_2 = proxy.GetData(1);
            proxy.Clear(1);
            Thread.Sleep(50);
            var v1_3 = proxy.GetData(1);
            var v2_2 = proxy.GetData(2);

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

            var proxy = new CacheProxyCollection<int, DateTime>(num => DateTime.Now, cache: cache.Object, keyPrefix: KEY);

            var v1_1 = proxy.GetData(1);
            Thread.Sleep(50);
            var v2_1 = proxy.GetData(2);
            Thread.Sleep(50);
            var v1_2 = proxy.GetData(1);
            proxy.ClearAll();
            Thread.Sleep(50);
            var v1_3 = proxy.GetData(1);
            var v2_2 = proxy.GetData(2);

            Assert.Equal(v1_1, v1_2);
            Assert.NotEqual(v1_1, v1_3);
            Assert.NotEqual(v2_1, v2_2);

            cache.Verify(c => c.Set(KEY + "_1", v1_1, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Set(KEY + "_2", v2_1, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Set(KEY + "_1", v1_2, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Set(KEY + "_2", v2_2, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Get<DateTime>(KEY + "_1", SerializationFormat.Null), Times.Exactly(3));
            cache.Verify(c => c.Get<DateTime>(KEY + "_2", SerializationFormat.Null), Times.Exactly(2));
            cache.Verify(c => c.RemoveAll(KEY + "_"), Times.Once);
        }

        [Fact]
        public void GetData_Params_String()
        {
            var cache = new Mock<CacheMock>() { CallBase = true };

            var proxy = new CacheProxyCollection<string, DateTime>(num => DateTime.Now, cache: cache.Object, keyPrefix: KEY);

            var v1 = proxy.GetData("1");
            Thread.Sleep(50);
            var v2 = proxy.GetData("1");

            Assert.Equal(v1, v2);

            cache.Verify(c => c.Set("KEY_1", v1, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Get<DateTime>("KEY_1", SerializationFormat.Null), Times.Exactly(2));
        }

        [Fact]
        public void GetData_Params_Null()
        {
            var cache = new Mock<CacheMock>() { CallBase = true };

            var proxy = new CacheProxyCollection<string, DateTime>(num => DateTime.Now, cache: cache.Object, keyPrefix: KEY);

            var v1 = proxy.GetData(null);
            Thread.Sleep(50);
            var v2 = proxy.GetData(null);

            Assert.Equal(v1, v2);

            cache.Verify(c => c.Set("KEY_null", v1, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Get<DateTime>("KEY_null", SerializationFormat.Null), Times.Exactly(2));
        }

        [Fact]
        public void GetData_Params_Complex()
        {
            var cache = new Mock<CacheMock>() { CallBase = true };

            var proxy = new CacheProxyCollection<Dictionary<string, object>, DateTime>(num => DateTime.Now, cache: cache.Object, keyPrefix: KEY);

            var v1 = proxy.GetData(new Dictionary<string, object> { { "asd", 1 } });
            Thread.Sleep(50);
            var v2 = proxy.GetData(new Dictionary<string, object> { { "asd", 1 } });
            Thread.Sleep(50);
            var v3 = proxy.GetData(new Dictionary<string, object> { { "asd", 2 } });

            Assert.Equal(v1, v2);
            Assert.NotEqual(v1, v3);

            cache.Verify(c => c.Set("KEY_{\"asd\":1}", v1, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Get<DateTime>("KEY_{\"asd\":1}", SerializationFormat.Null), Times.Exactly(2));
            cache.Verify(c => c.Set("KEY_{\"asd\":2}", v3, DeferredCacheProxy<DateTime>.EXPIRATION_DEFAULT, SerializationFormat.Null), Times.Once);
            cache.Verify(c => c.Get<DateTime>("KEY_{\"asd\":2}", SerializationFormat.Null), Times.Once);
        }
    }
}