//-----------------------------------------------------------------------
// <copyright file="AzBlobStore.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Threading.Tasks;
    using Azure;
    using Azure.Storage;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Azure.Storage.Blobs.Specialized;
    using Azure.Storage.Sas;
    using Common;

    sealed class AzBlobStore : BlobStore
    {

        private readonly StorageSharedKeyCredential _credential;
        private readonly BlobServiceClient _blobService;
        public AzBlobStore(string connectionString) 
            : base(AzBlobStoreInfoService.Instance)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Storage connection string is required.", nameof(connectionString));

            var valuesByKey = ParseKeyValueString(connectionString, StringComparer.InvariantCultureIgnoreCase);
            if(false == valuesByKey.TryGetValue("AccountName", out var accountName) ||
               false == valuesByKey.TryGetValue("AccountKey", out var accountKey))
                throw new ArgumentException("Invalid connection string format. Both AccountName and AccountKey key-value pairs are required.");

            AccountName = accountName;
            AccountKey = accountKey;
            _credential = new StorageSharedKeyCredential(AccountName, AccountKey);
            _blobService = new BlobServiceClient(connectionString);
        }

        public string AccountName { get; }
        protected string AccountKey { get; }

        public Uri GetContainerUri(string containerName, string query)
        {
            var builder = new UriBuilder()
            {
                Scheme = "https",
                Host = $"{AccountName}.blob.core.windows.net",
                Path = $"{containerName}",
                Query = query
            };
            return builder.Uri;
        }

        public Uri GetBlobUri(string containerName, string blobPath, string query)
        {
            var builder = new UriBuilder()
            {
                Scheme = "https",
                Host = $"{AccountName}.blob.core.windows.net",
                Path = $"{containerName}/{blobPath}",
                Query = query
            };
            return builder.Uri;
        }

        protected override Uri GetCollectionUri(string collectionName, SignCollectionUriOptions options)
        {
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = collectionName,
                StartsOn = options.StartsOn,
                ExpiresOn = options.ExpiresOn
            };

            sasBuilder.SetPermissions((BlobContainerSasPermissions)options.Permissions.GetValueOrDefault());
            var sas = sasBuilder.ToSasQueryParameters(_credential).ToString();
            return GetContainerUri(collectionName, sas);
        }

        protected override Uri GetBlobUri(string collectionName, string blobName, SignBlobUriOptions options)
        {
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = collectionName,
                BlobName = blobName,
                StartsOn = options.StartsOn,
                ExpiresOn = options.ExpiresOn
            };

            sasBuilder.SetPermissions((BlobSasPermissions)options.Permissions.GetValueOrDefault());
            var sas = sasBuilder.ToSasQueryParameters(_credential).ToString();
            return GetBlobUri(collectionName, blobName, sas);
        }

        protected override async Task<bool> DeleteCollectionIfExistsAsync(string containerName, CollectionDeleteOptions options)
        {
            var client = _blobService.GetBlobContainerClient(containerName);
            var info = await client.DeleteIfExistsAsync();
            return (info?.Value).GetValueOrDefault(false);
        }

        protected override async Task<bool> CreateCollectionIfNotExistsAsync(string containerName, CollectionCreateOptions options)
        {
            var client = _blobService.GetBlobContainerClient(containerName);
            var info = await client.CreateIfNotExistsAsync();
            return (info?.Value != null);
        }

        protected override async Task LockAsync(string collectionName, string blobName, BlobLockedCallback callback)
        {
            var container = _blobService.GetBlobContainerClient(collectionName);
            var blob = container.GetBlobClient(blobName);
            var transactionId = Guid.NewGuid();
            var lease = blob.GetBlobLeaseClient(transactionId.ToString("N"));

            var lockInitialDuration = TimeSpan.FromSeconds(30);
            await lease.AcquireAsync(lockInitialDuration);

            try
            {
                var renewInterval = TimeSpan.FromTicks(Convert.ToInt64(0.9 * lockInitialDuration.Ticks));
                await Observable
                    .Interval(renewInterval)
                    .TakeUntil(callback.Invoke(transactionId).ToObservable())
                    .SelectMany(tick => lease.RenewAsync())
                    .LastOrDefaultAsync();
            }
            finally
            {
                await lease.ReleaseAsync();
            }
        }


        protected override async Task CreateCollectionAsync(string collectionName, CollectionCreateOptions options)
        {
            var info = await _blobService.CreateBlobContainerAsync(collectionName);
        }

        protected override async IAsyncEnumerable<IBlobInfo> GetBlobsAsync(string collectionName, ListBlobsOptions options)
        {
            var container = _blobService.GetBlobContainerClient(collectionName);
            var traits = BlobTraits.None | (options.IncludeMetadata ? BlobTraits.Metadata : BlobTraits.None);
            await foreach (BlobItem blob in container.GetBlobsAsync(traits))
            {
                yield return new AzBlobInfo(collectionName, blob, ()=> container.GetBlobClient(blob.Name));
            }
        }

        protected override async Task<Stream> OpenReadAsync(string collectionName, string blobName, BlobReadOptions config)
        {
            var container = _blobService.GetBlobContainerClient(collectionName);
            var blob = container.GetBlobClient(blobName);
            var downloadInfo = await blob.DownloadAsync();
            return downloadInfo.Value.Content;
        }

        protected override async Task<string> ReadAllTextAsync(string collectionName, string blobName, BlobReadOptions config)
        {
            var container = _blobService.GetBlobContainerClient(collectionName);
            var blob = container.GetBlobClient(blobName);
            var downloadInfo = await blob.DownloadAsync();
            using var reader = config.StreamReaderFactory(downloadInfo.Value.Content);
            return await reader.ReadToEndAsync();
        }

        protected override async Task<byte[]> ReadAllBytesAsync(string collectionName, string blobName, BlobReadOptions config)
        {
            var container = _blobService.GetBlobContainerClient(collectionName);
            var blob = container.GetBlobClient(blobName);
            await using var stream = new MemoryStream();
            await blob.DownloadToAsync(stream);
            return stream.ToArray();
        }

        protected override async Task WriteAllTextAsync(string collectionName, string blobName, string text, BlobWriteTextOptions options)
        {
            await using var stream = new MemoryStream();
            await using var writer = options.StreamWriterFactory.Invoke(stream);
            options.ApplyWriterSettings(writer);
            await writer.WriteAsync(text);
            await writer.FlushAsync();
            stream.Seek(0, SeekOrigin.Begin);
            await UploadAsync(collectionName, blobName, stream, options);
        }

        protected override Task DeleteBlobAsync(string collectionName, string blobName, DeleteBlobOptions options)
        {
            var container = _blobService.GetBlobContainerClient(collectionName);
            return container.DeleteBlobAsync(blobName);
        }

        protected override Task DeleteCollectionAsync(string collectionName, CollectionDeleteOptions options)
        {
            var container = _blobService.GetBlobContainerClient(collectionName);
            return container.DeleteAsync();
        }

        protected override Task UploadAsync(string collectionName, string blobName, Stream input, BlobUploadOptions options)
        {
            var container = _blobService.GetBlobContainerClient(collectionName);
            var blob = container.GetBlobClient(blobName);
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

        protected override async Task<IBlobInfo> GetBlobInfoAsync(string collectionName, string blobName, BlobLoadInfoOptions options)
        {
            var container = _blobService.GetBlobContainerClient(collectionName);
            var blob = container.GetBlobClient(blobName);
            var properties = await blob.GetPropertiesAsync();
    
            return new AzBlobInfo(collectionName, blobName, properties.Value,  ()=> blob);
        }

        protected override async Task<bool> DeleteBlobIfExistsAsync(string collectionName, string blobName, DeleteBlobOptions options)
        {
            var container = _blobService.GetBlobContainerClient(collectionName);
            var blob = container.GetBlobClient(blobName);
            return await blob.DeleteIfExistsAsync();
        }

        public override string ToString() => AccountName;
    }
}
