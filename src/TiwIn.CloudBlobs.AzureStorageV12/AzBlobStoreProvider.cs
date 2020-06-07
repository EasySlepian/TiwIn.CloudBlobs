//-----------------------------------------------------------------------
// <copyright file="AzBlobStoreProvider.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Diagnostics;
    using Azure.Storage.Blobs;
    using Common;

    public sealed class AzBlobStoreProvider : BlobStoreProvider
    {
        private AzBlobStoreProvider() : base(AzBlobStoreInfoService.Instance)
        {
            
        }

        [DebuggerStepThrough]
        public static IBlobStore Parse(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Storage connection string is required.", nameof(connectionString));
            return new AzBlobStore(connectionString);
        }

        public static IBlobStoreProvider Create() => new AzBlobStoreProvider();



        [DebuggerStepThrough]
        protected override IBlobStore CreateBlobStore(string connectionString) => Parse(connectionString);

        protected override IBlobRef GetBlobRef(Uri uri)
        {
            var blob = new BlobClient(uri);
            return new AzBlobRef(blob);
        }

        [DebuggerStepThrough]
        protected override ICollectionRef GetCollection(Uri signedUri)
        {
            var container = new BlobContainerClient(signedUri);
            return new AzBlobCollectionRef(container, AzBlobStoreInfoService.Instance);
        }
    }
}
