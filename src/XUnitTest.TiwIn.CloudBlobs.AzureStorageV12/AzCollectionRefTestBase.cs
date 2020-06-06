//-----------------------------------------------------------------------
// <copyright file="AzCollectionRefTestBase.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using Azure.Storage.Blobs;

    public abstract class AzCollectionRefTestBase
    {
        private IBlobStore _store;

        protected AzCollectionRefTestBase()
        {
            Provider = AzBlobStoreProvider.Create();
            TestContainerName = GetType().GUID.ToString("N");
            BlobServiceClient = new BlobServiceClient(Configurations.ConnectionString);
            _store = AzBlobStoreProvider.Parse(Configurations.ConnectionString);
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
            var uri = _store.GetCollectionUri(TestContainerName, config);
            return Provider.GetCollectionRef(uri);
        }

        protected Uri CreateCollectionUri(Action<SignCollectionUriOptions> config) =>
            _store.GetCollectionUri(TestContainerName, config);

        protected ICollectionRef CreateCollectionRef(string collectionName, Action<SignCollectionUriOptions> config)
        {
            var uri = _store.GetCollectionUri(collectionName, config);
            return Provider.GetCollectionRef(uri);
        }
    }
}
