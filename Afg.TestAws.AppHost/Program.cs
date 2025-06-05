using Amazon.CDK;
using Amazon.CDK.AWS.SQS;

var builder = DistributedApplication.CreateBuilder(args);

var cdk = builder.AddAWSCDKStack("cdk");

var sqs = cdk.AddSQSQueue("TestQueue", new QueueProps()
{
    QueueName = "TestQueue",
    VisibilityTimeout = Duration.Seconds(30),
    RetentionPeriod = Duration.Days(4),
    ReceiveMessageWaitTime = Duration.Seconds(20)
});

var sender = builder.AddAWSLambdaFunction<Projects.Afg_TestAws_Sender>("Sender", "Afg.TestAws.Sender")
    .WithReference(sqs)
    .WaitFor(sqs);

var receiver = builder.AddAWSLambdaFunction<Projects.Afg_TestAws_Reciever>("Receiver", "Afg.TestAws.Receiver")
    .WithReference(sqs)
    .WaitFor(sqs)
    .WithSQSEventSource(sqs);

builder.Build().Run();