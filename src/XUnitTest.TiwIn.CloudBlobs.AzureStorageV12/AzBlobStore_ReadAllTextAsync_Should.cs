//-----------------------------------------------------------------------
// <copyright file="AzBlobStore_ReadAllTextAsync_Should.cs" company="TiwIn">
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

    [Guid("8132ead5-8830-4bbb-90e0-f18a0dbce004")]
    public class AzBlobStore_ReadAllTextAsync_Should : AzBlobStoreTest
    {
        [Fact]
        public async Task DownloadText()
        {
            var blob = TestContainer.GetBlobClient("download-test.txt");
            await "This is a test".ProcessAsStreamAsync(async stream =>
            {
                await blob.UploadAsync(stream, overwrite: true);
                var actual = await Store.ReadAllTextAsync(TestContainerName, "download-test.txt");
                Assert.Equal("This is a test", actual);
            });

        }


        [Fact]
        public async Task ThrowBlobNotFound()
        {
            await AssertExtensions.ThrowsAsync(
                Store.IsBlobNotFoundError,
                () => Store.ReadAllTextAsync(TestContainerName, Guid.NewGuid().ToString()));
        }


        [Fact]
        public async Task ThrowCollectionNotFound()
        {
            await AssertExtensions.ThrowsAsync(
                Store.IsCollectionNotFoundError,
                () => Store.ReadAllTextAsync(Guid.NewGuid().ToString("N"), "myObject.bin"));
        }

    }
}
