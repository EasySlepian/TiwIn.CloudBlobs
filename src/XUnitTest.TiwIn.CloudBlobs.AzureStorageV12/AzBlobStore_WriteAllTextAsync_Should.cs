//-----------------------------------------------------------------------
// <copyright file="AzBlobStore_WriteAllTextAsync_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Extensions;
    using Xunit;

    [Guid("3886dd78-8bb9-49f3-a61c-aef30baec3c3")]
    public sealed class AzBlobStore_WriteAllTextAsync_Should : AzBlobStoreTest
    {
        [Fact]
        public async Task UploadText()
        {
            var blob = TestContainer.GetBlobClient("upload.tst");
            await blob.DeleteIfExistsAsync();
            await Store.WriteAllTextAsync(
                TestContainerName,
                blob.Name, 
                "This is a test");
            var actual = await blob.ReadAllTextAsync();
            Assert.Equal("This is a test", actual);
        }

        [Fact]
        public async Task ThrowCollectionNotFound()
        {
            await AssertExtensions.ThrowsAsync(
                Store.IsCollectionNotFoundError,
                () => Store.WriteAllTextAsync(
                    Guid.NewGuid().ToString(),
                    "AnyBlob.txt",
                    "This is a test"));
        }
    }
}
