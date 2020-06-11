namespace Samples.TiwIn.CloudBlobs.AWSSDK.S3
{
    using Amazon.S3;
    using Amazon.S3.Transfer;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Amazon;
    using Amazon.Runtime;
    using Amazon.S3.Model;
    using global::TiwIn.CloudBlobs.AWSSDK.S3;

    class Program
    {
        private const string bucketName = "my-aws-bucket2";
        private const string keyName = "README.md";

        


        static async Task<int> Main(string[] args)
        {
            var connectionString =new AwsS3ConnectionStringBuilder()
            {
                AccessKey = GetEnvVariable("AccessKey"),
                SecretKey = GetEnvVariable("SecretKey"),
                RegionEndpointSystemName = GetEnvVariable("RegionSystemName")
            };

            // Creates an AWS Store Blobs implementation of store provider
            var provider = AwsBlobStoreProvider.Create();

            // Create a Cloud Blob Store object which will be used to issue commands against the storage account 
            var store = provider.GetBlobStore(connectionString.ConnectionStringBuilder);


            // Create the my-container collection
            await store.CreateCollectionIfNotExistsAsync("easyslepian");

            // Upload blobs to a container
            try
            {
                await store.WriteAllTextAsync("easyslepian", "my-blob.txt", "Hello world!");
            }
            catch (Exception ex) when (store.IsBlobAlreadyExistsError(ex))
            {
                Console.WriteLine("my-blob.txt already exists.");
            }

            // List all blobs in the container
            await foreach (var blobItem in store.GetBlobsAsync("easyslepian"))
            {
                Console.WriteLine("\t" + blobItem.BlobName);
            }

            return 0;
        }

        private static string GetEnvVariable(string key)
        {
            var value = Environment.GetEnvironmentVariable(key);
            return string.IsNullOrWhiteSpace(value) ? throw new InvalidOperationException($"'{key}' environment variable is missing") : value;
        }
    }
}
