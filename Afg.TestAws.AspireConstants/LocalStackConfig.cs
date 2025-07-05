namespace Afg.TestAws.AspireConstants;

public static class LocalStackConfig
{
    // Env var name
    public const string UseLocalStackEnvVar = "USE_LOCALSTACK";

    // Configuration keys (for appsettings or DI)
    public const string LocalStackSection = "LocalStack";
    public const string AwsSection        = "AWS";

    // Hard-coded defaults for local development
    public const string LocalStackHost    = "localhost.localstack.cloud";
    public const int    LocalStackPort    = 4566;
    public const string Region            = "us-east-1";
    public const string AwsProfile        = "default";

    public const string ImageName = "localstack/localstack:latest";


    public const string SetupLocalStack = "SetupLocalStack";
    

    
    public static class Services
    {
        public const string Label = "SERVICES";
        public const string Value = "sns,sqs,lambda,cloudformation,sts,ssm,dynamodb,apigateway,events";
    }
    
}