Bardock.Caching.Proxies
=======================

Provides classes that abstract access to a cache item or collection of cache items.

## Dependencies

* Bardock.Utils
* Newtonsoft.Json
* Sixeyed.Caching-Bardock

## Usage

Install [nuget package](https://www.nuget.org/packages/Bardock.Caching.Proxies/):

	Install-Package Bardock.Caching.Proxies

### CacheProxy

Use a `CacheProxy` in order to abstract access to a cache item

	// Define a proxy for database users caching for 1 second
	var proxy = new CacheProxy<User>(
		dataLoadFunc: () => Db.Users.ToList(), 
		cache: Cache.AspNet, 
		key: "DateNow", 
		expiration: TimeSpan.FromSeconds(1));

    var v1 = proxy.GetData();
	// Executes dataLoadFunc and stores the return value in cache
 
    Thread.Sleep(100);
    var v2 = proxy.GetData();
	// Gets value from cache (v2 is equal than v1)

    Thread.Sleep(1000);
    var v3 = proxy.GetData();
	// Cache item is expired, executes dataLoadFunc again and stores the return value in cache

You can manually set the data (useful when you just created or updated the data and want to store it)

	proxy.SetData(user);

Or clear cached item

	proxy.Clear();

### CacheProxyCollection

Use a `CacheProxyCollection` in order to abstract access to a collection of cache items

	// Define a proxy for database users caching for 1 second
	// Use user ID (int) to identify each item 
	var proxy = new CacheProxyCollection<User, int>(
		dataLoadFunc: id => Db.Users.Find(id), 
		cache: Cache.AspNet, 
		keyPrefix: "DateNow", 
		expiration: TimeSpan.FromSeconds(1));

    var v1a = proxy.GetData(1);
	// Executes dataLoadFunc (finds user with ID == 1) and stores the return value in cache

    var v2a = proxy.GetData(2);
	// Executes dataLoadFunc (finds user with ID == 2) and stores the return value in cache
 
    Thread.Sleep(100);
    var v1b = proxy.GetData(1);
	// Gets value from cache (v1b is equal than v1a)

    Thread.Sleep(1000);
    var v1c = proxy.GetData(1);
	// Cache item is expired, executes dataLoadFunc again and stores the return value in cache

You can manually set the data of a specific item

	proxy.SetData(user, 1);

Or clear cached items

	proxy.Clear(1);
	proxy.ClearAll();

## Running the Unit Tests

1. Download and install [xUnit.net runner for Visual Studio](https://visualstudiogallery.msdn.microsoft.com/463c5987-f82b-46c8-a97e-b1cde42b9099)
2. Now you can use the Test Explorer in Visual Studio