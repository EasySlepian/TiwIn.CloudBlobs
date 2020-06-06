//-----------------------------------------------------------------------
// <copyright file="AzCollectionRef_ReadAllTextAsync_Should.cs" company="TiwIn">
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

    [Guid("2baf3a37-96c5-4699-9e8a-102d66983180")]
    public class AzCollectionRef_ReadAllTextAsync_Should : AzCollectionRefTestBase
    {
        [Fact]
        public async Task DownloadText()
        {
            var container = CreateCollectionRef(options =>
            {
                options.Permissions = CollectionPermissions.Read;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            var blobName = "some-blob.txt";

            await "this is a test".ProcessAsStreamAsync(stream => TestContainer.GetBlobClient(blobName).UploadAsync(stream, overwrite: true));
            var actual = await container.ReadAllTextAsync(blobName);
            
            Assert.Equal("this is a test", actual);
        }


        [Fact]
        public async Task ThrowBlobNotFound()
        {
            var container = CreateCollectionRef(options =>
            {
                options.Permissions = CollectionPermissions.Read;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            await AssertExtensions.ThrowsAsync(
                container.IsBlobNotFoundError,
                () => container.ReadAllTextAsync(Guid.NewGuid().ToString()));
        }


        [Fact]
        public async Task ThrowCollectionNotFound()
        {
            var container = CreateCollectionRef(Guid.NewGuid().ToString(),options =>
            {
                options.Permissions = CollectionPermissions.Read;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });
            await AssertExtensions.ThrowsAsync(
                container.IsCollectionNotFoundError,
                () => container.ReadAllTextAsync( "myObject.bin"));
        }

    }
}
