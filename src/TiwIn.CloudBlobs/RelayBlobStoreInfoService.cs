//-----------------------------------------------------------------------
// <copyright file="RelayBlobStoreInfoService.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System;
    using System.Diagnostics;
    using Common;

    public abstract class RelayBlobStoreInfoService : IBlobStoreInfoService
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IBlobStoreInfoService _innerInfoService;

        [DebuggerStepThrough]
        protected RelayBlobStoreInfoService(BlobStoreInfoService innerInfoProvider)
        {
            _innerInfoService = innerInfoProvider ?? throw new System.ArgumentNullException(nameof(innerInfoProvider));
        }

        [DebuggerStepThrough]
        public bool IsConflictError(Exception ex) => _innerInfoService.IsConflictError(ex);

        [DebuggerStepThrough]
        public bool IsBlobNotFoundError(Exception ex) => _innerInfoService.IsBlobNotFoundError(ex);

        [DebuggerStepThrough]
        public bool IsCollectionNotFoundError(Exception ex) => _innerInfoService.IsCollectionNotFoundError(ex);

        [DebuggerStepThrough]
        public bool IsCollectionAlreadyExistsError(Exception ex) => _innerInfoService.IsCollectionAlreadyExistsError(ex);

        public bool IsAuthorizationPermissionMismatchError(Exception ex)
        {
            return _innerInfoService.IsAuthorizationPermissionMismatchError(ex);
        }

        public bool IsBlobAlreadyExistsError(Exception ex)
        {
            return _innerInfoService.IsBlobAlreadyExistsError(ex);
        }

        public bool IsAuthorizationError(Exception ex)
        {
            return _innerInfoService.IsAuthorizationError(ex);
        }

        [DebuggerStepThrough]
        public BlobName ParseBlobFullName(string fullName) => _innerInfoService.ParseBlobFullName(fullName);
    }
}
