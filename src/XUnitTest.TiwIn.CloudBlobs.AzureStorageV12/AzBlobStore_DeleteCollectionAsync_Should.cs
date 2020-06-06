//-----------------------------------------------------------------------
// <copyright file="AzBlobStore_DeleteCollectionAsync_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Xunit;

    [Guid("38d967e1-15e7-480d-9362-bb0e53444f6b")]
    public sealed class AzBlobStore_DeleteCollectionAsync_Should : AzBlobStoreTest
    {
        [Fact]
        public async Task DeleteExisting()
        {
            var containerName = $"{GetType().GUID}-{DateTime.UtcNow.TimeOfDay.Ticks}";
            await BlobServiceClient.CreateBlobContainerAsync(containerName);
            var container = BlobServiceClient.GetBlobContainerClient(containerName);
            Assert.True(await container.ExistsAsync());
            await Store.DeleteCollectionIfExistsAsync(containerName);
            Assert.False(await container.ExistsAsync());
        }

        [Fact]
        public async Task ThrowIfNotExists()
        {
            var containerName = $"{GetType().GUID}-{DateTime.UtcNow.TimeOfDay.Ticks / 2}";
            var container = BlobServiceClient.GetBlobContainerClient(containerName);
            Assert.False(await container.ExistsAsync());
            await AssertExtensions.ThrowsAsync(Store.IsCollectionNotFoundError, ()=> Store.DeleteCollectionAsync(containerName));
        }
    }
}
