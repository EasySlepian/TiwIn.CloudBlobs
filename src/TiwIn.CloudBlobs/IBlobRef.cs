//-----------------------------------------------------------------------
// <copyright file="IBlobRef.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public interface IBlobRef : IBlobStoreInfoService
    {
        Task DeleteAsync(Action<DeleteBlobOptions> config = null);
        Task<bool> DeleteIfExistsAsync(Action<DeleteBlobOptions> config = null);
        Task<Stream> OpenReadAsync(Action<BlobReadOptions> config = null);
        Task<byte[]> ReadAllBytesAsync(Action<BlobReadOptions> config = null);
        Task UploadAsync(Stream stream, Action<BlobWriteTextOptions> config = null);
        Task WriteAllTextAsync(string text, Action<BlobWriteTextOptions> config = null);
    }
}
