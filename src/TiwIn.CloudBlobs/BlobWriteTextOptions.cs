//-----------------------------------------------------------------------
// <copyright file="BlobWriteTextOptions.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    public sealed class BlobWriteTextOptions : BlobWriteOptions
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Func<Stream, StreamWriter> _streamWriterFactory;

        public BlobWriteTextOptions()
        {
            ContentType = "text/plain";
            ContentEncoding = Encoding.UTF8.HeaderName;
        }
        public Func<Stream, StreamWriter> StreamWriterFactory
        {
            [DebuggerNonUserCode]
            get => (_streamWriterFactory ??= CreateStreamWriter);
            [DebuggerNonUserCode]
            set => _streamWriterFactory = value ?? throw new ArgumentNullException(nameof(value));
        }

        private StreamWriter CreateStreamWriter(Stream stream) => new StreamWriter(stream);

        public void ApplyWriterSettings(StreamWriter writer)
        {
            ContentEncoding = String.IsNullOrWhiteSpace(ContentEncoding) ? writer.Encoding.HeaderName : ContentEncoding;
        }
    }
}
