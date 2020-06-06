//-----------------------------------------------------------------------
// <copyright file="BlobName.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System;

    public struct BlobName
    {

        public BlobName(string collectionName, string blobName)
        {
            CollectionName = collectionName;
            Name = blobName;
            if(string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Collection name is required.", nameof(collectionName));
            if (string.IsNullOrWhiteSpace(blobName))
                throw new ArgumentException("Blob name is required.", nameof(blobName));
        }

        public string CollectionName { get; }
        public string Name { get; }
    }
}
