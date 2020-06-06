//-----------------------------------------------------------------------
// <copyright file="BlobStoreProvider.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.Common
{
    using System;
    using System.Diagnostics;

    public abstract class BlobStoreProvider : RelayBlobStoreInfoService, IBlobStoreProvider
    {
        protected BlobStoreProvider(BlobStoreInfoService innerInfoProvider) : base(innerInfoProvider)
        {
        }

        protected abstract IBlobRef GetBlobRef(Uri preSignedUri);
        protected abstract ICollectionRef GetCollection(Uri signedUri);
        protected abstract IBlobStore CreateBlobStore(string connectionString);

        IBlobRef IBlobStoreProvider.GetBlobRef(Uri preSignedUri)
        {
            if (preSignedUri == null) throw new ArgumentNullException(nameof(preSignedUri));
            return GetBlobRef(preSignedUri);
        }

        [DebuggerStepThrough]
        ICollectionRef IBlobStoreProvider.GetCollectionRef(Uri signedUri)
        {
            if (signedUri == null) throw new ArgumentNullException(nameof(signedUri));
            return GetCollection(signedUri);
        }

        [DebuggerStepThrough]
        IBlobStore IBlobStoreProvider.GetBlobStore(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Storage connection string is required.", nameof(connectionString));

            return CreateBlobStore(connectionString);
        }
    }
}
