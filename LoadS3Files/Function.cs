using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Util;
using LoadS3Files.Data;
using LoadS3Files.Service;
using LoadS3Files.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Globalization;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LoadS3Files;

public class Function
{
    IAmazonS3 S3Client { get; set; }
    IConfiguration Configuration { get; set; }
    public string FileName { get; set; }

    private readonly BasicAWSCredentials _credentials;
    NutriFileImporterService NutriFileService { get; set; }
  
    public Function()
    {
        S3Client = new AmazonS3Client();
    }

    public Function(IAmazonS3 s3Client)
    {
        this.S3Client = s3Client;
    }

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
                FileName = GetFileName(s3Event.Object.Key);
                var response = S3Client.GetObjectAsync(s3Event.Bucket.Name, s3Event.Object.Key).Result;

                string importedPath = PathConfiguration.GetPath("importedFiles", FileName);
                var stream = response.ResponseStream;


                var Imported = NutriFileService.ProcessNutriFile(stream, s3Event);

                if (Imported)
                    NutriFileService.MoveFile(S3Client, s3Event.Bucket.Name, s3Event.Object.Key, importedPath);

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

    private string GetFileName(string key)
    {
        string[] splitKey = key.Split('/');

        return splitKey[splitKey.Length -1];
    }

    private void Initialize()
    {
        var cultureInfo = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        var serviceCollection = new ServiceCollection();

        var serviceProvider = new ServiceCollection()
       .AddSingleton<NutriFileImporterService>()
       .AddScoped<INutriRepository>(r => new NutriRepository("Data Source=DESKTOP-FECD75M\\SQLEXPRESS;Initial Catalog=Nutricionista;Trusted_Connection=True;"))
       .BuildServiceProvider();

        NutriFileService = serviceProvider.GetService<NutriFileImporterService>();
    }

}