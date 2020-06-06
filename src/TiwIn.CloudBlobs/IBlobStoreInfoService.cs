//-----------------------------------------------------------------------
// <copyright file="IBlobStoreInfoService.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System;

    public interface IBlobStoreInfoService
    {
        bool IsConflictError(Exception ex);
        bool IsBlobNotFoundError(Exception ex);
        bool IsCollectionNotFoundError(Exception ex);
        bool IsCollectionAlreadyExistsError(Exception ex);
        bool IsAuthorizationPermissionMismatchError(Exception ex);

        bool IsBlobAlreadyExistsError(Exception ex);

        bool IsAuthorizationError(Exception ex);

        BlobName ParseBlobFullName(string fullName);
    }
}
