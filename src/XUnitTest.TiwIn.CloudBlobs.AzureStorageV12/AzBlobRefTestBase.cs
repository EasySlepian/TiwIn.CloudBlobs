//-----------------------------------------------------------------------
// <copyright file="AzBlobRefTestBase.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using Azure.Storage.Blobs;

    public abstract class AzBlobRefTestBase
    {
        public IBlobStore Store { get; }

        protected AzBlobRefTestBase()
        {
            Provider = AzBlobStoreProvider.Create();
            TestContainerName = GetType().GUID.ToString("N");
            BlobServiceClient = new BlobServiceClient(Configurations.ConnectionString);
            Store = AzBlobStoreProvider.Parse(Configurations.ConnectionString);
            TestContainer = BlobServiceClient.GetBlobContainerClient(TestContainerName);
            TestContainer.CreateIfNotExists();
        }

        protected string TestContainerName { get; }
        public BlobServiceClient BlobServiceClient { get; }
        //protected IBlobStore Store { get; }

        protected BlobContainerClient TestContainer { get; }

        protected IBlobStoreProvider Provider { get; }

        protected ICollectionRef CreateCollectionRef(Action<SignCollectionUriOptions> config)
        {
            var uri = Store.GetCollectionUri(TestContainerName, config);
            return Provider.GetCollectionRef(uri);
        }

        protected Uri CreateCollectionUri(Action<SignCollectionUriOptions> config) =>
            Store.GetCollectionUri(TestContainerName, config);

        protected ICollectionRef CreateCollectionRef(string collectionName, Action<SignCollectionUriOptions> config)
        {
            var uri = Store.GetCollectionUri(collectionName, config);
            return Provider.GetCollectionRef(uri);
        }

        protected IBlobRef CreateBlobRef(string blobName, Action<SignBlobUriOptions> config)
        {
            var uri = Store.GetBlobUri(TestContainerName, blobName, config);
            return Provider.GetBlobRef(uri);
        }
    }
}
