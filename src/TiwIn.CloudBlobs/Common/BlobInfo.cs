//-----------------------------------------------------------------------
// <copyright file="BlobInfo.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.Common
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public abstract class BlobInfo : RelayBlobStoreInfoService, IBlobInfo
    {
        

        protected BlobInfo(string collectionName, string blobName, BlobStoreInfoService provider) 
            : base(provider)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Storage collection name is required.", nameof(collectionName));
            if (string.IsNullOrWhiteSpace(blobName))
                throw new ArgumentException("Storage object name is required.", nameof(blobName));

            CollectionName = collectionName;
            BlobName = blobName;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public long? ContentLength { get; protected set; }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string CollectionName { get; protected set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string BlobName { get; protected set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string ContentType { get; protected set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string ContentEncoding { get; protected set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string ContentLanguage { get; protected set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public DateTimeOffset? CreatedOn { get; protected set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IImmutableDictionary<string, string> Metadata { get; protected set; }

        string IBlobInfo.BlobName => this.BlobName;

        string IBlobInfo.CollectionName => this.CollectionName;

        string IBlobInfo.ContentType => this.ContentType;

        string IBlobInfo.ContentEncoding => this.ContentEncoding;

        string IBlobInfo.ContentLanguage => this.ContentLanguage;

        long? IBlobInfo.ContentLength => this.ContentLength;

        DateTimeOffset? IBlobInfo.CreatedOn => this.CreatedOn;

        IImmutableDictionary<string, string> IBlobInfo.Metadata => this.Metadata;

        protected abstract Task<object> ReadAllTextAsync(BlobReadOptions options);

        [DebuggerStepThrough]
        Task<object> IBlobInfo.ReadAllTextAsync(Action<BlobReadOptions> config)
        {
            var options = new BlobReadOptions();
            config?.Invoke(options);
            return ReadAllTextAsync(options);
        }
    }
}
