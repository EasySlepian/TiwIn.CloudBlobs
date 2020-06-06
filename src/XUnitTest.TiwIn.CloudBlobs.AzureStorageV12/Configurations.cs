//-----------------------------------------------------------------------
// <copyright file="Configurations.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.CloudBlobs.AzureStorageV12
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class Configurations
    {
        private const string LaunchSettings = "Properties\\launchSettings.json";


        static Configurations()
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            if (!string.IsNullOrWhiteSpace(connectionString) || !File.Exists(LaunchSettings)) return;
            using var file = File.OpenText("Properties\\launchSettings.json");
            var reader = new JsonTextReader(file);
            var jObject = JObject.Load(reader);

            var variables = (jObject
                    .GetValue("profiles") ?? throw new InvalidOperationException())
                //select a proper profile here
                .SelectMany(profiles => profiles.Children())
                .SelectMany(profile => profile.Children<JProperty>())
                .Where(prop => prop.Name == "environmentVariables")
                .SelectMany(prop => prop.Value.Children<JProperty>())
                .ToList();

            foreach (var variable in variables)
            {
                Environment.SetEnvironmentVariable(variable.Name, variable.Value.ToString());
            }
        }

        public static string ConnectionString => GetEnvVariable("ConnectionString");

        private static string GetEnvVariable(string key)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if(string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"{key} environment variable is missing.");
            return value;
        }
    }
}
