//-----------------------------------------------------------------------
// <copyright file="ICollectionRef.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public interface ICollectionRef : IBlobStoreInfoService
    {
        string CollectionName { get; }
        
        Task DeleteBlobAsync(string blobName, Action<DeleteBlobOptions> config = null);

        Task<bool> DeleteBlobIfExistsAsync(string blobName, Action<DeleteBlobOptions> config = null);

        IAsyncEnumerable<IBlobInfo> GetBlobsAsync(Action<ListBlobsOptions> config = null);


        Task<string> ReadAllTextAsync(string blobName, Action<BlobReadOptions> config = null);


        Task<Stream> OpenReadAsync(string blobName, Action<BlobReadOptions> config = null);


        Task<byte[]> ReadAllBytesAsync(string blobName, Action<BlobReadOptions> config = null);

        Task UploadAsync(string blobName, Stream input, Action<BlobUploadOptions> config = null);


        Task WriteAllTextAsync(string blobName, string text, Action<BlobWriteTextOptions> config = null);

        Task<IBlobInfo> GetBlobInfoAsync(string blobName, Action<BlobLoadInfoOptions> config = null);
        Uri GetBlobUri(string blobName);
    }
}
