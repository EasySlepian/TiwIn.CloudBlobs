//-----------------------------------------------------------------------
// <copyright file="CollectionRef.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using Extensions;

    public abstract class CollectionRef : RelayBlobStoreInfoService, ICollectionRef
    {
        protected CollectionRef(BlobStoreInfoService innerInfoProvider) 
            : base(innerInfoProvider)
        {
        }

        public abstract string CollectionName { get; }

        protected abstract Task DeleteBlobAsync(string blobName, DeleteBlobOptions options);

        protected abstract Task<bool> DeleteBlobIfExistsAsync(string blobName, DeleteBlobOptions options);
        protected abstract IAsyncEnumerable<IBlobInfo> GetBlobsAsync(ListBlobsOptions options);
        protected abstract Task<string> ReadAllTextAsync(string blobName, BlobReadOptions options);
        protected abstract Task<Stream> OpenReadAsync(string blobName, BlobReadOptions options);
        protected abstract Task<byte[]> ReadAllBytesAsync(string blobName, BlobReadOptions options);
        protected abstract Task UploadAsync(string blobName, Stream input, BlobUploadOptions options);
        protected abstract Task WriteAllTextAsync(string blobName, string text, BlobWriteTextOptions options);
        protected abstract Task<IBlobInfo> GetBlobInfoAsync(string blobName, BlobLoadInfoOptions options);

        protected abstract Uri GetBlobUri(string blobName);

        private void AssertBlobName(string blobName)
        {
            if(blobName.IsNullOrWhiteSpace())
                throw new ArgumentException("Blob name is required");
        }


        Task ICollectionRef.DeleteBlobAsync(string blobName, Action<DeleteBlobOptions> config)
        {
            AssertBlobName(blobName);
            var options = new DeleteBlobOptions();
            config?.Invoke(options);
            return DeleteBlobAsync(blobName, options);
        }

        

        IAsyncEnumerable<IBlobInfo> ICollectionRef.GetBlobsAsync(Action<ListBlobsOptions> config)
        {
            var options = new ListBlobsOptions();
            config?.Invoke(options);
            return GetBlobsAsync(options);
        }

        Task<string> ICollectionRef.ReadAllTextAsync(string blobName, Action<BlobReadOptions> config)
        {
            AssertBlobName(blobName);
            var options = new BlobReadOptions();
            config?.Invoke(options);
            return ReadAllTextAsync(blobName, options);
        }

        Task<Stream> ICollectionRef.OpenReadAsync(string blobName, Action<BlobReadOptions> config)
        {
            AssertBlobName(blobName);
            var options = new BlobReadOptions();
            config?.Invoke(options);
            return OpenReadAsync(blobName, options);
        }

        Task<byte[]> ICollectionRef.ReadAllBytesAsync(string blobName, Action<BlobReadOptions> config)
        {
            AssertBlobName(blobName);
            var options = new BlobReadOptions();
            config?.Invoke(options);
            return ReadAllBytesAsync(blobName, options);
        }

        Task ICollectionRef.UploadAsync(string blobName, Stream input, Action<BlobUploadOptions> config)
        {
            AssertBlobName(blobName);
            if (input == null) throw new ArgumentNullException(nameof(input));
            var options = new BlobUploadOptions();
            config?.Invoke(options);
            return UploadAsync(blobName, input, options);
        }

        Task ICollectionRef.WriteAllTextAsync(string blobName, string text, Action<BlobWriteTextOptions> config)
        {
            AssertBlobName(blobName);
            if(text.IsNullOrWhiteSpace())
                throw new ArgumentException("Input text is required", nameof(text));
            var options = new BlobWriteTextOptions();
            config?.Invoke(options);
            return WriteAllTextAsync(blobName, text, options);
        }

        Task<IBlobInfo> ICollectionRef.GetBlobInfoAsync(string blobName, Action<BlobLoadInfoOptions> config)
        {
            AssertBlobName(blobName);
            var options = new BlobLoadInfoOptions();
            config?.Invoke(options);
            return GetBlobInfoAsync(blobName, options);
        }

        

        Task<bool> ICollectionRef.DeleteBlobIfExistsAsync(string blobName, Action<DeleteBlobOptions> config)
        {
            AssertBlobName(blobName);
            var options = new DeleteBlobOptions();
            config?.Invoke(options);
            return DeleteBlobIfExistsAsync(blobName, options);
        }

        Uri ICollectionRef.GetBlobUri(string blobName)
        {
            AssertBlobName(blobName);
            return GetBlobUri(blobName);
        }
    }
}
