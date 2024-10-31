# VertexAWSProvider

A custom AWS credential provider for .NET applications that reads from an `aws.config` ini file when present and falls back to the default AWS credential provider chain if the file is not found.

## Installation

```sh
dotnet add package VertexAWSProvider
```

## Usage

```csharp
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using AwsCustomProvider;

class Program
{
    static void Main(string[] args)
    {
        var credentials = new VertexProvider(); // Uses win32 profile and config path
        var client = new AmazonSecretsManagerClient(credentials);

        var secretName = "YOUR_SECRET_NAME"; // Replace with your secret name

        var request = new GetSecretValueRequest
        {
            SecretId = secretName
        };

        try
        {
            var response = client.GetSecretValueAsync(request).Result;
            if (response.SecretString != null)
            {
                Console.WriteLine("Secret Value: " + response.SecretString);
            }
            else
            {
                Console.WriteLine("Secret is binary data, not a string.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error retrieving secret: " + e.Message);
        }
    }
}
```

You can also specify custom profile and config path:

```csharp
var credentials = new VertexProvider("profileName", "path/to/aws.config");
```
