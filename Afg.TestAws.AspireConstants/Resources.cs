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

    public const string Cdk = "Cdk";

    public const string TestQueue = "TestQueue";
    public const string TestTopic = "TestTopic";
    
}