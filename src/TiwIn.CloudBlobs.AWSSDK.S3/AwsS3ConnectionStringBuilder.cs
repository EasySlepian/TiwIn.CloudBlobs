namespace TiwIn.CloudBlobs.AWSSDK.S3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Amazon;

    public sealed class AwsS3ConnectionStringBuilder
    {
        readonly Regex _connectionStringRegex;
        public AwsS3ConnectionStringBuilder()
        {
            _connectionStringRegex = new Regex(@"(?xis-m)^@pair(?:;@pair){2};?$"
                .Replace("@pair", @"(?<key>\w+)=(?<value>[^;]+)"));
        }



        public AwsS3ConnectionStringBuilder(string connectionString) : this()
        {
            if(string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string is required.");
            connectionString = Regex.Replace(connectionString, @"\s+", string.Empty);
            var pairs =
                (from tp in connectionString.Split(";")
                    let match = Regex.Match(tp, @"([^=]+)=(\S+)")
                    where match.Success
                    select KeyValuePair.Create(match.Groups[1].Value, match.Groups[2].Value))
                .ToDictionary(p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase);
            AccessKey = pairs.TryGetValue("AccessKey", out var accessKey)
                ? accessKey
                : throw new ArgumentException($"AccessKey value is missing");
            SecretKey = pairs.TryGetValue("SecretKey", out var secretKey)
                ? secretKey
                : throw new ArgumentException($"SecretKey value is missing");
            RegionEndpointSystemName = pairs.TryGetValue("Region", out var region)
                ? region
                : throw new ArgumentException($"Region value is missing");

        }

        public string AccessKey { get; set; }
        public string SecretKey { get; set; }

        public string RegionEndpointSystemName
        {
            get => RegionEndpoint?.SystemName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    RegionEndpoint = null;
                    return;
                }

                RegionEndpoint = RegionEndpoint.GetBySystemName(value.Trim());
            }
        }

        public RegionEndpoint RegionEndpoint
        {
            get;
            set;
        }

        public string ConnectionStringBuilder => Build();

        public string Build()
        {
            return $"AccessKey={AccessKey};SecretKey={SecretKey};Region={RegionEndpoint.SystemName}";
        }
    }
}
