//-----------------------------------------------------------------------
// <copyright file="AzBlobCollectionRef.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Common;

    [DebuggerDisplay("{CollectionName}")]
    sealed class AzBlobCollectionRef : CollectionRef
    {
        private readonly BlobContainerClient _containerClient;


        public AzBlobCollectionRef(BlobContainerClient containerClient, BlobStoreInfoService innerInfoProvider) 
            : base(innerInfoProvider)
        {
            _containerClient = containerClient ?? throw new ArgumentNullException(nameof(containerClient));
        }

        public override string CollectionName => _containerClient.Name;


        protected override Task DeleteBlobAsync(string blobName, DeleteBlobOptions options)
        {
            return _containerClient.DeleteBlobAsync(blobName, DeleteSnapshotsOption.None);
        }

        protected override async Task<bool> DeleteBlobIfExistsAsync(string blobName, DeleteBlobOptions options)
        {
            var info =  await _containerClient.DeleteBlobIfExistsAsync(blobName, DeleteSnapshotsOption.None);
            return (info?.Value).GetValueOrDefault(false);
        }

        protected override async IAsyncEnumerable<IBlobInfo> GetBlobsAsync(ListBlobsOptions options)
        {
            await foreach (var blob in _containerClient.GetBlobsAsync())
            {
                yield return new AzBlobInfo(_containerClient.Name, blob, ()=> _containerClient.GetBlobClient(blob.Name));
            }
        }

        protected override async Task<string> ReadAllTextAsync(string blobName, BlobReadOptions options)
        {
            var blob = _containerClient.GetBlobClient(blobName);
            var downloadInfo = await blob.DownloadAsync();
            using var reader = options.StreamReaderFactory(downloadInfo.Value.Content);
            return await reader.ReadToEndAsync();
        }

        protected override async Task<Stream> OpenReadAsync(string blobName, BlobReadOptions options)
        {
            var blob = _containerClient.GetBlobClient(blobName);
            var downloadInfo = await blob.DownloadAsync();
            return downloadInfo.Value.Content;
        }

        protected override async Task<byte[]> ReadAllBytesAsync(string blobName, BlobReadOptions options)
        {
            var blob = _containerClient.GetBlobClient(blobName);
            await using var stream = new MemoryStream();
            await blob.DownloadToAsync(stream);
            return stream.ToArray();
        }

        protected override Task UploadAsync(string blobName, Stream input, BlobUploadOptions options)
        {
            var blob = _containerClient.GetBlobClient(blobName);
            return blob.UploadAsync(
                input,
                new BlobHttpHeaders
                {
                    ContentType = options.ContentType,
                    ContentEncoding = options.ContentEncoding
                },
                options.Metadata,
                conditions: options.Overwrite ? null : new BlobRequestConditions { IfNoneMatch = new ETag("*") });
        }

        protected override async Task WriteAllTextAsync(string blobName, string text, BlobWriteTextOptions options)
        {
            await using var stream = new MemoryStream();
            await using var writer = options.StreamWriterFactory.Invoke(stream);
            options.ApplyWriterSettings(writer);
            await writer.WriteAsync(text);
            await writer.FlushAsync();
            stream.Seek(0, SeekOrigin.Begin);
            await UploadAsync(blobName, stream, options);
        }

        protected override async Task<IBlobInfo> GetBlobInfoAsync(string blobName, BlobLoadInfoOptions options)
        {
            var blob = _containerClient.GetBlobClient(blobName);
            var properties = await blob.GetPropertiesAsync();

            return new AzBlobInfo(CollectionName, blobName, properties.Value, () => blob);
        }

        protected override Uri GetBlobUri(string blobName)
        {
            var blob = _containerClient.GetBlobClient(blobName);
            return blob.Uri;
        }
    }
}
