//-----------------------------------------------------------------------
// <copyright file="AzBlobRef.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Common;

    sealed class AzBlobRef : BlobRef
    {
        private readonly BlobClient _blob;

        public AzBlobRef(BlobClient blob) 
            : base(AzBlobStoreInfoService.Instance)
        {
            _blob = blob ?? throw new ArgumentNullException(nameof(blob));
        }

        protected override Task DeleteAsync(DeleteBlobOptions options)
        {
            return _blob.DeleteAsync();
        }

        protected override async Task<bool> DeleteIfExistsAsync(DeleteBlobOptions options)
        {
            var result = await _blob.DeleteIfExistsAsync();
            return (result?.Value == true);
        }

        protected override async Task<Stream> OpenReadAsync(BlobReadOptions options)
        {
            var downloadInfo = await _blob.DownloadAsync();
            return downloadInfo.Value.Content;
        }

        protected override async Task<byte[]> ReadAllBytesAsync(BlobReadOptions options)
        {
            await using var stream = new MemoryStream();
            await _blob.DownloadToAsync(stream);
            return stream.ToArray();
        }

        protected override Task UploadAsync(Stream stream, BlobWriteTextOptions options)
        {
            return _blob.UploadAsync(
                stream,
                new BlobHttpHeaders
                {
                    ContentType = options.ContentType,
                    ContentEncoding = options.ContentEncoding
                },
                options.Metadata,
                conditions: options.Overwrite ? null : new BlobRequestConditions { IfNoneMatch = new ETag("*") });
        }
    }
}
