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

            

            return 0;
        }
    }
}
