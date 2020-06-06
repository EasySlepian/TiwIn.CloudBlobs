//-----------------------------------------------------------------------
// <copyright file="AzBlobRef_DeleteIfExistsAsync_Should.cs" company="TiwIn">
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

    [Guid("d40b094b-d778-46dd-9ac7-aeb5a427544e")]
    public sealed class AzBlobRef_DeleteIfExistsAsync_Should : AzBlobRefTestBase
    {
        [Fact]
        public async Task ReturnTrueIfDeleted()
        {
            var blob = TestContainer.GetBlobClient("test-blob.txt");
            await "some text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite: true));

            var blobRef = CreateBlobRef(blob.Name, options =>
            {
                options.Permissions = BlobPermissions.Delete;
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            Assert.True(await blob.ExistsAsync());
            Assert.True(await blobRef.DeleteIfExistsAsync());
            Assert.False(await blob.ExistsAsync());
        }

        [Fact]
        public async Task ReturnFalseIfNotExists()
        {

            var blobRef = CreateBlobRef(Guid.NewGuid().ToString(), options =>
            {
                options.Permissions = BlobPermissions.Delete;
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            Assert.False(await blobRef.DeleteIfExistsAsync());
        }
    }
}
