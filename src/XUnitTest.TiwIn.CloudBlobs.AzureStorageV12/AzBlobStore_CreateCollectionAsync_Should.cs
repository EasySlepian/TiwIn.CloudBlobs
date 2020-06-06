//-----------------------------------------------------------------------
// <copyright file="AzBlobStore_CreateCollectionAsync_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Xunit;

    [Guid("3165378c-03b4-48bf-8d7c-f90674a8c502")]
    public sealed class AzBlobStore_CreateCollectionAsync_Should : AzBlobStoreTest
    {
        [Fact]
        public async Task CreateIfNotExists()
        {
            var containerName = $"{GetType().GUID}-{DateTime.UtcNow.TimeOfDay.Ticks}";
            var container = BlobServiceClient.GetBlobContainerClient(containerName);
            if (await container.DeleteIfExistsAsync())
            {
                await Task.Delay(TimeSpan.FromSeconds(15));
            };

            try
            {
                Assert.False(await container.ExistsAsync());
                await Store.CreateCollectionAsync(containerName);
                Assert.True(await container.ExistsAsync());
            }
            finally
            {
                await container.DeleteIfExistsAsync();
            }
        }


        [Fact]
        public async Task ThrowIfAlreadyExists()
        {
            await AssertExtensions.ThrowsAsync(Store.IsCollectionAlreadyExistsError,()=>
                Store.CreateCollectionAsync(TestContainerName));
        }
    }
}
