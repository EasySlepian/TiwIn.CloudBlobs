//-----------------------------------------------------------------------
// <copyright file="AzBlobStore_DeleteCollectionIfExistsAsync_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Xunit;

    [Guid("b3eb9ab5-c13b-4a67-972f-fdd4d625c14a")]
    public class AzBlobStore_DeleteCollectionIfExistsAsync_Should : AzBlobStoreTest
    {
        [Fact]
        public async Task ReturnTrueIfDeleted()
        {
            var containerName = $"{GetType().GUID}-{DateTime.UtcNow.TimeOfDay.Ticks}";
            await BlobServiceClient.CreateBlobContainerAsync(containerName);
            var container = BlobServiceClient.GetBlobContainerClient(containerName);
            Assert.True(await container.ExistsAsync());
            Assert.True(await Store.DeleteCollectionIfExistsAsync(containerName));
            Assert.False(await container.ExistsAsync());
        }

        [Fact]
        public async Task ReturnFalseIfNotExists()
        {
            var containerName = $"{GetType().GUID}-{DateTime.UtcNow.TimeOfDay.Ticks/2}";
            var container = BlobServiceClient.GetBlobContainerClient(containerName);
            Assert.False(await container.ExistsAsync());
            Assert.False(await Store.DeleteCollectionIfExistsAsync(containerName));
        }
    }
}
