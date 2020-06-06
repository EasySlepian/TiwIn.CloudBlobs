//-----------------------------------------------------------------------
// <copyright file="AzBlobStore_ReadAllBytesAsync_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using TiwIn.Extensions;
    using Xunit;

    [Guid("fe51e297-b1e3-401f-96e5-8fda1eea8779")]
    public class AzBlobStore_ReadAllBytesAsync_Should : AzBlobStoreTest
    {
        [Fact]
        public async Task DownloadBytes()
        {
            var blob = TestContainer.GetBlobClient("download-test.txt");
            await "This is a test".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite: true));

            var bytes = await Store.ReadAllBytesAsync(TestContainerName, "download-test.txt");
            var actual = Encoding.UTF8.GetString(bytes);
            Assert.Equal("This is a test", actual);
        }


        [Fact]
        public async Task ThrowBlobNotFound()
        {
            await AssertExtensions.ThrowsAsync(
                Store.IsBlobNotFoundError,
                () => Store.ReadAllBytesAsync(TestContainerName, Guid.NewGuid().ToString()));
        }


        [Fact]
        public async Task ThrowCollectionNotFound()
        {
            await AssertExtensions.ThrowsAsync(
                Store.IsCollectionNotFoundError,
                () => Store.ReadAllBytesAsync(Guid.NewGuid().ToString("N"), "myObject.bin"));
        }
    }
}
