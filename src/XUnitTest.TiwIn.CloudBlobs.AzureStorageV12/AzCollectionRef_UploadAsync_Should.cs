//-----------------------------------------------------------------------
// <copyright file="AzCollectionRef_UploadAsync_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Extensions;
    using TiwIn.Extensions;
    using Xunit;
    using static CollectionPermissions;

    [Guid("f7f9b77d-1c4c-46e5-bb50-6aef0f19d1c3")]
    public sealed class AzCollectionRef_UploadAsync_Should : AzCollectionRefTestBase
    {
        [Fact]
        public async Task UploadStream()
        {
            var container = CreateCollectionRef(options =>
            {
                options.Permissions = Write | Create;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            var blobName = "uploaded.txt";
            await "This is a test".ProcessAsStreamAsync(stream => container.UploadAsync(blobName, stream, options=> options.Overwrite = true));

            var actual = await TestContainer.GetBlobClient(blobName).ReadAllTextAsync();
            Assert.Equal("This is a test", actual);
        }
    }
}
