namespace Afg.TestAws.AspireConstants;

public static class Resources
{
    public const string LocalStack = "LocalStack";
    public const string DynamoDb = "DynamoDb";
    public const string TestDb = "TestDb";
    public const string Sns = "Sns";
    public const string Sqs = "Sqs";
    
    public static class Sender
    {
        public const string Name = "Sender";
        public const string FunctionName = "SenderFunction";
        public const string Handler = "Afg.TestAws.Sender::Afg.TestAws.Sender.Function::FunctionHandler";
    }
    
    public static class Receiver
    {
        public const string Name = "Receiver";
        public const string FunctionName = "ReceiverFunction";
        public const string Handler = "Afg.TestAws.Receiver::Afg.TestAws.Receiver.Function::FunctionHandler";
    }
    
    public static class LocalStackConfigurator
    {
        public const string Name = "LocalStackConfigurator";
    }

    public static class AwsCdk
    {
        public const string Name = "Cdk";
        public const string StackName = "AwsCdkStack";

        public static class AccessKey
        {
            public const string Label = "AWS_ACCESS_KEY_ID";
            public const string Value = "test";
        }

        public static class SecretKey
        {
            public const string Label = "AWS_SECRET_ACCESS_KEY";
            public const string Value = "test";
        }
    }

    public const string TestQueue = "TestQueue";
    public const string TestTopic = "TestTopic";
    
}