//-----------------------------------------------------------------------
// <copyright file="AzBlobStoreInfoService.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Collections.Generic;
    using Azure;
    using Common;

    sealed class AzBlobStoreInfoService : BlobStoreInfoService
    {
        public const string ContainerAlreadyExists = "ContainerAlreadyExists";
        public const string BlobAlreadyExists = "BlobAlreadyExists";
        public const string AuthorizationPermissionMismatch = "AuthorizationPermissionMismatch";
        public const string AuthorizationFailure = "AuthorizationFailure";
        public const string BlobNotFound = "BlobNotFound";
        public const string UnauthorizedBlobOverwrite = "UnauthorizedBlobOverwrite";
        static readonly StringComparer ErrorCodeComparer = StringComparer.OrdinalIgnoreCase;
        private static readonly HashSet<string> ConflictErrorCodes = new HashSet<string>(ErrorCodeComparer)
        {
            ContainerAlreadyExists,
            BlobAlreadyExists
        };

        private static readonly HashSet<string> AuthorizationErrorCodes = new HashSet<string>(ErrorCodeComparer)
        {
            AuthorizationFailure,
            AuthorizationPermissionMismatch,
            UnauthorizedBlobOverwrite
        };

        internal static readonly AzBlobStoreInfoService Instance = new AzBlobStoreInfoService();

        private static bool IsMatch(string errorCode, Exception ex) => (ex is RequestFailedException rfEx) && ErrorCodeComparer.Equals(errorCode, rfEx.ErrorCode);

        protected override bool IsCollectionAlreadyExists(Exception ex) => IsMatch(ContainerAlreadyExists, ex);
        protected override bool IsAuthorizationPermissionMismatchError(Exception ex) => IsMatch(AuthorizationPermissionMismatch, ex);

        protected override bool IsBlobAlreadyExistsError(Exception ex) => IsMatch(BlobAlreadyExists, ex);

        protected override bool IsAuthorizationError(Exception ex)
        {
            return (ex is RequestFailedException rfEx) && AuthorizationErrorCodes.Contains(rfEx.ErrorCode);
        }

        protected override bool IsConflictError(Exception ex)
        {
            return (ex is RequestFailedException rfEx) && ConflictErrorCodes.Contains(rfEx.ErrorCode);
        }

        protected override bool IsBlobNotFoundError(Exception ex)
        {
            return (ex is RequestFailedException rfEx) && ErrorCodeComparer.Equals(BlobNotFound, rfEx.ErrorCode);
        }

        protected override bool IsCollectionNotFoundError(Exception ex)
        {
            return (ex is RequestFailedException rfEx) &&
                   "ContainerNotFound".Equals(rfEx.ErrorCode, StringComparison.OrdinalIgnoreCase);
        }


    }
}
