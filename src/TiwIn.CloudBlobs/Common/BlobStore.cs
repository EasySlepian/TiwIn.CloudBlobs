//-----------------------------------------------------------------------
// <copyright file="BlobStore.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public abstract class BlobStore : RelayBlobStoreInfoService, IBlobStore
    {
        
        private static readonly Regex KeyValueRegex;
        private static readonly Regex CollectionNameRegex;

        static BlobStore()
        {
            var keyValuePattern = @"(?xim-s)(?<=^|;)\s*(?<key>@key)\s*=\s*(?<value>@value)"
                .Replace("@key", @"[^\s=]+")
                .Replace("@value", @"[^\s;]+");
            KeyValueRegex = new Regex(keyValuePattern, RegexOptions.Compiled);
            CollectionNameRegex = new Regex(@"^\w+(-\w+)*$", RegexOptions.Compiled);
        }

        [DebuggerStepThrough]
        protected BlobStore(BlobStoreInfoService infoProvider) : base(infoProvider)
        {
            
        }


        protected Dictionary<string, string> ParseKeyValueString(string input,
            StringComparer comparer)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input key-value string is required.", nameof(input));

            var result = new Dictionary<string, string>(comparer);
            for (Match match = KeyValueRegex.Match(input); match.Success; match = match.NextMatch())
            {
                Debug.Assert(match.Groups["key"].Success);
                Debug.Assert(match.Groups["value"].Success);
                result.Add(match.Groups["key"].Value, match.Groups["value"].Value);
            }

            return result;
        }

        protected abstract Uri GetCollectionUri(string collectionName, SignCollectionUriOptions options);
        protected abstract Uri GetBlobUri(string collectionName, string blobName, SignBlobUriOptions options);

        protected abstract Task<bool> DeleteCollectionIfExistsAsync(string collectionName, CollectionDeleteOptions options);

        protected abstract Task LockAsync(string collectionName, string blobName, BlobLockedCallback callback);

        protected abstract Task CreateCollectionAsync(string collectionName, CollectionCreateOptions options);

        protected abstract Task<bool> CreateCollectionIfNotExistsAsync(string collectionName, CollectionCreateOptions options);
        protected abstract IAsyncEnumerable<IBlobInfo> GetBlobsAsync(string collectionName, ListBlobsOptions options);
        protected abstract Task<Stream> OpenReadAsync(string collectionName, string blobName, BlobReadOptions options);
        protected abstract Task<string> ReadAllTextAsync(string collectionName, string blobName, BlobReadOptions config);
        protected abstract Task<byte[]> ReadAllBytesAsync(string collectionName, string blobName, BlobReadOptions config);
        protected abstract Task WriteAllTextAsync(string collectionName, string blobName, string text, BlobWriteTextOptions options);
        protected abstract Task DeleteBlobAsync(string collectionName, string blobName, DeleteBlobOptions options);

        protected abstract Task DeleteCollectionAsync(string collectionName, CollectionDeleteOptions options);

        protected abstract Task UploadAsync(string collectionName, string blobName, Stream input, BlobUploadOptions options);
        protected abstract Task<IBlobInfo> GetBlobInfoAsync(string collectionName, string blobName, BlobLoadInfoOptions options);

        protected abstract Task<bool> DeleteBlobIfExistsAsync(string collectionName, string blobName, DeleteBlobOptions options);


        [DebuggerStepThrough]
        private void Assert(string collectionName, string blobName)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Collection name is required", nameof(collectionName));
            if (string.IsNullOrWhiteSpace(blobName))
                throw new ArgumentException("Object name is required", nameof(blobName));
        }

        [DebuggerStepThrough]
        IAsyncEnumerable<IBlobInfo> IBlobStore.GetBlobsAsync(string collectionName, Action<ListBlobsOptions> config)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Collection name is required", nameof(collectionName));
            var options = new ListBlobsOptions();
            config?.Invoke(options);
            return GetBlobsAsync(collectionName, options);
        }


        [DebuggerStepThrough]
        Uri IBlobStore.GetCollectionUri(string collectionName, Action<SignCollectionUriOptions> config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            var options = new SignCollectionUriOptions();
            config.Invoke(options);
            options.Assert();
            return GetCollectionUri(collectionName, options);
        }

        [DebuggerStepThrough]
        Uri IBlobStore.GetBlobUri(string collectionName, string blobName, Action<SignBlobUriOptions> config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            var options = new SignBlobUriOptions();
            config.Invoke(options);
            options.Assert();
            return this.GetBlobUri(collectionName, blobName, options);
        }


        [DebuggerStepThrough]
        Task IBlobStore.LockAsync(string collectionName, string blobName, BlobLockedCallback callback)
        {
            Assert(collectionName, blobName);
            if (callback is null) throw new ArgumentNullException(nameof(callback));
            return this.LockAsync(collectionName, blobName, callback);
        }

        

        [DebuggerStepThrough]
        Task<string> IBlobStore.ReadAllTextAsync(
            string collectionName, 
            string blobName,
            Action<BlobReadOptions> config)
        {
            Assert(collectionName, blobName);
            var options = new BlobReadOptions();
            config?.Invoke(options);
            return this.ReadAllTextAsync(collectionName, blobName, options);
        }



        Task<Stream> IBlobStore.OpenReadAsync(string collectionName, string blobName, Action<BlobReadOptions> config = null)
        {
            Assert(collectionName, blobName);
            var options = new BlobReadOptions();
            config?.Invoke(options);
            return OpenReadAsync(collectionName, blobName, options);
        }

        

        Task<byte[]> IBlobStore.ReadAllBytesAsync(string collectionName, string blobName, Action<BlobReadOptions> config)
        {
            Assert(collectionName, blobName);
            var options = new BlobReadOptions();
            config?.Invoke(options);
            return this.ReadAllBytesAsync(collectionName, blobName, options);
        }

        Task IBlobStore.UploadAsync(string collectionName, string blobName, Stream input, Action<BlobUploadOptions> config)
        {
            Assert(collectionName, blobName);
            if (input == null) throw new ArgumentNullException(nameof(input));
            var options = new BlobUploadOptions();
            config?.Invoke(options);
            return this.UploadAsync(collectionName, blobName, input, options);
        }


        [DebuggerStepThrough]
        Task IBlobStore.WriteAllTextAsync(
            string collectionName, 
            string blobName, 
            string text,
            Action<BlobWriteTextOptions> config)
        {
            Assert(collectionName, blobName);
            if (text is null) throw new ArgumentNullException(nameof(text));

            var options = new BlobWriteTextOptions();
            options.ContentType = "text/plain";
            config?.Invoke(options);
            return this.WriteAllTextAsync(collectionName, blobName, text, options);
        }


        

        [DebuggerStepThrough]
        Task<IBlobInfo> IBlobStore.GetBlobInfoAsync(string collectionName, string blobName, Action<BlobLoadInfoOptions> config)
        {
            Assert(collectionName, blobName);
            var options = new BlobLoadInfoOptions();
            config?.Invoke(options);
            return this.GetBlobInfoAsync(collectionName, blobName, options);
        }


        [DebuggerStepThrough]
        Task IBlobStore.CreateCollectionAsync(string collectionName, Action<CollectionCreateOptions> config)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Collection name is required", nameof(collectionName));
            
            if (IsValidCollectionName(collectionName))
            {
                var options = new CollectionCreateOptions();
                config?.Invoke(options);
                return this.CreateCollectionAsync(collectionName, options);
            }

            throw new ArgumentOutOfRangeException(nameof(collectionName), "Invalid collection name");
        }

        

        Task<bool> IBlobStore.DeleteCollectionIfExistsAsync(string collectionName, Action<CollectionDeleteOptions> config)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Collection name is required", nameof(collectionName));
            var options = new CollectionDeleteOptions();
            config?.Invoke(options);
            return this.DeleteCollectionIfExistsAsync(collectionName, options);
        }

        [DebuggerStepThrough]
        Task<bool> IBlobStore.CreateCollectionIfNotExistsAsync(string collectionName, Action<CollectionCreateOptions> config)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Collection name is required", nameof(collectionName));
            if (IsValidCollectionName(collectionName))
            {
                var options = new CollectionCreateOptions();
                config?.Invoke(options);
                return this.CreateCollectionIfNotExistsAsync(collectionName, options);
            }
            throw new ArgumentOutOfRangeException(nameof(collectionName), "Invalid collection name");
        }

        Task IBlobStore.DeleteBlobAsync(string collectionName, string blobName, Action<DeleteBlobOptions> config)
        {
            Assert(collectionName, blobName);
            var options = new DeleteBlobOptions();
            config?.Invoke(options);
            return DeleteBlobAsync(collectionName, blobName, options);
        }

        Task<bool> IBlobStore.DeleteBlobIfExistsAsync(string collectionName, string blobName, Action<DeleteBlobOptions> config)
        {
            Assert(collectionName, blobName);
            var options = new DeleteBlobOptions();
            config?.Invoke(options);
            return DeleteBlobIfExistsAsync(collectionName, blobName, options);
        }

        protected virtual bool IsValidCollectionName(string collectionName) =>
            CollectionNameRegex.IsMatch(collectionName);

        Task IBlobStore.DeleteCollectionAsync(string collectionName, Action<CollectionDeleteOptions> config)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Collection name is required", nameof(collectionName));
            var options = new CollectionDeleteOptions();
            config?.Invoke(options);
            return DeleteCollectionAsync(collectionName, options);
        }
    }
}
