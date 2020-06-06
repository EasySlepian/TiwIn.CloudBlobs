//-----------------------------------------------------------------------
// <copyright file="AzCollectionRef_GetBlobUri_Should.cs" company="TiwIn">
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

    [Guid("e07b7a8f-8c5f-44ab-a389-91669c4d7a80")]
    public class AzCollectionRef_GetBlobUri_Should : AzCollectionRefTestBase
    {
        [Fact]
        public async Task SupportReadOnlyOperation()
        {
            var container = CreateCollectionRef(options =>
            {
                options.Permissions = CollectionPermissions.Read;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            var blobName = "read-only-blob.txt";
            var uri = container.GetBlobUri(blobName);
            var blob = new BlobClient(uri);

            await "some text".ProcessAsStreamAsync(stream => TestContainer.GetBlobClient(blobName).UploadAsync(stream, overwrite: true));
            await blob.ReadAllTextAsync();
            await blob.GetPropertiesAsync();

            await AssertExtensions.ThrowsAsync(container.IsAuthorizationPermissionMismatchError,
                () => blob.SetMetadataAsync(new Dictionary<string, string>()));
            await AssertExtensions.ThrowsAsync(container.IsAuthorizationPermissionMismatchError,
                () => "some new text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream)));
            await AssertExtensions.ThrowsAsync(container.IsAuthorizationPermissionMismatchError,
                () => blob.DeleteAsync());
        }

        [Fact]
        public async Task SupportWriteOnlyOperation()
        {
            var container = CreateCollectionRef(options =>
            {
                options.Permissions = CollectionPermissions.Write;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            var blobName = "write-only-blob.txt";
            var uri = container.GetBlobUri(blobName);
            var blob = new BlobClient(uri);

            await "some text".ProcessAsStreamAsync(stream => TestContainer.GetBlobClient(blobName).UploadAsync(stream, overwrite: true));
            await "some new text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite: true));
            await blob.SetMetadataAsync(new Dictionary<string, string>());

            await AssertExtensions.ThrowsAsync(container.IsAuthorizationPermissionMismatchError,
                () => blob.ReadAllTextAsync());
            await AssertExtensions.ThrowsAsync(container.IsAuthorizationPermissionMismatchError,
                () => blob.GetPropertiesAsync());

            await AssertExtensions.ThrowsAsync(container.IsAuthorizationPermissionMismatchError,
                () => blob.DeleteAsync());
        }

        [Fact]
        public async Task SupportCreateOnlyOperation()
        {
            var container = CreateCollectionRef(options =>
            {
                options.Permissions = CollectionPermissions.Create;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            var blobName = "create-only-blob.txt";
            var uri = container.GetBlobUri(blobName);
            var blob = new BlobClient(uri);

            await TestContainer.DeleteBlobIfExistsAsync(blob.Name);
            await "some text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream));
            await AssertExtensions.ThrowsAsync(container.IsAuthorizationError,
                () => "some new text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite:true)));

            await AssertExtensions.ThrowsAsync(container.IsAuthorizationPermissionMismatchError,
                () => blob.SetMetadataAsync(new Dictionary<string, string>()));

            await AssertExtensions.ThrowsAsync(container.IsAuthorizationPermissionMismatchError,
                () => blob.ReadAllTextAsync());
            await AssertExtensions.ThrowsAsync(container.IsAuthorizationPermissionMismatchError,
                () => blob.GetPropertiesAsync());

            await AssertExtensions.ThrowsAsync(container.IsAuthorizationPermissionMismatchError,
                () => blob.DeleteAsync());
        }

        [Fact]
        public async Task SupportDeleteOnlyOperation()
        {
            var container = CreateCollectionRef(options =>
            {
                options.Permissions = CollectionPermissions.Delete;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            var blobName = "delete-only-blob.txt";
            var uri = container.GetBlobUri(blobName);
            var blob = new BlobClient(uri);

            await "some text".ProcessAsStreamAsync(stream => TestContainer.GetBlobClient(blobName).UploadAsync(stream, overwrite: true));


            await AssertExtensions.ThrowsAsync(container.IsAuthorizationPermissionMismatchError,
                () => blob.SetMetadataAsync(new Dictionary<string, string>()));

            await AssertExtensions.ThrowsAsync(container.IsAuthorizationPermissionMismatchError,
                () => "some new text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite: true)));

            await AssertExtensions.ThrowsAsync(container.IsAuthorizationPermissionMismatchError,
                () => blob.ReadAllTextAsync());
            await AssertExtensions.ThrowsAsync(container.IsAuthorizationPermissionMismatchError,
                () => blob.GetPropertiesAsync());

            await blob.DeleteAsync();
        }

        [Fact]
        public async Task SupportCompositePermissionsOperations()
        {
            var container = CreateCollectionRef(options =>
            {
                options.Permissions = CollectionPermissions.All;
                options.Started(TimeSpan.FromDays(-1));
                options.ExpiresAfter(TimeSpan.FromMinutes(1));
            });

            var blobName = "composite-permissions-blob.txt";
            var uri = container.GetBlobUri(blobName);
            var blob = new BlobClient(uri);

            await "some text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite:true));
            await "some new text".ProcessAsStreamAsync(stream => blob.UploadAsync(stream, overwrite: true));
            await blob.SetMetadataAsync(new Dictionary<string, string>());
            await blob.ReadAllTextAsync();
            await blob.GetPropertiesAsync();
            await blob.DeleteAsync();
        }
    }
}
