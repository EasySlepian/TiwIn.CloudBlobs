//-----------------------------------------------------------------------
// <copyright file="AzBlobStore_CreateCollectionIfNotExistsAsync_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Xunit;

    [Guid("40a2fb2a-aff7-42d0-bccb-846254631267")]
    public sealed class AzBlobStore_CreateCollectionIfNotExistsAsync_Should : AzBlobStoreTest
    {
        [Fact]
        public async Task ReturnTrueIfCreated()
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
                Assert.True(await Store.CreateCollectionIfNotExistsAsync(containerName));
                Assert.True(await container.ExistsAsync());
            }
            finally
            {
                await container.DeleteIfExistsAsync();
            }
        }


        [Fact]
        public async Task ReturnFalseIfNotCreated()
        {
            var containerName = $"{GetType().GUID}-{DateTime.UtcNow.TimeOfDay.Ticks/2}";
            var container = BlobServiceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();

            try
            {
                Assert.True(await container.ExistsAsync());
                Assert.False(await Store.CreateCollectionIfNotExistsAsync(containerName));
                Assert.True(await container.ExistsAsync());
            }
            finally
            {
                await container.DeleteIfExistsAsync();
            }
        }
    }
}
