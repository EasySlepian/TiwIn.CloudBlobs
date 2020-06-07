//-----------------------------------------------------------------------
// <copyright file="IBlobStore.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Cloud Blob Store contract
    /// </summary>
    /// <seealso cref="TiwIn.CloudBlobs.IBlobStoreInfoService" />
    public partial interface IBlobStore : IBlobStoreInfoService
    {
        Uri GetCollectionUri(string collectionName, Action<SignCollectionUriOptions> config);

        Uri GetBlobUri(string collectionName, string blobName, Action<SignBlobUriOptions> config);

        Task CreateCollectionAsync(string collectionName, Action<CollectionCreateOptions> config = null);

        Task<bool> CreateCollectionIfNotExistsAsync(string collectionName, Action<CollectionCreateOptions> config = null);

        Task DeleteCollectionAsync(string collectionName, Action<CollectionDeleteOptions> config = null);
        Task<bool> DeleteCollectionIfExistsAsync(string collectionName, Action<CollectionDeleteOptions> config = null);


        Task LockAsync(string collectionName, string blobName, BlobLockedCallback callback);

        /// <summary>
        /// The <see cref="GetBlobsAsync"/> operation returns an async sequence of blobs in this container.
        /// Enumerating the blobs may make multiple requests to the store while fetching all the values.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="config">The options configuration.</param>
        /// <returns></returns>
        IAsyncEnumerable<IBlobInfo> GetBlobsAsync(string collectionName, Action<ListBlobsOptions> config = null);


        Task<string> ReadAllTextAsync(string collectionName, string blobName, Action<BlobReadOptions> config = null);

        
        Task<Stream> OpenReadAsync(string collectionName, string blobName, Action<BlobReadOptions> config = null);


        Task<byte[]> ReadAllBytesAsync(string collectionName, string blobName, Action<BlobReadOptions> config = null);

        Task UploadAsync(string collectionName, string blobName, Stream input, Action<BlobUploadOptions> config = null);

        /// <summary>
        /// Asynchronously creates a new BLOB and writes the specified string to the BLOB.
        /// BLOB overwriting behavior is controlled with the options config- callback
        /// </summary>
        /// <param name="collectionName">Name of the blob collection.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="text">The text to be uploaded.</param>
        /// <param name="config">The upload options configuration.</param>
        /// <returns></returns>
        Task WriteAllTextAsync(string collectionName, string blobName, string text, Action<BlobWriteTextOptions> config = null);

        Task DeleteBlobAsync(string collectionName, string blobName, Action<DeleteBlobOptions> config = null);
        Task<bool> DeleteBlobIfExistsAsync(string collectionName, string blobName, Action<DeleteBlobOptions> config = null);

        Task<IBlobInfo> GetBlobInfoAsync(string collectionName, string blobName, Action<BlobLoadInfoOptions> config = null);

    }

    public partial interface IBlobStore
    {
        Uri GetBlobUri(string blobFullName, Action<SignBlobUriOptions> config)
        {
            var blobName = this.ParseBlobFullName(blobFullName);
            return this.GetBlobUri(blobName.CollectionName, blobName.Name, config);
        }

        Task LockAsync(string blobFullName, BlobLockedCallback callback)
        {
            var blobName = this.ParseBlobFullName(blobFullName);
            return this.LockAsync(blobName.CollectionName, blobName.Name, callback);
        }

        Task<string> ReadAllTextAsync(string blobFullName, Action<BlobReadOptions> config = null)
        {
            var blobName = this.ParseBlobFullName(blobFullName);
            return this.ReadAllTextAsync(blobName.CollectionName, blobName.Name, config);
        }

        Task<Stream> OpenReadAsync(string blobFullName, Action<BlobReadOptions> config = null)
        {
            var blobName = this.ParseBlobFullName(blobFullName);
            return this.OpenReadAsync(blobName.CollectionName, blobName.Name, config);
        }

        Task<byte[]> ReadAllBytesAsync(string blobFullName, Action<BlobReadOptions> config = null)
        {
            var blobName = this.ParseBlobFullName(blobFullName);
            return this.ReadAllBytesAsync(blobName.CollectionName, blobName.Name, config);
        }

        [DebuggerStepThrough]
        Task UploadAsync(string blobFullName, Stream input, Action<BlobUploadOptions> config = null)
        {
            var blobName = this.ParseBlobFullName(blobFullName);
            return this.UploadAsync(blobName.CollectionName, blobName.Name, input, config);
        }

        Task WriteAllTextAsync(string blobFullName, string text, Action<BlobWriteTextOptions> config = null)
        {
            var blobName = this.ParseBlobFullName(blobFullName);
            return this.WriteAllTextAsync(blobName.CollectionName, blobName.Name, config);
        }

        Task<IBlobInfo> GetBlobInfoAsync(string blobFullName, Action<BlobLoadInfoOptions> config = null)
        {
            var blobName = this.ParseBlobFullName(blobFullName);
            return this.GetBlobInfoAsync(blobName.CollectionName, blobName.Name, config);
        }

    }
}
