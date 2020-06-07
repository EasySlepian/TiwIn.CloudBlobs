//-----------------------------------------------------------------------
// <copyright file="IBlobStoreProvider.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System;

    /// <summary>
    /// Blob Store Provider
    /// </summary>
    /// <seealso cref="TiwIn.CloudBlobs.IBlobStoreInfoService" />
    public interface IBlobStoreProvider : IBlobStoreInfoService
    {
        IBlobRef GetBlobRef(Uri preSignedUri);
        ICollectionRef GetCollectionRef(Uri preSignedUri);

        /// <summary>
        /// Creates an instance of <see cref="IBlobStore"/> by parsing the provided storage connection string.
        /// </summary>
        /// <param name="connectionString">The storage connection string.</param>
        /// <returns></returns>
        IBlobStore GetBlobStore(string connectionString);
    }
}
