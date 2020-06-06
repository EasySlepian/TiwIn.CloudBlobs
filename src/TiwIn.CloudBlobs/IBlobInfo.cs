//-----------------------------------------------------------------------
// <copyright file="IBlobInfo.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System;
    using System.Collections.Immutable;
    using System.Threading.Tasks;

    public interface IBlobInfo : IBlobStoreInfoService
    {
        string BlobName { get; }
        string CollectionName { get; }

        string ContentType { get; }
        string ContentEncoding { get; }
        string ContentLanguage { get;  }
        long? ContentLength { get; }

        DateTimeOffset? CreatedOn { get; }

        IImmutableDictionary<string, string> Metadata { get; }
        Task<object> ReadAllTextAsync(Action<BlobReadOptions> config = null);
    }
}
