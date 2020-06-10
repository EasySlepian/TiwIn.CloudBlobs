# Cloud Blobs client library for .NET
This library is designed to provide with a generic, vendor agnostic .NET API for Cloud Blobs. 
## Examples
### Working with Azure Blobs
 ```csharp
            // Creates an Azure Store Blobs implementation of store provider
            var provider = AzBlobStoreProvider.Create();

            // Create a Cloud Blob Store object which will be used to issue commands against the storage account 
            var store = provider.GetBlobStore(connectionString);

            // Create the my-container collection
            await store.CreateCollectionIfNotExistsAsync("my-container");

            // Upload blobs to a container
            try
            {
                await store.WriteAllTextAsync("my-container", "my-blob.txt", "Hello world!");
            }
            catch (Exception ex) when(store.IsBlobAlreadyExistsError(ex))
            {
                Console.WriteLine("my-blob.txt already exists.");
            }

            // List all blobs in the container
            await foreach (var blobItem in store.GetBlobsAsync("my-container"))
            {
                Console.WriteLine("\t" + blobItem.BlobName);
            }
```            
