using Afg.TestAws.AppHost;
using Afg.TestAws.AspireConstants;
using Amazon.CDK;
using Amazon.CDK.AWS.SQS;

var builder = DistributedApplication.CreateBuilder(args);



var localStack = builder.AddContainer(Resources.LocalStack, LocalStackConfig.ImageName)
    .WithEnvironment(LocalStackConfig.Services.Label, LocalStackConfig.Services.Value)
    .WithEnvironment(AwsConfig.DefaultRegion.Label, AwsConfig.DefaultRegion.Value)
    .WithHttpEndpoint(4566, 4566)
    .WithHttpHealthCheck("/_localstack/health");// Enable debug logging

var lsConfig = builder.ConfigureLocalStack().WaitFor(localStack);

var awsConfig = builder.AddAWSSDKConfig()
    .WithRegion(Amazon.RegionEndpoint.USEast1);

var cdk = builder.AddAWSCDKStack(Resources.Cdk)
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