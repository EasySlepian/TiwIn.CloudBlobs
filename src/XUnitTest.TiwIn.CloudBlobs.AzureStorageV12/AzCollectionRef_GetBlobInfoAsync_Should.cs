//-----------------------------------------------------------------------
// <copyright file="AzCollectionRef_GetBlobInfoAsync_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using TiwIn.Extensions;
    using Xunit;

    [Guid("e2f4d52f-2078-4dfc-9de0-42616ed2802b")]
    public sealed class AzCollectionRef_GetBlobInfoAsync_Should : AzCollectionRefTestBase
    {
        [Fact]
        public async Task IncludeMetadata()
        {
            var blob = TestContainer.GetBlobClient("test.txt");
            await "some text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite: true));
            await blob.SetMetadataAsync(new Dictionary<string, string>()
            {
                ["TestKey"] = "TestValue"
            });

            var container = CreateCollectionRef(options =>
            {
                options.Permissions = CollectionPermissions.Read;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            var info = await container.GetBlobInfoAsync(blob.Name);
            Assert.Equal(blob.Name, info.BlobName);
            Assert.Equal(1, info.Metadata.Count);
            Assert.Equal("TestValue", info.Metadata["TestKey"]);
        }
    }
}
