//-----------------------------------------------------------------------
// <copyright file="AzBlobStore_DeleteBlobIfExistsAsync_Should.cs" company="TiwIn">
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

    [Guid("aa245f0c-17ae-47b8-9259-83c1f786e850")]
    public sealed class AzBlobStore_DeleteBlobIfExistsAsync_Should : AzBlobStoreTest
    {
        [Fact]
        public async Task ReturnTrueIfDeleted()
        {
            var blob = TestContainer.GetBlobClient("test-blob.txt");
            await "some text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite: true));
            Assert.True(await blob.ExistsAsync());
            Assert.True(await Store.DeleteBlobIfExistsAsync(TestContainerName, blob.Name));
            Assert.False(await blob.ExistsAsync());
        }

        [Fact]
        public async Task ReturnFalseIfNotExists()
        {
            var blob = TestContainer.GetBlobClient(Guid.NewGuid().ToString());
            Assert.False(await blob.ExistsAsync());
            Assert.False(await Store.DeleteBlobIfExistsAsync(TestContainerName, blob.Name));
        }
    }
}
