//-----------------------------------------------------------------------
// <copyright file="SignBlobUriOptions.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System;
    using System.Text;

    public sealed class SignBlobUriOptions : SignUriOptions
    {
        internal SignBlobUriOptions()
        {
            
        }

        public BlobPermissions? Permissions { get; set; }

        internal override void Assert()
        {
            base.Assert();
            if (false == Permissions.HasValue)
                throw new InvalidOperationException($"Blob permissions are required.");
        }

        public override string ToString()
        {
            return new StringBuilder($"{Permissions}")
                .Append($" {base.ToString()}")
                .ToString();
        }
    }
}
