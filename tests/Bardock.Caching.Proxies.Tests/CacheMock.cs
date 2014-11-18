using System;
using System.Collections.Generic;
using System.Linq;
using Sixeyed.Caching;

namespace Bardock.Caching.Proxies.Tests
{
    public class CacheMock : ICache
    {
        private Dictionary<string, object> _storage;

        public CacheMock()
        {
            Reset();
        }

        public void Reset()
        {
            _storage = new Dictionary<string, object>();
        }

        public CacheType CacheType
        {
            get { throw new NotImplementedException(); }
        }

        public virtual bool Exists(string key)
        {
            return _storage.ContainsKey(key);
        }

        public virtual T Get<T>(string key, Sixeyed.Caching.Serialization.SerializationFormat serializationFormat = Sixeyed.Caching.Serialization.SerializationFormat.Null)
        {
            return (T)Get(typeof(T), key, serializationFormat);
        }

        public virtual object Get(Type type, string key, Sixeyed.Caching.Serialization.SerializationFormat serializationFormat = Sixeyed.Caching.Serialization.SerializationFormat.Null)
        {
            if (!Exists(key))
                throw new CacheKeyNotFoundException();
            return _storage[key];
        }

        public virtual Dictionary<string, T> GetAll<T>(Sixeyed.Caching.Serialization.SerializationFormat serializationFormat = Sixeyed.Caching.Serialization.SerializationFormat.Null)
        {
            throw new NotImplementedException();
        }

        public virtual Dictionary<string, object> GetAll(Type type, Sixeyed.Caching.Serialization.SerializationFormat serializationFormat = Sixeyed.Caching.Serialization.SerializationFormat.Null)
        {
            throw new NotImplementedException();
        }

        public virtual void Initialise()
        {
        }

        public virtual void Remove(string key)
        {
            _storage.Remove(key);
        }

        public virtual void RemoveAll(string keyPrefix)
        {
            if (keyPrefix == null)
            {
                Reset();
            }
            else
            {
                _storage = _storage
                    .Where(x => !x.Key.StartsWith(keyPrefix))
                    .ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public virtual void RemoveAll()
        {
            RemoveAll(keyPrefix: null);
        }

        public virtual Sixeyed.Caching.Serialization.Serializer Serializer
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual void Set(string key, object value, TimeSpan validFor, Sixeyed.Caching.Serialization.SerializationFormat serializationFormat = Sixeyed.Caching.Serialization.SerializationFormat.Null)
        {
            _storage[key] = value;
        }

        public virtual void Set(string key, object value, DateTime expiresAt, Sixeyed.Caching.Serialization.SerializationFormat serializationFormat = Sixeyed.Caching.Serialization.SerializationFormat.Null)
        {
            _storage[key] = value;
        }

        public virtual void Set(string key, object value, Sixeyed.Caching.Serialization.SerializationFormat serializationFormat = Sixeyed.Caching.Serialization.SerializationFormat.Null)
        {
            _storage[key] = value;
        }
    }
}