using System.Text.Json;
using Afg.TestAws.LambdaDefaults;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Annotations;
using Amazon.SQS;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Afg.TestAws.Sender;

public class Functions
{
    private static readonly IAmazonSQS _sqsClient;
    private static readonly string _queueUrl;

    // Static ctor: build config from env, then create SQS client
    static Functions()
    {
        // 1) Build IConfiguration from environment variables (Aspire injected them)
        var cfg = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        // 2) Setup DI container
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(cfg);

        // 3) If USE_LOCALSTACK=true, wire up LocalStack defaults
        services.AddAspireLocalStack(cfg);

        // 4) Build the service provider
        var serviceProvider = services.BuildServiceProvider();

        // 5) Get services from DI with Scope
        using var serviceScope = serviceProvider.CreateScope();
        var scopedProvider = serviceScope.ServiceProvider;

        _sqsClient = scopedProvider.GetRequiredService<IAmazonSQS>();
        _queueUrl = scopedProvider.GetRequiredService<IConfiguration>()
                        .GetValue<string>("Aspire:Resources:TestQueue:QueueUrl")
                    ?? throw new InvalidOperationException("Missing TestQueue URL");
    }


    /// <summary>
    /// A Lambda function to respond to HTTP Get methods from API Gateway
    /// </summary>
    /// <remarks>
    /// This uses the <see href="https://github.com/aws/aws-lambda-dotnet/blob/master/Libraries/src/Amazon.Lambda.Annotations/README.md">Lambda Annotations</see> 
    /// programming model to bridge the gap between the Lambda programming model and a more idiomatic .NET model.
    /// 
    /// This automatically handles reading parameters from an APIGatewayProxyRequest
    /// as well as syncing the function definitions to serverless.template each time you build.
    /// 
    /// If you do not wish to use this model and need to manipulate the API Gateway 
    /// objects directly, see the accompanying Readme.md for instructions.
    /// </remarks>
    /// <param name="context">Information about the invocation, function, and execution environment</param>
    /// <returns>The response as an implicit <see cref="APIGatewayProxyResponse"/></returns>
    [LambdaFunction]
    public async Task<bool> Get(MessageDto request, ILambdaContext context, CancellationToken cancellationToken)
    {
        try
        {
            Guard.Against.Null(request, nameof(request));
            Guard.Against.NullOrEmpty(request.Message, nameof(request.Message));
            Guard.Against.NullOrEmpty(request.Sender, nameof(request.Sender));
            Guard.Against.NullOrEmpty(request.Receiver, nameof(request.Receiver));

            var resp = await _sqsClient.SendMessageAsync(_queueUrl,
                JsonSerializer.Serialize(request), cancellationToken);

            context.Logger.LogLine($"Sent message: {JsonSerializer.Serialize(resp)}");

            return true;
        }
        catch (Exception ex)
        {
            context.Logger.LogError(ex, "An error occurred while processing the request");
            return false;
        }
    }
}


