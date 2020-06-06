//-----------------------------------------------------------------------
// <copyright file="BlobStoreInfoService.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.Common
{
    using System;
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    public abstract class BlobStoreInfoService : IBlobStoreInfoService
    {
        private static readonly Regex BlobFullNameRegex;

        static BlobStoreInfoService()
        {
            BlobFullNameRegex = new Regex(@"(?xim-s)^(?:https?\W{3}[^//]+.|[//])?(?<container>[^//]+).(?<blob>.+)", RegexOptions.Compiled);
        }

        protected abstract bool IsConflictError(Exception ex);
        protected abstract bool IsBlobNotFoundError(Exception ex);
        protected abstract bool IsCollectionNotFoundError(Exception ex);
        protected abstract bool IsCollectionAlreadyExists(Exception ex);

        protected abstract bool IsAuthorizationPermissionMismatchError(Exception ex);
        protected abstract bool IsBlobAlreadyExistsError(Exception ex);
        protected abstract bool IsAuthorizationError(Exception ex);

        protected virtual BlobName ParseBlobFullName(string fullName)
        {
            var match = BlobFullNameRegex.Match(fullName);
            if (match.Success)
            {
                Debug.Assert(match.Groups["container"].Success);
                Debug.Assert(match.Groups["blob"].Success);
                return new BlobName(
                    match.Groups["container"].Value,
                    match.Groups["blob"].Value);
            }

            throw new FormatException("Invalid blob full name.");
        }

        [DebuggerStepThrough]
        BlobName IBlobStoreInfoService.ParseBlobFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Blob's full name is required.", nameof(fullName));
            return this.ParseBlobFullName(fullName);
        }

        bool IBlobStoreInfoService.IsConflictError(Exception ex) => !(ex is null) && IsConflictError(ex);

        bool IBlobStoreInfoService.IsBlobNotFoundError(Exception ex) => !(ex is null) && IsBlobNotFoundError(ex);

        bool IBlobStoreInfoService.IsCollectionNotFoundError(Exception ex) => !(ex is null) && IsCollectionNotFoundError(ex);
        bool IBlobStoreInfoService.IsCollectionAlreadyExistsError(Exception ex) => !(ex is null) && IsCollectionAlreadyExists(ex);
        bool IBlobStoreInfoService.IsBlobAlreadyExistsError(Exception ex) => !(ex is null) && IsBlobAlreadyExistsError(ex);

        bool IBlobStoreInfoService.IsAuthorizationPermissionMismatchError(Exception ex) => !(ex is null) && IsAuthorizationPermissionMismatchError(ex);

        bool IBlobStoreInfoService.IsAuthorizationError(Exception ex) => !(ex is null) && IsAuthorizationError(ex);
    }
}
