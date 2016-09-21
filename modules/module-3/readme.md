# Module 3: Building Connected Apps with Azure Mobile Apps
**Objective**: Build your first connected mobile app with Azure Mobile Apps & Xamarin.

##### Prerequisites
Ensure you have the following software installed:

* Visual Studio 2015 Community Edition (or higher) or Xamarin Studio Community Edition (or higher)
* [Xamarin](xamarin.com/download)
* **Optional**: [SQLite for Universal Windows Platform](https://visualstudiogallery.msdn.microsoft.com/4913e7d5-96c9-4dde-a1a1-69820d615936) - Required for using Azure Mobile Apps in Universal Windows Platform (UWP) applications.

This module does use Azure, so before beginning ensure that you have started a [free trial (with $200 in credits)](https://azure.microsoft.com/en-us/offers/ms-azr-0044p/) or have an existing Azure account.

Download the starter code for this module to begin, or continue working with the completed code from Module 2.

### Module Instructions
This module walks you through the basics of building connected mobile apps with Azure Mobile Apps and Xamarin, from configuring a backend to enabling online/offline synchronization in our mobile apps.

##### 1. Create a new Azure Mobile App.
##### Create a Resource Group
To begin, visit [portal.azure.com(https://portal.azure.com). The Azure Portal provides a central location for creating new cloud services, from mobile backends to big-data analytics and machine learning to identity with Azure Active Directory. The portal opens up on a page named the **Dashboard**. Think of this as a "home screen" for all of your Azure services. For easy access, you can create shortcuts on the dashboard to your services. On the left-hand side of the screen is a sidebar containing links to various cloud services, as well as a `New` button to allow us to create new services.

To help organize resources (such as our mobile app backend), we can take advantage of the **Resource Group** concept in Azure. Think of a resource group as a glorified folder, where we can place cloud resources that should be grouped together in a single directory for easy access and management. It's common to create a new resource group for each mobile application you build.

On the left-hand side of the portal, click the `New` button. A new **blade**, or navigation window, will appear that lists services in the `Azure Marketplace`. In the search bar, type `Resource Group` and hit `Enter`. In the list of results that appear, click `Resource group`.

[create-new-resource-group]

The `Resource group` blade will appear detailing what a resource group is, along with the publisher and a link to documentation. Click the `Create` button. A new blade will appear for us to create a new resource group. For `Resource group name`, enter a name, such as "SpentApp". Select a subscription (though generally the default) is correct. Finally, select a `Resource group location`. This is where all of our services will lie, so it's important that you pick the location that's closest to your customers. For testing, you should typically select the location that is closest to you. Click `Pin to Dashboard` to create a shortcut to this group from our Azure Dashboard, and then click `Create`.

[create-empty-resource-group]

A resource group will be created, and you should be navigated to a blade containing your newly-created `Resource Group`.

[resource-group]

##### Create an Azure Mobile App
Now that we have a cloud container for our services, let's create an **Azure Mobile App*. Azure Mobile Apps offer a highly scalable, globally available mobile development platform that brings a rich set of capabilities to mobile developers, such as the ability to store data in the cloud, perform user authentication (local, social, and enterprise), as well as send push notifications and create custom API endpoints. They have some amazing other features, such as automatic online/offline synchronization, automatic scaling, staging environments, continuous deployment, and virtual networking.

To create a new Azure Mobile App, click the `Add` button at the top of the resource group you just created and search for `Mobile Apps`.

[create-azure-mobile-app]

Several different options appear in the search results. The two most important to us are `Mobile App` and `Mobile Apps Quickstart`. For production applications, you will want to create a `Mobile App`. This provides the full functionality of everything Azure Mobile Apps has to offer, including data storage with SQLServer and the ability to build backends in Node.js or .NET. For testing applications and getting started quickly, we can use the `Mobile Apps Quickstart`, which creates a Node.js backend with a backing SQLite data store that doesn't require any additional configuration.

> For a more detailed look at Azure Mobile Apps, including how to build .NET backends for mobile apps, check out my "Learn Azure" course on GitHub.

Click on `Mobile Apps Quickstart`, followed by `Create`. The `Mobile Apps Quickstart` blade will appear. For `App name`, enter an application name. Note that this value must be unique, as it is a URL. For `App Service Plan / Location`, select the current configuration, and click `Create New`. A new blade will appear named `App Service plan`. Enter a name for your plan, as well as a location, and select a `Pricing Tier`. Any pricing tier will work great (including `Free`), but my recommendation is to go with `Basic`. For production applications, you should ship on at least `Standard` tier. This brings tons of awesome features that you are going to want in a mobile backend, including staging slots for testing out backend changes, automatic scaling up to 10 instances, daily backups, and georeplication for speedier backends. 

Select the plan that works for you, and click the `Create` button. A **deployment** will begin. 

[create-new-mobile-app]

You can track progress of the deployment by clicking the notification bell in the upper-righthand corner. Note that it could take anywhere from 2-5 minutes for our mobile app to deploy, as lots of things are happening behind the scenes (deployment of a website, configuration of a database, etc.).

[notification]

When the deployment succeeds, go back to your resource group by clicking `Resource groups` followed by your resource group on the sidebar. Two items have been deployed to your resource group, an App Service plan and an App Service. 

##### 2. Configure Azure Easy Tables.
Click on the App Service to view details for your Azure Mobile App. This blade is the main location for configuring Azure Mobile App settings, such as data storage, push notifications, and authentication. We can also monitor usage of our mobile app and investigate failures with our backend.

[Azure-mobile-app]

In the search bar, search for `Easy Tables`. Easy Tables are a feature of `Azure Mobile Apps` that allow us to create a backend without writing any code. We can supply a table schema, and Easy Tables will automatically generate an API endpoint and handle all data storage for us.

> Easy Tables are great for storing data that is not relational. If your mobile app requires relational data, your best best is to create a .NET Azure Mobile App. To learn how, visit my "Learn Azure" course.

To create a table, click the `Add` button. For the table `Name`, enter `Expense`. If you have authentication configured for your `Mobile App`, you can also manage permissions. For this workshop, let's allow create-read-update-delete (CRUD) access to everyone by leaving the default `Allow anonymous access` permission. After the table is created, click the `Expense` table. From this blade, we can view existing data, update table permissions, add custom scripts to our API endpoint, as well as manage our table schema. Click `Manage schema`, followed by `Add a column`.

[manage-schema]

For `Column name`, enter `company`. For `Data type`, select `String`. Repeat this process for the following properties in our `Expense` model:

 * `description`: String
 * `amount`: String
 * `receipt`: String
 * `date` : Date

[schema]

We are now done configuring our Azure Mobile App no-code backend with Easy Tables!

##### 3. Connect Spent to the cloud.
Now that we have a functioning backend, let's connect our Spent to the cloud. 

If you noticed earlier, we had several additional columns that were automatically added for us when we created an Easy Table. These columns are used to help Azure Mobile Apps perform offline synchronization and conflict resolution. Let's create a new class named `EntityData` in our models folder to mirror these columns.

```csharp

using System;

using Microsoft.WindowsAzure.MobileServices;

namespace Spent
{
	public class EntityData
	{
		public EntityData()
		{
			Id = Guid.NewGuid().ToString();
		}

		public string Id { get; set; }

		[CreatedAt]
		public DateTimeOffset CreatedAt { get; set; }

		[UpdatedAt]
		public DateTimeOffset UpdatedAt { get; set; }

		[Version]
		public string AzureVersion { get; set; }
	}
}
```

Next, update our `Expense` model to subclass `EntityData`. All models that we want to connect to the cloud should have `EntityData` as their base class.

Now that our models are in shape, right-click `Services`, and add a new blank C# class named `AzureDataService`. We will be implementing the `IDataService` interface. This is great, as it requires minimal code changes throughout the app to support our new cloud data storage service. Implement the `IDataService` interface, and add a method named `Initialize`.

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
...
public class AzureDataService : IDataService
{
	public AzureDataService()
	{

	}

	async Task Initialize()
	{

	}

	public async Task AddExpenseAsync(Expense ex)
	{
		throw new NotImplementedException();
	}

	public async Task<IEnumerable<Expense>> GetExpensesAsync()
	{
		throw new NotImplementedException();
	}
}
```

We will be implementing this service using the **Azure Mobile Client SDK**. This SDK is extremely powerful; lots of "magic" behind the scenes in managed for us. We don't have to worry about forming HTTP requests, configuring authentication headers, or even caching our data - the SDK handles that for us. One of my major complaints about other cloud SDKs is they feel like they were written in another language and ported over. The Azure Mobile Client SDK was written by .NET developers for .NET developers, so the API surface feels natural and takes advantage of awesome C# features like async/await. The SDK is distributed via two NuGet packages: the Azure Mobile Client SDK and the Azure Mobile Client SDK SQLiteStore. The SQLiteStore package enables us to have a functioning application even if the user loses connectivity with online/offline synchronization. Both of these packages have been added for you. Let's start by bringing these namespaces into our `AzureDataService`.

```csharp
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
```

Most communication is done through a class named `MobileServiceClient`, which handles communication with our Azure Mobile App. Create a new public property named `MobileService` with type `MobileServiceClient`.

```csharp
public MobileServiceClient MobileService { get; set; }
```

Let's initialize this in the constructor of our `AzureDataService` by passing in the URL of our lab. This code let's the `MobileServiceClient` know the base URL for all API requests that it must form.

```csharp
public AzureDataService()
{
    MobileService = new MobileServiceClient("http://spendapplab.azurewebsites.net/", null);
}
```

Just as we did in `MockDataService`, our `AzureDataService` will need some additional initialization logic. Let's start by adding two class-level fields to track the initialization state, and store a reference to our expenses table.

```csharp
bool isInitialized;
IMobileServiceSyncTable<Expense> expensesTable;
```

Add the following code to the `Initialize` method to configure our local data store.

```csharp
async Task Initialize()
{
	if (isInitialized)
		return;
			
	var store = new MobileServiceSQLiteStore("app.db");
	store.DefineTable<Expense>();
	await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());
	expensesTable = MobileService.GetSyncTable<Expense>();

	isInitialized = true;
}
```

This code checks to see if the `AzureDataService` has already been initialized. If it has not, we create a new `MobileServiceSQLiteStore`, which is just a local database for handling online/offline synchronization. Next, we define the `Expense` table. Finally, we initialize our data store with a default sync handler that manages automatic conflict resolution for us.

Online/offline sychronization requires a little bit of glue for the local and remote data stores to stay in sync. Add a new async method named `SyncExpenses`.

```csharp
async Task SyncExpenses()
{
	try
	{
		await MobileService.SyncContext.PushAsync();
		await expensesTable.PullAsync($"all{typeof(Expense).Name}", expensesTable.CreateQuery());
	}
	catch (Exception)
	{
		System.Diagnostics.Debug.WriteLine("An error syncing occurred. That is OK, as we have offline sync.");
	}
}
```

This code first pushes all local changes to the server if they are not already persisted there. Next, it pulls down all changes on the server that are not available locally on the device. By supplying a query identifier to our `PullAsync` method, we can use incremental sync, so only the latest changes are pulled down; not the entire table. Now we are ready to write the code that adds and retrieves expenses! 

Add the following code to the `AddExpenseAsync` method. We'll first initialize the service, then use the `expensesTable` field we crated earlier to insert an item to the local data store, and finally sync these changes to the server.

```csharp
public async Task AddExpenseAsync(Expense ex)
{
	await Initialize();

	await expensesTable.InsertAsync(ex);
    await SyncExpenses();
}
```

Finally, we need to add code to our `GetExpensesAsync` to retrieve all expenses from both the local and remote data store.

```csharp
public async Task<IEnumerable<Expense>> GetExpensesAsync()
{
	await Initialize();

	await SyncExpenses();

	return await expensesTable.ToEnumerableAsync();
}
```

Because we are using the `DependencyService`, we need to ensure only one dependency is available for `IDataService` so the correct type can be resolved. Remove the attribute from the `MockDataService` and add the following to the top of the `AzureDataService`.

```csharp
using Xamarin.Forms;

[assembly: Dependency(typeof(Spent.AzureDataService))]
```

Run the app, add an item, and you should see that the item is pushed to the cloud data store in our Azure Mobile App.

[screenshot]

Boom! We've now written all the logic for our `AzureDataService`. In just over 50 lines of code, we are saving and retrieving expenses from both our local and cloud data stores, as well as keeping them in sync. Run the app, and you'll be able to add new items and have them appear in the Easy Tables data browser.

[ExpensesApp]