namespace TiwIn.CloudBlobs.AWSSDK.S3
{
    using System;
    using Common;

    public sealed class AwsBlobStoreProvider : BlobStoreProvider
    {

        private AwsBlobStoreProvider() : base(AwsBlobStoreInfoService.Instance)
        {
        }

        protected override IBlobStore CreateBlobStore(string connectionString)
        {
            var builder = new AwsS3ConnectionStringBuilder(connectionString);
            return new AwsBlobStore(builder.AccessKey, builder.SecretKey, builder.RegionEndpoint);
        }

        protected override IBlobRef GetBlobRef(Uri preSignedUri)
        {
            throw new NotImplementedException();
        }

        protected override ICollectionRef GetCollection(Uri signedUri)
        {
            throw new NotImplementedException();
        }

        public static IBlobStoreProvider Create() => new AwsBlobStoreProvider();
    }
}
