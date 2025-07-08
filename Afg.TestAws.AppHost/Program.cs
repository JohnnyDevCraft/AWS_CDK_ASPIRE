using Afg.TestAws.AppHost;
using Afg.TestAws.AspireConstants;
using Amazon.CDK;
using Amazon.CDK.AWS.SQS;
using Environment = System.Environment;

var builder = DistributedApplication.CreateBuilder(args);



var localStack = builder.AddContainer(Resources.LocalStack, LocalStackConfig.ImageName)
    .WithEnvironment(LocalStackConfig.Services.Label, LocalStackConfig.Services.Value)
    .WithEnvironment(AwsConfig.DefaultRegion.Label, AwsConfig.DefaultRegion.Value)
    .WithHttpEndpoint(4566, 4566)
    .WithHttpHealthCheck("/_localstack/health");// Enable debug logging

var lsConfig = builder.AddProject<Projects.Afg_TestAws_LocalStackConfigurator>(Resources.LocalStackConfigurator.Name)
    .WaitFor(localStack);

Environment.SetEnvironmentVariable(Resources.AwsCdk.AccessKey.Label, Resources.AwsCdk.AccessKey.Value);
Environment.SetEnvironmentVariable(Resources.AwsCdk.SecretKey.Label, Resources.AwsCdk.SecretKey.Value);
Environment.SetEnvironmentVariable("AWS_SESSION_TOKEN", string.Empty);

var awsConfig = builder.AddAWSSDKConfig()
    .WithRegion(Amazon.RegionEndpoint.USEast1);

var cdk = builder.AddAWSCDKStack(Resources.AwsCdk.StackName)
    .WaitFor(localStack)
    .WaitForCompletion(lsConfig);

var sqs = cdk.AddSQSQueue(Resources.TestQueue, new QueueProps()
{
    QueueName = Resources.TestQueue,
    VisibilityTimeout = Duration.Seconds(30),
    RetentionPeriod = Duration.Days(4),
    ReceiveMessageWaitTime = Duration.Seconds(20)
});

var sender = builder.AddAWSLambdaFunction<Projects.Afg_TestAws_Sender>(Resources.Sender.Name, Resources.Sender.Handler)
    .WithReference(sqs)
    .WaitFor(sqs)
    .WaitFor(cdk)
    .WithLocalStack();

var receiver = builder.AddAWSLambdaFunction<Projects.Afg_TestAws_Reciever>(Resources.Receiver.Name, Resources.Receiver.Handler)
    .WithReference(sqs)
    .WaitFor(sqs)
    .WithSQSEventSource(sqs)
    .WaitFor(cdk)
    .WithLocalStack();

builder.Build().Run();