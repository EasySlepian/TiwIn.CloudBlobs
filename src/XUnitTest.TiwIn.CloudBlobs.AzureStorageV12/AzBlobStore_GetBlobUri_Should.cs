//-----------------------------------------------------------------------
// <copyright file="AzBlobStore_GetBlobUri_Should.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Extensions;
    using TiwIn.Extensions;
    using Xunit;
    using static BlobPermissions;

    [Guid("c142f89c-4bcb-4556-bb2c-06bc8a792561")]
    public class AzBlobStore_GetBlobUri_Should : AzBlobStoreTest
    {
        [Fact]
        public async Task SupportReadOnlyOperation()
        {
            var blobName = "read-only-blob.txt";
            var uri = Store.GetBlobUri(TestContainerName, blobName, options =>
            {
                options.Permissions = Read;
                options.Started(TimeSpan.FromMinutes(1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });
            var blob = new BlobClient(uri);

            await TestContainer.DeleteBlobIfExistsAsync(blobName);
            await "some text".ProcessAsStreamAsync(stream => TestContainer.UploadBlobAsync(blobName, stream));
            await blob.ReadAllTextAsync();
            await blob.GetPropertiesAsync();

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError,
                () => blob.SetMetadataAsync(new Dictionary<string, string>()));
            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError,
                () => "some new text".ProcessAsStreamAsync(stream=> blob.UploadAsync(stream)));
            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError,
                () => blob.DeleteAsync());

            ;


        }

        [Fact]
        public async Task SupportWriteOnlyOperation()
        {
            var blobName = "write-only-blob.txt";
            var uri = Store.GetBlobUri(TestContainerName, blobName, options =>
            {
                options.Permissions = Write;
                options.Started(TimeSpan.FromMinutes(1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });
            var blob = new BlobClient(uri);

            await TestContainer.DeleteBlobIfExistsAsync(blobName);
            await "some text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream));
            await "some new text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite: true));
            await blob.SetMetadataAsync(new Dictionary<string, string>());

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError,
                () => blob.ReadAllTextAsync());
            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError,
                () => blob.GetPropertiesAsync());
            
            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError,
                () => blob.DeleteAsync());
        }

        [Fact]
        public async Task SupportCreateOnlyOperation()
        {
            var blobName = "create-only-blob.txt";
            var uri = Store.GetBlobUri(TestContainerName, blobName, options =>
            {
                options.Permissions = Write;
                options.Started(TimeSpan.FromMinutes(1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });
            var blob = new BlobClient(uri);

            await TestContainer.DeleteBlobIfExistsAsync(blobName);
            await "some text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream));
            await blob.SetMetadataAsync(new Dictionary<string, string>());
            await "some new text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite: true));

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError,
                () => blob.ReadAllTextAsync());
            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError,
                () => blob.GetPropertiesAsync());

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError,
                () => blob.DeleteAsync());
        }

        [Fact]
        public async Task SupportDeleteOnlyOperation()
        {
            var blobName = "delete-only-blob.txt";
            var uri = Store.GetBlobUri(TestContainerName, blobName, options =>
            {
                options.Permissions = Delete;
                options.Started(TimeSpan.FromMinutes(1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });
            var blob = new BlobClient(uri);

            await TestContainer.DeleteBlobIfExistsAsync(blobName);
            await "some text".ProcessAsStreamAsync(stream => TestContainer.UploadBlobAsync(blobName, stream));
       

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError,
                () => blob.SetMetadataAsync(new Dictionary<string, string>()));

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError,
                () => "some new text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite: true)));

            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError,
                () => blob.ReadAllTextAsync());
            await AssertExtensions.ThrowsAsync(Store.IsAuthorizationPermissionMismatchError,
                () => blob.GetPropertiesAsync());

            await blob.DeleteAsync();
        }

        [Fact]
        public async Task SupportCompositePermissionsOperations()
        {
            var blobName = "delete-only-blob.txt";
            var uri = Store.GetBlobUri(TestContainerName, blobName, options =>
            {
                options.Permissions = All;
                options.Started(TimeSpan.FromMinutes(1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });
            var blob = new BlobClient(uri);

            await TestContainer.DeleteBlobIfExistsAsync(blobName);
            await "some text".ProcessAsStreamAsync(stream => TestContainer.UploadBlobAsync(blobName, stream));
            await "some new text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite:true));
            await blob.SetMetadataAsync(new Dictionary<string, string>());
            await blob.ReadAllTextAsync();
            await blob.GetPropertiesAsync();
            await blob.DeleteAsync();
        }
    }
}
