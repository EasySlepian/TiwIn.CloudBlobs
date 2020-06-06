//-----------------------------------------------------------------------
// <copyright file="BlobReadOptions.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System;
    using System.IO;

    public sealed class BlobReadOptions
    {
        private Func<Stream, StreamReader> _streamReaderFactory;


        public Func<Stream, StreamReader> StreamReaderFactory
        {
            get => _streamReaderFactory ??= CreateReader;
            set => _streamReaderFactory = value ?? throw new ArgumentNullException(nameof(value), "Stream factory value");
        }

        private StreamReader CreateReader(Stream stream) => new StreamReader(stream);
    }
}
