//-----------------------------------------------------------------------
// <copyright file="BlobLoadInfoOptions.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    public class BlobLoadInfoOptions
    {
        public BlobLoadInfoOptions()
        {
            IncludeMetadata = true;
        }

        public bool IncludeMetadata { get; set; }
    }
}
