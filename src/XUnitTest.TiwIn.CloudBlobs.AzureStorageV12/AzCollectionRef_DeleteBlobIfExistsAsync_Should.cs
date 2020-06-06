//-----------------------------------------------------------------------
// <copyright file="AzCollectionRef_DeleteBlobIfExistsAsync_Should.cs" company="TiwIn">
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

    [Guid("9477aff4-10c4-4ef8-8244-e0e546f9cb50")]
    public sealed class AzCollectionRef_DeleteBlobIfExistsAsync_Should : AzCollectionRefTestBase
    {
        [Fact]
        public async Task ReturnTrueIfDeleted()
        {
            var blob = TestContainer.GetBlobClient("test-blob.txt");
            await "some text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite: true));

            var container = CreateCollectionRef(options =>
            {
                options.Permissions = CollectionPermissions.Delete;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            Assert.True(await blob.ExistsAsync());
            Assert.True(await container.DeleteBlobIfExistsAsync(blob.Name));
            Assert.False(await blob.ExistsAsync());
        }

        [Fact]
        public async Task ReturnFalseIfNotExists()
        {
            var container = CreateCollectionRef(options =>
            {
                options.Permissions = CollectionPermissions.Delete;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            Assert.False(await container.DeleteBlobIfExistsAsync(Guid.NewGuid().ToString()));
        }
    }
}
