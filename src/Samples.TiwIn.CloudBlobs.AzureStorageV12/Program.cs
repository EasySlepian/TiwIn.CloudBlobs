//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Threading.Tasks;

    class Program
    {
        static async Task<int> Main()
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            if (String.IsNullOrWhiteSpace(connectionString))
            {
                await Console.Error.WriteLineAsync($"Missing environment variables value. Key: ConnectionString");
                return 1;
            }

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

            return 0;
        }
    }
}
