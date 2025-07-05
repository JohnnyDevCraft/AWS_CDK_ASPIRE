using Afg.TestAws.AspireConstants;

namespace Afg.TestAws.AppHost;

public static class LocalStackExtensions
{
    public static IResourceBuilder<T> WithLocalStack<T>(this IResourceBuilder<T> builder)
        where T : ProjectResource
    {
        // Configure the resource with LocalStack details
        builder.WithEnvironment("LocalStack__UseLocalStack", "true")
            .WithEnvironment("LocalStack__LocalStackHost", LocalStackConfig.LocalStackHost)
            .WithEnvironment("LocalStack__EdgePort", LocalStackConfig.LocalStackPort.ToString())
            .WithEnvironment("LocalStack__Region", LocalStackConfig.Region)
            .WithEnvironment("AWS__Region", LocalStackConfig.Region)
            .WithEnvironment("AWS__Profile", LocalStackConfig.AwsProfile);
        
        return builder;
    }
    
    public static IResourceBuilder<ExecutableResource> ConfigureLocalStack(this IDistributedApplicationBuilder builder)
    {
        
        
        var script = builder
            .AddExecutable(LocalStackConfig.SetupLocalStack, "nvm use 22 && cdklocal bootstrap aws://000000000000/us-east-1 \\\n  --bootstrap-bucket-name cdktoolkit-deploys \\\n  --qualifier hnb659fds", 
                ".")
            .WithEnvironment("AWS_ACCESS_KEY", "test")
            .WithEnvironment("AWS_SECRET_ACCESS_KEY", "test")
            .WithEnvironment("AWS_REGION", LocalStackConfig.Region);
        
        
        // Add LocalStack configuration to the application builder
        var endpoint = $"http://{LocalStackConfig.LocalStackHost}:{LocalStackConfig.LocalStackPort}";

        // — for the *current* process (so CDK’s CloudFormation calls go local) —
        Environment.SetEnvironmentVariable("AWS_ENDPOINT_URL",           endpoint);
        Environment.SetEnvironmentVariable("AWS_ENDPOINT_URL_CLOUDFORMATION", endpoint);
        Environment.SetEnvironmentVariable("AWS_ENDPOINT_URL_SQS",       endpoint);
        Environment.SetEnvironmentVariable("AWS_ENDPOINT_URL_LAMBDA",    endpoint);
        Environment.SetEnvironmentVariable("AWS_ENDPOINT_URL_SNS",       endpoint);
        Environment.SetEnvironmentVariable("AWS_ENDPOINT_URL_STS",       endpoint);
        
        return script;
    }
}