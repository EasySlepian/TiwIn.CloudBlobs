//-----------------------------------------------------------------------
// <copyright file="AzBlobStore_GetBlobInfoAsync_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using TiwIn.Extensions;
    using Xunit;

    [Guid("2b782aa2-c102-46b8-b63e-9aeb7632f0bd")]
    public sealed class AzBlobStore_GetBlobInfoAsync_Should : AzBlobStoreTest
    {
        [Fact]
        public async Task Work()
        {
            var blob = TestContainer.GetBlobClient("test.txt");
            await "some text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite: true));
            await blob.SetMetadataAsync(new Dictionary<string, string>()
            {
                ["TestKey"] = "TestValue"
            });

            var info = await Store.GetBlobInfoAsync(TestContainerName, blob.Name);
            Assert.Equal(blob.Name, info.BlobName);
            Assert.Equal(1, info.Metadata.Count);
            Assert.Equal("TestValue",info.Metadata["TestKey"]);
        }
    }
}
