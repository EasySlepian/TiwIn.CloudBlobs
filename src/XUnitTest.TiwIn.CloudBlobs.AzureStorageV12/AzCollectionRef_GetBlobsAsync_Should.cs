//-----------------------------------------------------------------------
// <copyright file="AzCollectionRef_GetBlobsAsync_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Extensions;
    using TiwIn.Extensions;
    using Xunit;

    [Guid("ac8b0138-8b84-4564-95ed-c8cca2c40faf")]
    public sealed class AzCollectionRef_GetBlobsAsync_Should : AzCollectionRefTestBase
    {
        [Fact]
        public async Task SupportListOnlyOperation()
        {
            var container = CreateCollectionRef(options =>
            {
                options.Permissions = CollectionPermissions.List;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            var blobNames = Enumerable
                .Range(0, 5)
                .Select(i => $"list-only-blob-{i}.txt").ToHashSet();

            await blobNames
                .ToObservable()
                .SelectMany(blobName =>
                {
                    return TestContainer
                        .DeleteBlobIfExistsAsync(blobName)
                        .ToObservable()
                        .Select(_ => blobName);
                })
                .SelectMany((blobName, index) => TestContainer
                    .GetBlobClient(blobName)
                    .WriteAllTextAsync($"This is a test {index}")
                    .ToObservable());


            await foreach (var item in container.GetBlobsAsync())
            {
                bool removed = blobNames.Remove(item.BlobName);
                if (!removed) continue;

                await AssertExtensions.ThrowsAsync(
                    container.IsAuthorizationPermissionMismatchError,
                    () => container.DeleteBlobAsync(item.BlobName));



                await "some text".ProcessAsStreamAsync(async (stream) =>
                {
                    await AssertExtensions.ThrowsAsync(
                        container.IsAuthorizationPermissionMismatchError,
                        () => container.UploadAsync(item.BlobName, stream));
                });

            }

            Assert.Empty(blobNames);
        }
    }
}
