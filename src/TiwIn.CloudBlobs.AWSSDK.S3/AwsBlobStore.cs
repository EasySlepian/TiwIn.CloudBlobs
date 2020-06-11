namespace TiwIn.CloudBlobs.AWSSDK.S3
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Amazon;
    using Amazon.Runtime;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Amazon.S3.Util;
    using Common;
    using Extensions;

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

        protected override async Task<bool> CreateCollectionIfNotExistsAsync(string collectionName, CollectionCreateOptions options)
        {
            if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, collectionName))
                return false;
            var putBucketRequest = new PutBucketRequest
            {
                BucketName = collectionName,
                UseClientRegion = true
            };
  
            var putBucketResponse = await _client.PutBucketAsync(putBucketRequest);
            return putBucketResponse.HttpStatusCode == HttpStatusCode.Accepted;
        }

        protected override async IAsyncEnumerable<IBlobInfo> GetBlobsAsync(string collectionName, ListBlobsOptions options)
        {
            var request = new ListObjectsV2Request
            {
                BucketName = collectionName,
                MaxKeys = 10
            };
            ListObjectsV2Response response;
            do
            {
                response = await _client.ListObjectsV2Async(request);

                // Process the response.
                foreach (var entry in response.S3Objects)
                {
                    yield return new AwsBlobInfo(entry.BucketName, entry.Key, this);
                }
                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);
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
            return text.ProcessAsStreamAsync(async stream =>
            {
                var response = await _client.PutObjectAsync(new PutObjectRequest()
                {
                    BucketName = collectionName, 
                    Key = blobName,
                    InputStream = stream
                });

            });
   
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
