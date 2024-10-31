using Amazon.Runtime;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime.CredentialManagement;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace VertexAWSProvider
{
    public class VertexProvider : AWSCredentials
    {
        private readonly AWSCredentials _defaultCredentials;
        private readonly AWSCredentials _customCredentials;

        public VertexProvider(string profile, string configPath)
        {
            _defaultCredentials = FallbackCredentialsFactory.GetCredentials();

            if (string.IsNullOrEmpty(configPath))
            {
                configPath = Path.Combine(Directory.GetCurrentDirectory(), "aws.config");
            }

            if (string.IsNullOrEmpty(profile))
            {
                profile = "win32";
            }

            var options = new AWSOptions
            {
                Profile = profile,
                ProfilesLocation = configPath
            };

            if (File.Exists(configPath))
            {
                var serviceProvider = new ServiceCollection()
                    .AddDefaultAWSOptions(options)
                    .BuildServiceProvider();

                var awsOptions = serviceProvider.GetService<AWSOptions>();
                var chain = new CredentialProfileStoreChain(awsOptions.ProfilesLocation);
                if (chain.TryGetAWSCredentials(options.Profile, out var credentials))
                {
                    _customCredentials = credentials;
                }
                else
                {
                    _customCredentials = null;
                }
            }
            else
            {
                _customCredentials = null;
            }
        }

        public override ImmutableCredentials GetCredentials()
        {
            return _customCredentials != null ? _customCredentials.GetCredentials() : _defaultCredentials.GetCredentials();
        }
    }
}
