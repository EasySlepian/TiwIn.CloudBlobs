//-----------------------------------------------------------------------
// <copyright file="AzCollectionRef_OpenReadAsync_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using TiwIn.Extensions;
    using Xunit;

    [Guid("24341c00-2080-4f3a-99d1-3d67d3bedf75")]
    public sealed class AzCollectionRef_OpenReadAsync_Should : AzCollectionRefTestBase
    {
        [Fact]
        public async Task DownloadContent()
        {
            var container = CreateCollectionRef(options =>
            {
                options.Permissions = CollectionPermissions.Read;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            var blobName = "read-only-blob.txt";

            await "this is a test".ProcessAsStreamAsync(stream => TestContainer.GetBlobClient(blobName).UploadAsync(stream, overwrite: true));
            await using var stream = await container.OpenReadAsync(blobName);
            using var reader = new StreamReader(stream);
            var actual = await reader.ReadToEndAsync();
            Assert.Equal("this is a test", actual);
        }
    }
}
