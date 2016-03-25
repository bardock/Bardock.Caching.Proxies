Bardock.Caching.Proxies
=======================

Provides classes that abstract access to a cache item or collection of cache items.

## Dependencies

* [Bardock.Utils](https://github.com/bardock/Bardock.Utils)
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
* [Sixeyed.Caching-Bardock](https://github.com/bardock/caching)

## Usage

Install [nuget package](https://www.nuget.org/packages/Bardock.Caching.Proxies/):

	Install-Package Bardock.Caching.Proxies

### CacheProxy

Use a `CacheProxy` in order to abstract access to a cache item

```C#
// Define a proxy for database users and cache them for 1 second
var proxy = new CacheProxy<User>(
	dataLoadFunc: () => Db.Users.ToList(), 
	cache: Cache.AspNet, 
	key: "Users", 
	expiration: TimeSpan.FromSeconds(1));

var users1 = proxy.GetData();
// Executes dataLoadFunc and stores the return value in cache

Thread.Sleep(100);
var users2 = proxy.GetData();
// Gets value from cache (users2 is equal than users1)

Thread.Sleep(1000);
var users3 = proxy.GetData();
// Cache item is expired, executes dataLoadFunc again and stores the return value in cache
```

You can manually set the data (useful when you just created or updated the data and want to store it)

```C#
proxy.SetData(users);
```

Or clear cached item

```C#
proxy.Clear();
```

### CacheProxyCollection

Use a `CacheProxyCollection` in order to abstract access to a collection of cache items

```C#
// Define a proxy for database users and cache them for 1 second
// Use user ID (int) to identify each item 
var proxy = new CacheProxyCollection<int, User>(
	dataLoadFunc: id => Db.Users.Find(id), 
	cache: Cache.AspNet, 
	keyPrefix: "Users", 
	expiration: TimeSpan.FromSeconds(1));

var user1a = proxy.GetData(1);
// Executes dataLoadFunc (finds user with ID == 1) and stores the return value in cache

var user2a = proxy.GetData(2);
// Executes dataLoadFunc (finds user with ID == 2) and stores the return value in cache

Thread.Sleep(100);
var user1b = proxy.GetData(1);
// Gets value from cache (user1b is equal than user1a)

Thread.Sleep(1000);
var user1c = proxy.GetData(1);
// Cache item is expired, executes dataLoadFunc again and stores the return value in cache
```

You can manually set the data of a specific item

```C#
proxy.SetData(1, user);
```

Or clear cached items

```C#
proxy.Clear(1);
proxy.ClearAll();
```

## Running the Unit Tests

1. Download and install [xUnit.net runner for Visual Studio](https://visualstudiogallery.msdn.microsoft.com/463c5987-f82b-46c8-a97e-b1cde42b9099)
2. Now you can use the Test Explorer in Visual Studio