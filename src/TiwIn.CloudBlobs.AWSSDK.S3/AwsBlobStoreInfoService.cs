namespace TiwIn.CloudBlobs.AWSSDK.S3
{
    using System;
    using Common;

    sealed class AwsBlobStoreInfoService : BlobStoreInfoService
    {
        public static readonly AwsBlobStoreInfoService Instance = new AwsBlobStoreInfoService();
        protected override bool IsConflictError(Exception ex)
        {
            throw new NotImplementedException();
        }

        protected override bool IsBlobNotFoundError(Exception ex)
        {
            throw new NotImplementedException();
        }

        protected override bool IsCollectionNotFoundError(Exception ex)
        {
            throw new NotImplementedException();
        }

        protected override bool IsCollectionAlreadyExists(Exception ex)
        {
            throw new NotImplementedException();
        }

        protected override bool IsAuthorizationPermissionMismatchError(Exception ex)
        {
            throw new NotImplementedException();
        }

        protected override bool IsBlobAlreadyExistsError(Exception ex)
        {
            throw new NotImplementedException();
        }

        protected override bool IsAuthorizationError(Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}
