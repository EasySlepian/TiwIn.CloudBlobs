//-----------------------------------------------------------------------
// <copyright file="AzCollectionRef_DeleteBlobAsync_Should.cs" company="TiwIn">
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

    [Guid("0a0e6839-a367-416a-8b56-d46970a55f6d")]
    public class AzCollectionRef_DeleteBlobAsync_Should : AzCollectionRefTestBase
    {
        [Fact]
        public async Task DeleteExistingIfPermitted()
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
            await container.DeleteBlobAsync(blob.Name);
            Assert.False(await blob.ExistsAsync());
        }

        [Fact]
        public async Task ThrowIfNotExists()
        {
            var container = CreateCollectionRef(options =>
            {
                options.Permissions = CollectionPermissions.Delete;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            await AssertExtensions.ThrowsAsync(container.IsBlobNotFoundError, () =>
                container.DeleteBlobAsync(Guid.NewGuid().ToString()));
        }

        [Fact]
        public async Task ThrowIfNotPermitted()
        {
            var container = CreateCollectionRef(options =>
            {
                options.Permissions = CollectionPermissions.Read;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            await AssertExtensions.ThrowsAsync(container.IsAuthorizationPermissionMismatchError, () =>
                container.DeleteBlobAsync(Guid.NewGuid().ToString()));
        }
    }
}
