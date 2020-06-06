//-----------------------------------------------------------------------
// <copyright file="AzBlobStore_DeleteBlobAsync_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using TiwIn.Extensions;
    using Xunit;

    [Guid("dad10a45-a122-469e-b307-f5e75b0e9c45")]
    public class AzBlobStore_DeleteBlobAsync_Should : AzBlobStoreTest
    {
        [Fact]
        public async Task DeleteExisting()
        {
            var blob = TestContainer.GetBlobClient("test-blob.txt");
            await "some text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite: true));
            Assert.True(await blob.ExistsAsync());
            await Store.DeleteBlobAsync(TestContainerName, blob.Name);
            Assert.False(await blob.ExistsAsync());
        }

        [Fact]
        public async Task ThrowIfNotExists()
        {
            var blob = TestContainer.GetBlobClient(Guid.NewGuid().ToString());
            await AssertExtensions.ThrowsAsync(Store.IsBlobNotFoundError, () => 
                Store.DeleteBlobAsync(TestContainerName, blob.Name));
        }
    }
}
