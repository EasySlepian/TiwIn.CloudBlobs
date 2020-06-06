//-----------------------------------------------------------------------
// <copyright file="AzBlobStore_GetContainerUri_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Extensions;
    using TiwIn.Extensions;
    using Xunit;
    using static CollectionPermissions;


    [Guid("534304c9-1d5e-4ca6-80b6-bc8fea95887a")]
    public class AzBlobStore_GetContainerUri_Should : AzBlobStoreTest
    {
        [Fact]
        public async Task SupportListOnlyOperation()
        {
            var uri = Store.GetCollectionUri(TestContainerName, options =>
            {
                options.Permissions = List;
                options.Started(TimeSpan.FromMinutes(1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            var container = new BlobContainerClient(uri);

            var blobNames = Enumerable
                .Range(0, 5)
                .Select(i=> $"list-only-blob-{i}.txt").ToHashSet();

            await blobNames
                .ToObservable()
                .SelectMany(blobName =>
                    {
                        return TestContainer
                            .DeleteBlobIfExistsAsync(blobName)
                            .ToObservable()
                            .Select(_=> blobName);
                    })
                .SelectMany((blobName,index) => TestContainer
                    .GetBlobClient(blobName)
                    .WriteAllTextAsync($"This is a test {index}")
                    .ToObservable());
            
            
            await foreach (var item in container.GetBlobsAsync())
            {
                bool removed = blobNames.Remove(item.Name);
                if (!removed) continue;

                await AssertExtensions.ThrowsAsync(
                    Store.IsAuthorizationPermissionMismatchError,
                    () => container.DeleteBlobAsync(item.Name));



                await "some text".ProcessAsStreamAsync(async (stream) =>
                {
                    await AssertExtensions.ThrowsAsync(
                        Store.IsAuthorizationPermissionMismatchError,
                        () => container.UploadBlobAsync(item.Name, stream));
                });

            }

            Assert.Empty(blobNames);

            await AssertExtensions.ThrowsAsync(
                Store.IsAuthorizationError,
                () => container.DeleteAsync());

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationError, () => container.SetMetadataAsync(new Dictionary<string, string>()
            {
                ["TestKey"] = "TestValue"
            }));

            

        }

        [Fact]
        public async Task SupportReadOnlyOperations()
        {
            var uri = Store.GetCollectionUri(TestContainerName, options =>
            {
                options.Permissions = Read;
                options.Started(TimeSpan.FromMinutes(1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            var container = new BlobContainerClient(uri);


            var blobName = $"read-only-blob.txt";
            await TestContainer.DeleteBlobIfExistsAsync(blobName);
            await "some text".ProcessAsStreamAsync(async (stream) =>
            {
                await TestContainer.UploadBlobAsync(blobName, stream);
            });


            var blob = container.GetBlobClient(blobName);
            var actual = await blob.ReadAllTextAsync();
            Assert.Equal("some text", actual);


            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError, async () =>
            {
                await foreach (var item in container.GetBlobsAsync()) { }
            });

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError, async () =>
            {
                await "some text"
                    .ProcessAsStreamAsync((stream) => container
                        .UploadBlobAsync(Guid.NewGuid().ToString(), stream));
            });

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError, () => blob.DeleteAsync());

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationError, () => container.SetMetadataAsync(new Dictionary<string, string>()
            {
                ["TestKey"] = "TestValue"
            }));
        }

        [Fact]
        public async Task SupportCreateOnlyOperations()
        {
            var uri = Store.GetCollectionUri(TestContainerName, options =>
            {
                options.Permissions = Create;
                options.Started(TimeSpan.FromMinutes(1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            var container = new BlobContainerClient(uri);


            var blobName = $"create-only-blob.txt";
            await TestContainer.DeleteBlobIfExistsAsync(blobName);
            await "some text".ProcessAsStreamAsync((stream) => container.UploadBlobAsync(blobName, stream));




            var blob = container.GetBlobClient(blobName);
            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError, () => blob.ReadAllTextAsync());

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationError, () => blob.SetMetadataAsync(new Dictionary<string, string>()
            {
                ["TestKey"] = "TestValue"
            }));


            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError, async () =>
            {
                await foreach (var item in container.GetBlobsAsync()) { }
            });

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError, () => blob.DeleteAsync());

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationError, () => container.SetMetadataAsync(new Dictionary<string, string>()
            {
                ["TestKey"] = "TestValue"
            }));
        }

        [Fact]
        public async Task SupportWriteOnlyOperations()
        {
            var uri = Store.GetCollectionUri(TestContainerName, options =>
            {
                options.Permissions = Write ;
                options.Started(TimeSpan.FromMinutes(1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            var container = new BlobContainerClient(uri);


            var blobName = $"write-only-blob.txt";
            await TestContainer.DeleteBlobIfExistsAsync(blobName);
            await "some text".ProcessAsStreamAsync((stream) => container.UploadBlobAsync(blobName, stream));

            var blob = container.GetBlobClient(blobName);
            await blob.SetMetadataAsync(new Dictionary<string, string>()
            {
                ["TestKey"] = "TestValue"
            });


            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError, () => blob.ReadAllTextAsync());

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError, async () =>
            {
                await foreach (var item in container.GetBlobsAsync()) { }
            });

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError, () => blob.DeleteAsync());

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationError, () => container.SetMetadataAsync(new Dictionary<string, string>()
            {
                ["TestKey"] = "TestValue"
            }));
        }

        [Fact]
        public async Task SupportDeleteOnlyOperations()
        {
            var uri = Store.GetCollectionUri(TestContainerName, options =>
            {
                options.Permissions = Delete;
                options.Started(TimeSpan.FromMinutes(1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            var blobName = $"delete-only-blob.txt";
            var container = new BlobContainerClient(uri);
            var blob = container.GetBlobClient(blobName);

            
            await TestContainer.DeleteBlobIfExistsAsync(blobName);
            await "some text".ProcessAsStreamAsync((stream) => TestContainer.UploadBlobAsync(blobName, stream));

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError,  () => blob
                .SetMetadataAsync(new Dictionary<string, string>()
            {
                ["TestKey"] = "TestValue"
            }));
            


            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError, () => blob.ReadAllTextAsync());

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError, async () =>
            {
                await foreach (var item in container.GetBlobsAsync()) { }
            });

            

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationError, () => container.SetMetadataAsync(new Dictionary<string, string>()
            {
                ["TestKey"] = "TestValue"
            }));

            await blob.DeleteAsync();
        }

        [Fact]
        public async Task SupportCompositeOperations()
        {
            var uri = Store.GetCollectionUri(TestContainerName, options =>
            {
                options.Permissions = All;
                options.Started(TimeSpan.FromMinutes(1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            var blobName = $"all-permissions-blob.txt";
            var container = new BlobContainerClient(uri);

            await container.DeleteBlobIfExistsAsync(blobName);
            await "some text".ProcessAsStreamAsync((stream) => container.UploadBlobAsync(blobName, stream));
            var blob = container.GetBlobClient(blobName);

            await blob.SetMetadataAsync(new Dictionary<string, string>()
            {
                ["TestKey"] = "TestValue"
            });



            await blob.ReadAllTextAsync();
            await foreach (var item in container.GetBlobsAsync())
            {
                Debug.WriteLine(item.Name);
            }



            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationError, () => container.SetMetadataAsync(new Dictionary<string, string>()
            {
                ["TestKey"] = "TestValue"
            }));

            await blob.DeleteAsync();
        }
    }
}
