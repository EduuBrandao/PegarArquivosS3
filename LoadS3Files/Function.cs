using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Util;
using LoadS3Files.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Globalization;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LoadS3Files;

public class Function
{
    IAmazonS3 S3Client { get; set; }
    IConfiguration Configuration { get; set; }
    public string FileName { get; set; }


    private readonly BasicAWSCredentials _credentials;
    NutriFileImporterService NutriFileService { get; set; }
    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {
        S3Client = new AmazonS3Client();
    }

    /// <summary>
    /// Constructs an instance with a preconfigured S3 client. This can be used for testing outside of the Lambda environment.
    /// </summary>
    /// <param name="s3Client"></param>
    public Function(IAmazonS3 s3Client)
    {
        this.S3Client = s3Client;
    }

    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
    /// to respond to S3 notifications.
    /// </summary>
    /// <param name="evnt"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler(S3Event evnt, ILambdaContext context)
    {
        Initialize();
        var eventRecords = evnt.Records ?? new List<S3Event.S3EventNotificationRecord>();
        foreach (var record in eventRecords)
        {
            var s3Event = record.S3;
            if (s3Event == null)
            {
                continue;
            }

            try
            {
                var response = S3Client.GetObjectAsync(s3Event.Bucket.Name, s3Event.Object.Key).Result;

                var stream = response.ResponseStream;


                var Imported = NutriFileService.ProcessNutriFile(stream, s3Event);
                using(StreamReader reader = new StreamReader(response.ResponseStream))
                {
                    List<string> fileLines = new List<string>();
                    while(!reader.EndOfStream)
                    {
                        context.Logger.LogLine(reader.ReadLine());
                    }
                }
            }
            catch (Exception e)
            {
                context.Logger.LogError($"Error getting object {s3Event.Object.Key} from bucket {s3Event.Bucket.Name}. Make sure they exist and your bucket is in the same region as this function.");
                context.Logger.LogError(e.Message);
                context.Logger.LogError(e.StackTrace);
                throw;
            }
        }


    }

    private void Initialize()
    {
        var cultureInfo = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        var serviceCollection = new ServiceCollection();

        var serviceProvider = new ServiceCollection()
     .AddSingleton<NutriFileImporterService>()
     .BuildServiceProvider();

        NutriFileService = serviceProvider.GetService<NutriFileImporterService>();
    }

}