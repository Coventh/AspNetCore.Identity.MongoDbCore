# AspNetCore.Identity.MongoDbCore

A MongoDb UserStore and RoleStore adapter for Microsoft.AspNetCore.Identity 2.0 and 3.1.
Allows you to use MongoDb instead of SQL server with Microsoft.AspNetCore.Identity 2.0 and 3.1.

# User and Role Entities

Your user and role entities must inherit from `MongoIdentityUser<TKey>` and `MongoIdentityRole<TKey>` in a way similar to the `IdentityUser<TKey>` and the `IdentityRole<TKey>` in `Microsoft.AspNetCore.Identity`, where `TKey` is the type of the primary key of your document.

Here is an example:

```csharp

public class ApplicationUser : MongoIdentityUser<Guid>
{
	public ApplicationUser() : base()
	{
	}

	public ApplicationUser(string userName, string email) : base(userName, email)
	{
	}
}

public class ApplicationRole : MongoIdentityRole<Guid>
{
	public ApplicationRole() : base()
	{
	}

	public ApplicationRole(string roleName) : base(roleName)
	{
	}
}
```

#### Id Fields

The `Id` field is automatically set at instantiation, this also applies to users inheriting from `MongoIdentityUser<int>`, where a random integer is assigned to the `Id`. It is however not advised to rely on such random mechanism to set the primary key of your document. Using documents inheriting from `MongoIdentityRole` and `MongoIdentityUser`, which both use the `Guid` type for primary keys, is recommended. MongoDB ObjectIds can optionally be used in lieu of GUIDs by passing a key type of `MongoDB.Bson.ObjectId`, e.g. `public class ApplicationUser : MongoIdentityUser<ObjectId>`.

#### Collection Names

MongoDB collection names are set to the plural camel case version of the entity class name, e.g. `ApplicationUser` becomes `applicationUsers`. To override this behavior apply the `CollectionName` attribute from the `MongoDbGenericRepository` nuget package:

```csharp
using MongoDbGenericRepository.Attributes;

namespace App.Entities
{
    // Name this collection Users
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
	...
```

# Configuration

To add the stores, you can use the `IdentityBuilder` extension like so:

```csharp
services.AddIdentity<ApplicationUser, ApplicationRole>()
	.AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
	(
		"mongodb://localhost:27017",
		"MongoDbTests"
	)
	.AddDefaultTokenProviders();
```

```csharp
var mongoDbContext = new MongoDbContext("mongodb://localhost:27017", "MongoDbTests");
services.AddIdentity<ApplicationUser, ApplicationRole>()
	.AddMongoDbStores<IMongoDbContext>(mongoDbContext)
	.AddDefaultTokenProviders();
// Use the mongoDbContext for other things.
```

You can also use the more explicit type declaration:

```csharp
var mongoDbContext = new MongoDbContext("mongodb://localhost:27017", "MongoDbTests");
services.AddIdentity<ApplicationUser, ApplicationRole>()
	.AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(mongoDbContext)
	.AddDefaultTokenProviders();
// Use the mongoDbContext for other things.
```

Alternatively a full configuration can be done by populating a `MongoDbIdentityConfiguration` object, which can have an `IdentityOptionsAction` property set to an action you want to perform against the `IdentityOptions` (`Action<IdentityOptions>`).

The `MongoDbSettings` object is used to set MongoDb Settings using the `ConnectionString` and the `DatabaseName` properties.

```csharp
var mongoDbIdentityConfiguration = new MongoDbIdentityConfiguration
{
	MongoDbSettings = new MongoDbSettings
	{
		ConnectionString = "mongodb://localhost:27017",
		DatabaseName = "MongoDbTests"
	},
	IdentityOptionsAction = options =>
	{
		options.Password.RequireDigit = false;
		options.Password.RequiredLength = 8;
		options.Password.RequireNonAlphanumeric = false;
		options.Password.RequireUppercase = false;
		options.Password.RequireLowercase = false;

		// Lockout settings
		options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
		options.Lockout.MaxFailedAccessAttempts = 10;

		// ApplicationUser settings
		options.User.RequireUniqueEmail = true;
		options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@.-_";
	}
};
services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, Guid>(mongoDbIdentityConfiguration)
        .AddDefaultTokenProviders();
```

# Running the tests

To run the tests, you need a local MongoDb server in default configuration (listening to `localhost:27017`).
Create a database named MongoDbTests for the tests to run.
