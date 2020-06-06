//-----------------------------------------------------------------------
// <copyright file="AzBlobRef_OpenReadAsync_Should.cs" company="TiwIn">
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

    [Guid("28f7358d-f376-4b5d-b9d2-783c3dcfe348")]
    public sealed class AzBlobRef_OpenReadAsync_Should : AzBlobRefTestBase
    {
        [Fact]
        public async Task DownloadContent()
        {
            var blob = TestContainer.GetBlobClient("test-blob.txt");
            await "some text".ProcessAsStreamAsync(s => blob.UploadAsync(s, overwrite: true));

            var blobRef = CreateBlobRef(blob.Name, options =>
            {
                options.Permissions = BlobPermissions.Read;
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            await "this is a test".ProcessAsStreamAsync(s => TestContainer
                .GetBlobClient(blob.Name)
                .UploadAsync(s, overwrite: true));
            await using var stream = await blobRef.OpenReadAsync();
            using var reader = new StreamReader(stream);
            var actual = await reader.ReadToEndAsync();
            Assert.Equal("this is a test", actual);
        }
    }
}
