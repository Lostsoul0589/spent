# Module 4: Saving Blobs to Azure Storage
**Objective**: Learn the basics of Azure Storage and Azure Functions, and how they relate to mobile app development.

##### Prerequisites
Ensure you have the following software installed:

* Visual Studio 2015 Community Edition (or higher) or Xamarin Studio Community Edition (or higher)
* [Xamarin](xamarin.com/download)
* **Optional**: [SQLite for Universal Windows Platform](https://visualstudiogallery.msdn.microsoft.com/4913e7d5-96c9-4dde-a1a1-69820d615936) - Required for using Azure Mobile Apps in Universal Windows Platform (UWP) applications.

This module does use Azure, so before beginning ensure that you have started a [free trial (with $200 in credits)](https://azure.microsoft.com/en-us/offers/ms-azr-0044p/) or have an existing Azure account. Completion of Module 3 prior to beginning Module 4 is not explicitly required, but is recommended.

Download the starter code for this module to begin, or continue working with the completed code from Module 3.

### Module Instructions
This module introduces Azure Storage, which we will use to store receipt images in Spent.

##### 1. Create an Azure Storage bucket.
[Azure Storage](https://azure.microsoft.com/en-us/services/storage/) is a service offered by Microsoft that provides scalable cloud storage for unstructured data (i.e. blobs), files, NoSQL tables, and queues. Blob storage is an important concept for mobile developers, as most mobile apps have some sort of data that must be stored outside of a typical table data store (such as an image or file). 

Let's use Azure Storage to store the receipt images that users wish to upload. Just like all Microsoft cloud services, we will can create a new Azure Storage at [portal.azure.com](https://portal.azure.com). Click `New` in the sidebar,  search for `Azure Storage`, select the `Storage account` option, and click `Create`.

The `Create storage account` blade will appear. Configure your Azure Storage account by completing the blade with the following information:

* `Name`: Must be unique, as this is a URL.
* `Deployment model`, select `Resource manager`.
* `Account kind`, select `Blob storage`. This creates a specialized storage account just for blobs that allows us to select more specialized storage options that only apply to blobs.
* `Performance`, select `Standard`.
* `Replication`, select `Read-access geo-redundant storage`. This replicates our storage across multiple regions that aren't close to your primary region. This is ideal where you have users that aren't central to a particular location or region.
* `Access tier`, select `Hot`. `Cold` should be used for storage that's accessed very infrequently, such as backups.
* `Storage service encryption`, select `Disabled`. Highly sensitive or personal documents should `Enable` this feature.
* `Resource group`, select `Use existing`, and select the resource group created in Module 3.
* `Location`, select whatever is closest to you. Typically, this is the same as the value selected in Module 3.

 ![](/modules/module-4/images/create-storage-account.png)

Click `Create` to create a specialized Azure Storage account for blobs. This may take up to a minute or two to deploy.

Once deployed, click the `Access keys` setting in the storage account sidebar. Copy the `Storage account name`, as well as the value for `key1`. Save these for later, as we will use them to connect our app to the storage account.

 ![](/modules/module-4/images/access-keys.png)

##### 2. Create a storage container.
Now that we have a storage account, we need a way to store blobs into our account. Blobs are required to be stored in **containers**, which are just folders for your blobs. Let's create a container. Navigate to your storage account within your resource group created in Module 3. Click on storage account, followed by `+ Container`. The `New container` blade will appear.

 ![](/modules/module-4/images/create-container.png)

Name the container `receipts`, select the `Blob` access type to allow public read access to our blobs, and click `Create`. A new container named `receipts` will appear in the container list, along with a URL to access the container.

 ![](/modules/module-4/images/created-container.png)

##### 3. Store receipts to Azure Storage.
Now that our server-side setup is complete to store blobs, let's update our Spent app to use Azure Storage to store photos of receipts. 

Just like Azure Mobile Apps, Azure Storage has a powerful SDK named the **[Windows Azure Storage SDK](https://www.nuget.org/packages/WindowsAzure.Storage/)**. This will make it super easy for us to upload blobs and other types of data to Azure Storage. If your app revolves around storage, the SDK even extends the powerful offline sync functionality found in the Azure Mobile Client SDK to data stored in Azure Storage. For convienence, this NuGet has already been added to the iOS, Android, and Universal Windows Platform projects.

Open up `NewExpenseViewModel`, and navigate to the `SaveExpenseAsync` method. Right now, we are passing the `Expense` to our `MessagingCenter`, which is subscribed to in our `ExpensesViewModel` for adding new expenses. The problem is that we are merely passing a path to a local image to the view model; ideally, we would pass the image itself for easy uploading. Add a new class-level field for storing the `MediaFile` until the user attempts to save.

```csharp
MediaFile receiptPhoto;
```

Next, set this value in the `AttachReceiptAsync` method.

```csharp
receiptPhoto = photo;
```

Let's modify our `SaveExpenseAsync` method to send a `object[]` with two items in the array - the `Expense` and the `MediaFile`, and send this array via our `MessagingCenter` to the `ExpensesViewModel`.

```csharp
async Task SaveExpenseAsync()
{
	if (IsBusy)
	    return;

	IsBusy = true;

    try
    {
		var expense = new Expense
		{
			Company = Company,
			Description = Description,
			Date = DateTime,
			Amount = Amount
		};

		var expenseData = new object[]
		{
			expense,
			receiptPhoto
		};

		MessagingCenter.Send(this, "AddExpense", expenseData);
		MessagingCenter.Send(this, "Navigate", "ExpensesPage");

		MessagingCenter.Send(this, "AddExpense", expense);
		MessagingCenter.Send(this, "Navigate", "ExpensesPage");
	}
	catch (Exception ex)
	{
        MessagingCenter.Send(this, "Error", ex.Message);
	}
	finally
	{
		IsBusy = false;
	}
}
```

Let's jump over to `ExpensesViewModel`. In the constructor of `ExpensesViewModel`, we subscribe to the message sent from `NewExpenseViewModel` to add expenses to our data store. This is where we will add our code to upload our photo to Azure Storage before saving it off to our data store.

Update the subscription to the message to accept two parameters and cast the objects back to their original format.

```csharp
using Plugin.Media.Abstraction;
...
MessagingCenter.Subscribe<NewExpenseViewModel, object[]>(this, "AddExpense", async (obj, expenseData) =>
{
    var expense = expenseData[0] as Expense;
	var photo = expenseData[1] as MediaFile;
	Expenses.Add(expense);

    // TODO: Upload photo to Azure Storage.

    await DependencyService.Get<IDataService>().AddExpenseAsync(expense);
});
```

We should check to make sure the photo is not null before uploading; the user may not have elected to upload a receipt. In this case, we should not upload to storage. If the photo is not null, we shold upload the image.

```csharp
...
if (photo != null)
{

}
...
```

> Note: There are many ways to use the Windows Azure Storage SDK to upload blobs. The following is one of several different approaches you can take.

To start, we will create a new `CloudStorageAccount` by parsing a connection string to our storage account. It's important to note that you should **not** use connection strings in production applications, only when testing locally. For production applications, you should create an API endpoint that services Shared Access Signature tokens to users on-demand. Not only is this approach more secure, but you can make changes to storage permissions without requiring an update to your app - you would just update the API endpoint. For more information on that approach, visit my "Learn Azure" course.

The connection string is formed by substituting in your `Account Name` and `Account Key` from Step #2 at the appropriate places in the following string.

```csharp
DefaultEndpointsProtocol=https;AccountName={INSERT-ACCOUNT-NAME-HERE};AccountKey={INSERT-ACCOUNT-KEY-HERE}
```

When fully formed, add the following line of codes to create a `CloudStorageAccount` object, and a `CloudBlobClient` for uploading our blobs. (Be sure to insert your own connection string formed with your account name and account key.)

```csharp
using Microsoft.WindowsAzure.Storage;
...
// Connect to the Azure Storage account.
// NOTE: You should use SAS tokens instead of Shared Keys in production applications.
var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=spentappstorage;AccountKey=n/o1HFFL3VqDI9RweXG/xnKvWMFiaq1Giw/htpCESjHILPoBx2VAvf/iAcGDh5D/+0GIaSTT9TT9OxWTIlshYA==");
var blobClient = storageAccount.CreateCloudBlobClient();
```

Next, we will get a reference to our `receipts` container that we created earlier, and create this container if it doesn't already exist.

```csharp
// Create the blob container if it doesn't already exist.
var container = blobClient.GetContainerReference("receipts");
await container.CreateIfNotExistsAsync();
```
It's time to upload our blob! We'll create a new `BlockBlob` with a random name generated from a `Guid`. We'll then convert the `MediaFile` to a `System.IO.Stream`, and upload the image. Finally, we can set the `Expense.Receipt` property to the URL of our uploaded receipt in Azure Storage.

```csharp
// Upload the blob to Azure Storage.
var blockBlob = container.GetBlockBlobReference(Guid.NewGuid().ToString());
await blockBlob.UploadFromStreamAsync(photo.GetStream());
expense.Receipt = blockBlob.Uri.ToString();
```

The subscription to the "AddExpense" message should now look something like the following.

```
MessagingCenter.Subscribe<NewExpenseViewModel, object[]>(this, "AddExpense", async (obj, expenseData) =>
{
	var expense = expenseData[0] as Expense;
	var photo = expenseData[1] as MediaFile;
	Expenses.Add(expense);

	// TODO: Upload photo to Azure Storage.
	if (photo != null)
	{
		// Connect to the Azure Storage account.
		// NOTE: You should use SAS tokens instead of Shared Keys in production applications.
		var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=spentappstorage;AccountKey=n/o1HFFL3VqDI9RweXG/xnKvWMFiaq1Giw/htpCESjHILPoBx2VAvf/iAcGDh5D/+0GIaSTT9TT9OxWTIlshYA==");
		var blobClient = storageAccount.CreateCloudBlobClient();

		// Create the blob container if it doesn't already exist.
		var container = blobClient.GetContainerReference("receipts");
		await container.CreateIfNotExistsAsync();

		// Upload the blob to Azure Storage.
		var blockBlob = container.GetBlockBlobReference(Guid.NewGuid().ToString());
		await blockBlob.UploadFromStreamAsync(photo.GetStream());
		expense.Receipt = blockBlob.Uri.ToString();
	}

	await DependencyService.Get<IDataService>().AddExpenseAsync(expense);
});
```

Great! Now run the app, create a new expense, attach a receipt, and save the photo. If you'd like, open the app up in another simulator, pull down the changes, and see that the image downloads and is shown on the expense detail page.
