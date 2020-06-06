//-----------------------------------------------------------------------
// <copyright file="AzBlobStore_GetBlobsAsync_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using TiwIn.Extensions;
    using Xunit;

    [Guid("328138f3-286c-496f-84a7-87525b17f261")]
    public sealed class AzBlobStore_GetBlobsAsync_Should : AzBlobStoreTest
    {
        [Fact]
        public async Task ListAllBlobs()
        {
            var expectedBlobs = Enumerable.Range(0, 10).Select(i => $"blob-{i}").ToHashSet();

            await expectedBlobs
                .ToObservable()
                .SelectMany(blobName => TestContainer.DeleteBlobIfExistsAsync(blobName));

            await Observable
                .Range(0, 10)
                .SelectMany(i => $"some text {i}"
                    .ProcessAsStreamAsync(stream => TestContainer.UploadBlobAsync($"blob-{i}", stream))
                    .ToObservable());

            Assert.Equal(10, expectedBlobs.Count);
            await foreach (var item in Store.GetBlobsAsync(TestContainerName))
            {
                expectedBlobs.Remove(item.BlobName);
            }
            Assert.Empty(expectedBlobs);

        }
    }
}
