//-----------------------------------------------------------------------
// <copyright file="AzBlobStore_UploadAsync_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Extensions;
    using TiwIn.Extensions;
    using Xunit;

    [Guid("18ebcf4f-48f6-4257-aff1-f2f1bddeace0")]
    public sealed class AzBlobStore_UploadAsync_Should: AzBlobStoreTest
    {
        [Fact]
        public async Task UploadStream()
        {
            var blob = TestContainer.GetBlobClient("upload.tst");
            await blob.DeleteIfExistsAsync();
            await "This is a test".ProcessAsStreamAsync(stream => Store.UploadAsync(
                TestContainerName,
                blob.Name,
                stream));
            var actual = await blob.ReadAllTextAsync();
            Assert.Equal("This is a test", actual);
        }
    }
}
