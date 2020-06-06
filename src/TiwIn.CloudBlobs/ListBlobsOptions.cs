//-----------------------------------------------------------------------
// <copyright file="ListBlobsOptions.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System.Diagnostics;

    public class ListBlobsOptions
    {
        [DebuggerNonUserCode]
        public ListBlobsOptions()
        {
            IncludeMetadata = true;
        }
        public bool IncludeMetadata { get; set; }
    }
}
