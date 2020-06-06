//-----------------------------------------------------------------------
// <copyright file="BlobUploadOptions.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    public class BlobUploadOptions
    {
        private Dictionary<string, string> _metadata;

        internal BlobUploadOptions()
        {
            Overwrite = false;
        }

        [DebuggerStepThrough]
        public BlobUploadOptions AddProperty(string key, string value)
        {
            LazyInitializer.EnsureInitialized(ref _metadata, () => new Dictionary<string, string>());
            _metadata[key] = value;
            return this;
        }

        public bool Overwrite { get; set; }

        public string ContentLanguage { get; set; }
        public string ContentType { get; set; }
        public string ContentEncoding { get; set; }
        public IDictionary<string, string> Metadata => _metadata;
    }
}
