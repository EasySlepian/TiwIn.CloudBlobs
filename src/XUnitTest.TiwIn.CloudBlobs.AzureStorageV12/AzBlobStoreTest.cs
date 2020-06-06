//-----------------------------------------------------------------------
// <copyright file="AzBlobStoreTest.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using Azure.Storage.Blobs;

    public abstract class AzBlobStoreTest
    {
        protected AzBlobStoreTest()
        {
            TestContainerName = GetType().GUID.ToString("N");
            BlobServiceClient = new BlobServiceClient(Configurations.ConnectionString);
            Store = AzBlobStoreProvider.Parse(Configurations.ConnectionString);
            TestContainer = BlobServiceClient.GetBlobContainerClient(TestContainerName);
            TestContainer.CreateIfNotExists();
        }

        protected string TestContainerName { get; }
        public BlobServiceClient BlobServiceClient { get; }
        protected IBlobStore Store { get; }

        protected BlobContainerClient TestContainer { get; }
    }
}
