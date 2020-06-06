//-----------------------------------------------------------------------
// <copyright file="BlobClientExtensions.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12.Extensions
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using TiwIn.Extensions;

    public static class BlobClientExtensions
    {
        public static async Task<string> ReadAllTextAsync(this BlobClient self, Func<Stream, StreamReader> readerFactory = null)
        {
            if (self is null) return null;
            readerFactory ??= (stream)=> new StreamReader(stream);
            await using var memory = new MemoryStream();
            await self.DownloadToAsync(memory);
            memory.Seek(0, SeekOrigin.Begin);
            using var reader = readerFactory.Invoke(memory);
            return await reader.ReadToEndAsync();
        }


        public static async Task WriteAllTextAsync(this BlobClient self, string text, Func<Stream, StreamWriter> writerFactory = null)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (text.IsNullOrWhiteSpace()) throw new ArgumentException("Text is required.", nameof(text));

            await text.ProcessAsStreamAsync(async (stream) =>
            {
                await self.UploadAsync(stream);
            }, writerFactory);
        }


    }
}
