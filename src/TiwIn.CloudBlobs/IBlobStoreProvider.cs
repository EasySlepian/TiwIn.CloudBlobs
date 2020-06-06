//-----------------------------------------------------------------------
// <copyright file="IBlobStoreProvider.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System;

    public interface IBlobStoreProvider : IBlobStoreInfoService
    {
        IBlobRef GetBlobRef(Uri preSignedUri);
        ICollectionRef GetCollectionRef(Uri preSignedUri);
        IBlobStore GetBlobStore(string connectionString);
    }
}
