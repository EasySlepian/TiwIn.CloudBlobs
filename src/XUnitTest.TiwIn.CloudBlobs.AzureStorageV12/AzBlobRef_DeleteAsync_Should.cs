//-----------------------------------------------------------------------
// <copyright file="AzBlobRef_DeleteAsync_Should.cs" company="TiwIn">
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

    [Guid("a2128d57-8a27-40db-944d-9b9f743d55de")]
    public class AzBlobRef_DeleteAsync_Should : AzBlobRefTestBase
    {
        [Fact]
        public async Task DeleteExistingIfPermitted()
        {
            var blob = TestContainer.GetBlobClient("test-blob.txt");
            await "some text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite: true));

            var blobRef = CreateBlobRef(blob.Name, options =>
            {
                options.Permissions = BlobPermissions.Delete;
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            Assert.True(await blob.ExistsAsync());
            await blobRef.DeleteAsync();
            Assert.False(await blob.ExistsAsync());
        }

        [Fact]
        public async Task ThrowIfNotExists()
        {
            var blob = TestContainer.GetBlobClient(Guid.NewGuid().ToString());

            var blobRef = CreateBlobRef(blob.Name, options =>
            {
                options.Permissions = BlobPermissions.All;
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            await AssertExtensions.ThrowsAsync(blobRef.IsBlobNotFoundError, () =>
                blobRef.DeleteAsync());
        }

        [Fact]
        public async Task ThrowIfNotPermitted()
        {
            var blob = TestContainer.GetBlobClient("test-blob.txt");
            await "some text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite: true));

            var blobRef = CreateBlobRef(blob.Name, options =>
            {
                options.Permissions = BlobPermissions.Read;
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            await AssertExtensions.ThrowsAsync(blobRef.IsAuthorizationPermissionMismatchError, () =>
                blobRef.DeleteAsync());
        }
    }
}
