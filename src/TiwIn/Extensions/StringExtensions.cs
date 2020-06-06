//-----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.Extensions
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public static class StringExtensions
    {
        public static async Task ProcessAsStreamAsync(this string self, Func<MemoryStream, Task> callback, Func<Stream, StreamWriter> writerFactory = null)
        {
            self ??= string.Empty;
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            writerFactory ??= (stream)=> new StreamWriter(stream);
            await using var stream = new MemoryStream();
            await using var writer = writerFactory.Invoke(stream);
            await writer.WriteAsync(self);
            await writer.FlushAsync();
            stream.Seek(0, SeekOrigin.Begin);
            await callback.Invoke(stream);
        }

        public static bool IsNullOrWhiteSpace(this string self) => string.IsNullOrWhiteSpace(self);
    }
}
