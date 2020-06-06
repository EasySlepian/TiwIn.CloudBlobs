//-----------------------------------------------------------------------
// <copyright file="BlobRef.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.Common
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Extensions;

    public abstract class BlobRef : RelayBlobStoreInfoService, IBlobRef
    {
        protected BlobRef(BlobStoreInfoService innerInfoProvider) : base(innerInfoProvider)
        {
        }

        protected abstract Task DeleteAsync(DeleteBlobOptions options);
        protected abstract Task<bool> DeleteIfExistsAsync(DeleteBlobOptions options);
        protected abstract Task<Stream> OpenReadAsync(BlobReadOptions options);
        protected abstract Task<byte[]> ReadAllBytesAsync(BlobReadOptions options);
        protected abstract Task UploadAsync(Stream stream, BlobWriteTextOptions options);

        Task IBlobRef.DeleteAsync(Action<DeleteBlobOptions> config)
        {
            var options = new DeleteBlobOptions();
            config?.Invoke(options);
            return DeleteAsync(options);
        }

        Task<bool> IBlobRef.DeleteIfExistsAsync(Action<DeleteBlobOptions> config)
        {
            var options = new DeleteBlobOptions();
            config?.Invoke(options);
            return DeleteIfExistsAsync(options);
        }

        Task<Stream> IBlobRef.OpenReadAsync(Action<BlobReadOptions> config)
        {
            var options = new BlobReadOptions();
            config?.Invoke(options);
            return OpenReadAsync(options);
        }

        Task<byte[]> IBlobRef.ReadAllBytesAsync(Action<BlobReadOptions> config)
        {
            var options = new BlobReadOptions();
            config?.Invoke(options);
            return ReadAllBytesAsync(options);
        }

        Task IBlobRef.UploadAsync(Stream stream, Action<BlobWriteTextOptions> config)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            var options = new BlobWriteTextOptions();
            config?.Invoke(options);
            return UploadAsync(stream, options);
        }

        Task IBlobRef.WriteAllTextAsync(string text, Action<BlobWriteTextOptions> config)
        {
            if(text.IsNullOrWhiteSpace())
                throw new ArgumentException("Input text is required.", nameof(text));
            var options = new BlobWriteTextOptions();
            config?.Invoke(options);
            return text.ProcessAsStreamAsync(stream=> UploadAsync(stream, options));
        }
    }
}
