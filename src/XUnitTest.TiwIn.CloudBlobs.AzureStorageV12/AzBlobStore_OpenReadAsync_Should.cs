//-----------------------------------------------------------------------
// <copyright file="AzBlobStore_OpenReadAsync_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using TiwIn.Extensions;
    using Xunit;

    [Guid("29c290a3-3e9a-40ab-b7a3-5746d5b8b9fc")]
    public sealed class AzBlobStore_OpenReadAsync_Should : AzBlobStoreTest
    {
        [Fact]
        public async Task DownloadContent()
        {
            await "this is a test".ProcessAsStreamAsync(s =>
                TestContainer.GetBlobClient("test-blob.txt").UploadAsync(s, overwrite: true));
            await using var stream = await Store.OpenReadAsync(TestContainerName, "test-blob.txt");
            using var reader = new StreamReader(stream);
            var actual = await reader.ReadToEndAsync();
            Assert.Equal("this is a test", actual);
        }
    }
}
