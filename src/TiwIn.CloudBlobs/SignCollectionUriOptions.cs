//-----------------------------------------------------------------------
// <copyright file="SignCollectionUriOptions.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System;
    using System.Text;


    public sealed class SignCollectionUriOptions : SignUriOptions
    {

        internal override void Assert()
        {
            base.Assert();
            if(false == Permissions.HasValue)
                throw new InvalidOperationException($"Collection permissions are required.");
        }

        public CollectionPermissions? Permissions { get; set; }


        public override string ToString()
        {
            return new StringBuilder($"{Permissions}")
                .Append($" {base.ToString()}")
                .ToString();
        }
    }
}
