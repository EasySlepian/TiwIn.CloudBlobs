namespace TiwIn.CloudBlobs.AWSSDK.S3
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Amazon;
    using Amazon.Runtime;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Common;

    class AwsBlobStore : BlobStore
    {
        private readonly AmazonS3Client _client;
        public AwsBlobStore(string accessKey, string secretKey, RegionEndpoint regionEndpoint) : base(AwsBlobStoreInfoService.Instance)
        {
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            _client = new AmazonS3Client(credentials, regionEndpoint);
        }

        protected override Uri GetCollectionUri(string collectionName, SignCollectionUriOptions options)
        {
            throw new NotImplementedException();
        }

        protected override Uri GetBlobUri(string collectionName, string blobName, SignBlobUriOptions options)
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> DeleteCollectionIfExistsAsync(string collectionName, CollectionDeleteOptions options)
        {
            throw new NotImplementedException();
        }

        protected override Task LockAsync(string collectionName, string blobName, BlobLockedCallback callback)
        {
            throw new NotImplementedException();
        }

        protected override Task CreateCollectionAsync(string collectionName, CollectionCreateOptions options)
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> CreateCollectionIfNotExistsAsync(string collectionName, CollectionCreateOptions options)
        {
            throw new NotImplementedException();
        }

        protected override IAsyncEnumerable<IBlobInfo> GetBlobsAsync(string collectionName, ListBlobsOptions options)
        {
            throw new NotImplementedException();
        }

        protected override Task<Stream> OpenReadAsync(string collectionName, string blobName, BlobReadOptions options)
        {
            throw new NotImplementedException();
        }

        protected override async Task<string> ReadAllTextAsync(string collectionName, string blobName, BlobReadOptions config)
        {
            var request = new GetObjectRequest
            {
                BucketName = collectionName,
                Key = blobName
            };

            using var response = await _client.GetObjectAsync(request);
            await using var responseStream = response.ResponseStream;
            using var reader = new StreamReader(responseStream);
            return await reader.ReadToEndAsync();
        }

        protected override Task<byte[]> ReadAllBytesAsync(string collectionName, string blobName, BlobReadOptions config)
        {
            throw new NotImplementedException();
        }

        protected override Task WriteAllTextAsync(string collectionName, string blobName, string text, BlobWriteTextOptions options)
        {
            throw new NotImplementedException();
        }

        protected override Task DeleteBlobAsync(string collectionName, string blobName, DeleteBlobOptions options)
        {
            throw new NotImplementedException();
        }

        protected override Task DeleteCollectionAsync(string collectionName, CollectionDeleteOptions options)
        {
            throw new NotImplementedException();
        }

        protected override Task UploadAsync(string collectionName, string blobName, Stream input, BlobUploadOptions options)
        {
            throw new NotImplementedException();
        }

        protected override Task<IBlobInfo> GetBlobInfoAsync(string collectionName, string blobName, BlobLoadInfoOptions options)
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> DeleteBlobIfExistsAsync(string collectionName, string blobName, DeleteBlobOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
