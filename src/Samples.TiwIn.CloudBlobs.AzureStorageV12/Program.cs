//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Threading.Tasks;
    using static System.TimeSpan;
    using static CollectionPermissions;

    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            if (String.IsNullOrWhiteSpace(connectionString))
            {
                await Console.Error.WriteLineAsync($"Missing environment variables value. Key: ConnectionString");
                return 1;
            }

            var provider = AzBlobStoreProvider.Create();
            var store = provider.GetBlobStore(connectionString);


            await store.CreateCollectionIfNotExistsAsync("my-container");

            //foreach (var index in Enumerable.Range(0, 10))
            //{
            //    var blobName = $"my-blob-{index}.txt";
            //    await store.WriteAllTextAsync("my-container", blobName,
            //        $"Hello world {index}",
            //        options => options.AddProperty("MyKey", $"MyValue-{index}"));
            //}

            //await foreach (var blobInfo in store.GetBlobsAsync("my-container"))
            //{
            //    var text = await blobInfo.ReadAllTextAsync();
            //    Console.WriteLine($"{blobInfo.BlobName}  ->   {text}");
            //}


            var collectionUrl = store.GetCollectionUri("my-container", options =>
            {
                options.Permissions = List | Read;
                options.Started(FromSeconds(30));
                options.ExpiresAfter(FromHours(2));
            });


            var blobUrlf = store.GetBlobUri("my-container/my-blob-8.txt", options =>
            {
                options.Permissions = BlobPermissions.Read;
                options.ExpiresAfter(TimeSpan.FromMinutes(20));
            });
            

            var collectionRef = provider.GetCollectionRef(collectionUrl);


            await foreach (var blobInfo in collectionRef.GetBlobsAsync())
            {
                var text = await blobInfo.ReadAllTextAsync();
                Console.WriteLine($"{blobInfo.BlobName}  ->   {text}");
            }

            //var blobInfo = await store.GetBlobInfoAsync("my-container", "my-file.txt");
            //var text = await blobInfo.ReadAllTextAsync();

            //Console.WriteLine(text);

            return 0;
        }
    }
}
