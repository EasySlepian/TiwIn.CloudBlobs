//-----------------------------------------------------------------------
// <copyright file="AzBlobRef_ReadAllBytesAsync_Should.cs" company="TiwIn">
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

    [Guid("23ef0f34-62bf-47af-af86-2441599e8b07")]
    public class AzBlobRef_ReadAllBytesAsync_Should : AzBlobRefTestBase
    {
        [Fact]
        public async Task DownloadBytes()
        {
            var blob = TestContainer.GetBlobClient("test-blob.txt");
            await "some text".ProcessAsStreamAsync(s => blob.UploadAsync(s, overwrite: true));

            var blobRef = CreateBlobRef(blob.Name, options =>
            {
                options.Permissions = BlobPermissions.Read;
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            await "this is a test".ProcessAsStreamAsync(stream => TestContainer.GetBlobClient(blob.Name).UploadAsync(stream, overwrite: true));

            var bytes = await blobRef.ReadAllBytesAsync();
            var actual = Encoding.UTF8.GetString(bytes);
            Assert.Equal("this is a test", actual);
        }


        [Fact]
        public async Task ThrowBlobNotFound()
        {
            var blobRef = CreateBlobRef(Guid.NewGuid().ToString(), options =>
            {
                options.Permissions = BlobPermissions.Read;
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });
            await AssertExtensions.ThrowsAsync(
                blobRef.IsBlobNotFoundError,
                () => blobRef.ReadAllBytesAsync());
        }

    }
}
