//-----------------------------------------------------------------------
// <copyright file="AzCollectionRef_ReadAllBytesAsync_Should.cs" company="TiwIn">
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

    [Guid("6c588c80-035f-4205-9c35-33a04cc7c0a0")]
    public class AzCollectionRef_ReadAllBytesAsync_Should : AzCollectionRefTestBase
    {
        [Fact]
        public async Task DownloadBytes()
        {
            var container = CreateCollectionRef(options =>
            {
                options.Permissions = CollectionPermissions.Read;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            var blobName = "some-blob.txt";

            await "this is a test".ProcessAsStreamAsync(stream => TestContainer.GetBlobClient(blobName).UploadAsync(stream, overwrite: true));

            var bytes = await container.ReadAllBytesAsync(blobName);
            var actual = Encoding.UTF8.GetString(bytes);
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
                () => container.ReadAllBytesAsync(Guid.NewGuid().ToString()));
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
                () => container.ReadAllBytesAsync("myObject.bin"));
        }
    }
}
