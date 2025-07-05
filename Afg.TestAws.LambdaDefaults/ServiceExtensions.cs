using Amazon.Lambda;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using LocalStack.Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Afg.TestAws.LambdaDefaults;

public static class ServiceExtensions
{
    public static TBuilder AddAspireLocalStack<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddLocalStack(builder.Configuration)
            .AddDefaultAwsOptions(builder.Configuration.GetAWSOptions());
        
        builder.Services.AddAwsService<IAmazonLambda>();
        builder.Services.AddAwsService<IAmazonSQS>();
        builder.Services.AddAwsService<IAmazonSimpleNotificationService>();

        return builder;
    }
    
    public static IServiceCollection AddAspireLocalStack(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLocalStack(configuration)
            .AddDefaultAwsOptions(configuration.GetAWSOptions());
        
        services.AddAwsService<IAmazonLambda>();
        services.AddAwsService<IAmazonSQS>();
        services.AddAwsService<IAmazonSimpleNotificationService>();

        return services;
    }
}