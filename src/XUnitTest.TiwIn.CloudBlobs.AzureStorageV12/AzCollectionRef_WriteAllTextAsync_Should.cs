//-----------------------------------------------------------------------
// <copyright file="AzCollectionRef_WriteAllTextAsync_Should.cs" company="TiwIn">
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
    using static CollectionPermissions;

    [Guid("6451303c-6542-40d3-99df-06221bf4f8d9")]
    public sealed class AzCollectionRef_WriteAllTextAsync_Should : AzCollectionRefTestBase
    {
        [Fact]
        public async Task UploadText()
        {
            var container = CreateCollectionRef(options =>
            {
                options.Permissions = Write | Create;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });
            var blob = TestContainer.GetBlobClient("upload.tst");
            await blob.DeleteIfExistsAsync();
            await container.WriteAllTextAsync(blob.Name, "This is a test");
            var actual = await blob.ReadAllTextAsync();
            Assert.Equal("This is a test", actual);
        }

        [Fact]
        public async Task ThrowCollectionNotFound()
        {
            var container = CreateCollectionRef(Guid.NewGuid().ToString(),options =>
            {
                options.Permissions = Write | Create;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });
            await AssertExtensions.ThrowsAsync(
                container.IsCollectionNotFoundError,
                () => container.WriteAllTextAsync(
                    "AnyBlob.txt",
                    "This is a test"));
        }
    }
}
