namespace TiwIn.CloudBlobs.AWSSDK.S3
{
    using System.Threading.Tasks;
    using Common;

    sealed class AwsBlobInfo : BlobInfo
    {
        public AwsBlobInfo(string collectionName, string blobName, AwsBlobStore store) 
            : base(collectionName, blobName, AwsBlobStoreInfoService.Instance)
        {
        }

        protected override Task<object> ReadAllTextAsync(BlobReadOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}
