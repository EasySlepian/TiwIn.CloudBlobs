//-----------------------------------------------------------------------
// <copyright file="AzBlobInfo.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using BlobInfo = Common.BlobInfo;

    [DebuggerDisplay("Azure blob: {CollectionName}/{BlobName}")]
    sealed class AzBlobInfo : BlobInfo
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Lazy<BlobClient> _blobClient;

        internal AzBlobInfo(string containerName, BlobItem blob, Func<BlobClient> blobClientFactory) 
            : base(containerName, blob.Name, AzBlobStoreInfoService.Instance)
        {
            if (blobClientFactory == null) throw new ArgumentNullException(nameof(blobClientFactory));
            _blobClient = new Lazy<BlobClient>(blobClientFactory);
            Metadata = blob.Metadata?.ToImmutableDictionary();
            CreatedOn = blob.Properties.CreatedOn;
            ContentType = blob.Properties.ContentType;
            ContentEncoding = blob.Properties.ContentEncoding;
            ContentLanguage = blob.Properties.ContentLanguage;
            ContentLength = blob.Properties.ContentLength;
        }

        public AzBlobInfo(string containerName, string blobName, BlobProperties properties, Func<BlobClient> blobClientFactory) 
            : base(containerName, blobName, AzBlobStoreInfoService.Instance)
        {
            if (blobClientFactory == null) throw new ArgumentNullException(nameof(blobClientFactory));
            _blobClient = new Lazy<BlobClient>(blobClientFactory);
            Metadata = properties.Metadata?.ToImmutableDictionary();
            CreatedOn = properties.CreatedOn;
            ContentType = properties.ContentType;
            ContentEncoding = properties.ContentEncoding;
            ContentLanguage = properties.ContentLanguage;
            ContentLength = properties.ContentLength;
        }

        protected override async Task<object> ReadAllTextAsync(BlobReadOptions options)
        {
            var blob = _blobClient.Value;
            var downloadInfo = await blob.DownloadAsync();
            using var reader = options.StreamReaderFactory(downloadInfo.Value.Content) 
                               ?? throw new NullReferenceException($"{nameof(options.StreamReaderFactory)} returned null.");
            return await reader.ReadToEndAsync();
        }
    }
}
