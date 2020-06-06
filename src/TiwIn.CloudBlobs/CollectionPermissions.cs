//-----------------------------------------------------------------------
// <copyright file="CollectionPermissions.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System;

    /// <summary>
    /// <see cref="T:ObjectStorageCollectionPermissions" /> contains the list of
    /// permissions that can be set for an object's access policy. 
    /// </summary>
    [Flags]
    public enum CollectionPermissions
    {
        /// <summary>Read the content, properties, metadata, or block list of any blob in the container. Use any blob in the container as the source of a copy operation.</summary>
        Read = 1,
        /// <summary>Indicates that Add is permitted.</summary>
        Add = 2,
        /// <summary>Write a new blob to the container, snapshot any blob in the container, or copy a blob to a new blob in the container.</summary>
        Create = 4,
        /// <summary>For any blob in the container, create or write content, properties, metadata, or block list. Snapshot or lease the blob. Resize the blob (page blob only). Use the blob as the destination of a copy operation.</summary>
        Write = 8,
        /// <summary>Delete any blob in the container.</summary>
        Delete = 16, // 0x00000010
        /// <summary>List blobs in the container.</summary>
        List = 32, // 0x00000020
        /// <summary>Indicates that all permissions are set.</summary>
        All = -1,
    }
}
