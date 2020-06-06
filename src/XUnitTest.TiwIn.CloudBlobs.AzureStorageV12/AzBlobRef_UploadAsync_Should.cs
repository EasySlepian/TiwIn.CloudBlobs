//-----------------------------------------------------------------------
// <copyright file="AzBlobRef_UploadAsync_Should.cs" company="TiwIn">
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
    using static BlobPermissions;

    [Guid("66e4a736-a7c3-4a8c-bd68-cee800165581")]
    public sealed class AzBlobRef_UploadAsync_Should : AzBlobRefTestBase
    {
        [Fact]
        public async Task UploadStream()
        {
            var blob = TestContainer.GetBlobClient("test-blob.txt");
            
            var blobRef = CreateBlobRef(blob.Name, options =>
            {
                options.Permissions = Write | Create;
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });


            await "This is a test".ProcessAsStreamAsync(stream => blobRef.UploadAsync(stream, options=> options.Overwrite = true));

            var actual = await TestContainer.GetBlobClient(blob.Name).ReadAllTextAsync();
            Assert.Equal("This is a test", actual);
        }
    }
}
