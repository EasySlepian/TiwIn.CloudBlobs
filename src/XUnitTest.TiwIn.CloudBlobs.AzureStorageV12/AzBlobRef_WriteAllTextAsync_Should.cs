//-----------------------------------------------------------------------
// <copyright file="AzBlobRef_WriteAllTextAsync_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Extensions;
    using Xunit;
    using static BlobPermissions;

    [Guid("154b8969-9492-4a38-ab0b-a00271de4f57")]
    public sealed class AzBlobRef_WriteAllTextAsync_Should : AzBlobRefTestBase
    {
        [Fact]
        public async Task UploadText()
        {
            var blob = TestContainer.GetBlobClient("test-blob.txt");

            var blobRef = CreateBlobRef(blob.Name, options =>
            {
                options.Permissions = Write | Create;
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });
            
            
            await blobRef.WriteAllTextAsync("This is a test", options=> options.Overwrite = true);
            var actual = await blob.ReadAllTextAsync();
            Assert.Equal("This is a test", actual);
        }
    }
}
