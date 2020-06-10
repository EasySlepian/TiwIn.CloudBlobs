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

        


        static async Task Main(string[] args)
        {
            var connectionString =new AwsS3ConnectionStringBuilder()
            {
                AccessKey = GetEnvVariable("AccessKey"),
                SecretKey = GetEnvVariable("SecretKey"),
                RegionEndpointSystemName = GetEnvVariable("RegionSystemName")
            };

            var provider = AwsBlobStoreProvider.Create();
            var store = provider.GetBlobStore(connectionString.ConnectionStringBuilder);

            var text = await store.ReadAllTextAsync(bucketName, keyName);

            var credentials = new BasicAWSCredentials(connectionString.AccessKey, connectionString.SecretKey);
            var client = new AmazonS3Client(credentials, connectionString.RegionEndpoint);
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = keyName
            };
            

            using var response = await client.GetObjectAsync(request);
            await using var responseStream = response.ResponseStream;
            using var reader = new StreamReader(responseStream);
            string title = response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
            string contentType = response.Headers["Content-Type"];
            Console.WriteLine("Object metadata, Title: {0}", title);
            Console.WriteLine("Content type: {0}", contentType);

            var result = await reader.ReadToEndAsync(); // Now you process the response body.
        }

        private static string GetEnvVariable(string key)
        {
            var value = Environment.GetEnvironmentVariable(key);
            return string.IsNullOrWhiteSpace(value) ? throw new InvalidOperationException($"'{key}' environment variable is missing") : value;
        }
    }
}
