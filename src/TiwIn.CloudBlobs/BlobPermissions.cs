//-----------------------------------------------------------------------
// <copyright file="BlobPermissions.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System;

    /// <summary>
    /// <see cref="BlobPermissions"/> contains the list of
    /// permissions that can be set for a blob's access policy. 
    /// </summary>
    [Flags]
    public enum BlobPermissions
    {
        /// <summary>
        /// Read the content, properties, metadata, and block list. Use the blob as the source of a copy operation.
        /// </summary>
        Read = 1,

        /// <summary>
        /// Add a block to an append blob.
        /// </summary>
        Add = 2,

        /// <summary>
        /// Write a new blob, snapshot a blob, or copy a blob to a new blob.
        /// </summary>
        Create = 4,

        /// <summary>
        /// Create or write content, properties, metadata, or block list. Snapshot or lease the blob. Resize the blob (page blob only). Use the blob as the destination of a copy operation.
        /// </summary>
        Write = 8,

        /// <summary>
        /// Delete a blob version
        /// </summary>
        Delete = 16,

        /// <summary>
        /// Indicates that all permissions are set.
        /// </summary>
        All = ~0
    }
}
