//-----------------------------------------------------------------------
// <copyright file="BlobLockedCallback.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System;
    using System.Threading.Tasks;

    public delegate Task BlobLockedCallback(Guid transactionId);
}
