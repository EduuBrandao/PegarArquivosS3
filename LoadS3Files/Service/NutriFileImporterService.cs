using Amazon;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.SQS;
using Amazon.SQS.Model;
using LoadS3Files.Data;
using LoadS3Files.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amazon.Lambda.S3Events.S3Event;

namespace LoadS3Files.Service
{
    public class NutriFileImporterService
    {

        private StreamReader _reader;
        private readonly INutriRepository _nutriRepository;
        public NutriFileImporterService(INutriRepository nutriRepository)
        {
            _nutriRepository = nutriRepository;
        }

        public void MoveFile(IAmazonS3 s3Client, string bucketName, string originPath, string destinyPath)
        {
            try
            {
                s3Client.CopyObjectAsync(bucketName, originPath, bucketName, destinyPath).Wait();
                s3Client.DeleteObjectAsync(bucketName, originPath).Wait();
            }
            catch (Exception e)
            {
                LambdaLogger.Log($"Erro inesperado ao mover arquivo: {e.Message}");

                throw;
            }
        }

        public bool ProcessNutriFile(Stream stream, S3Entity s3Event)
        {
            _reader = new StreamReader(stream, Encoding.GetEncoding("iso-8859-1"));

            NutriFileDTO nutriDTO =  BuildFile();


            //metodo para alocar nossos dados em uma Fila
            QueueUp(nutriDTO);

            return true;
        }

        private async void QueueUp(NutriFileDTO nutriDTO)
        {
            foreach (var customer in nutriDTO.Customers)
            {
                string json = JsonConvert.SerializeObject(customer);

                var client = new AmazonSQSClient(RegionEndpoint.SAEast1);
                var request = new SendMessageRequest
                {
                    QueueUrl = "https://sqs.sa-east-1.amazonaws.com/254632350317/NutriIsa",
                    MessageBody = json
                };

                await client.SendMessageAsync(request);
            }

        }

        private NutriFileDTO BuildFile()
        {
            try
            {
                NutriFileDTO nutriDTO = new NutriFileDTO();
                List<string> fileLines = new List<string>();

                while (!_reader.EndOfStream)
                {
                    var fileLine = _reader.ReadLine().Replace("'", "");

                    fileLines.Add(fileLine);

                    if (fileLine != null)
                    {
                        switch (fileLine.Split(";")[0])
                        {
                            case "1":
                                nutriDTO.Customers.Add(CreateNutriCustomerFileDTO(fileLine));
                                break;
                        }
                    }
                }

               
                _reader.Close();
                _reader.Dispose();

                return nutriDTO;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void CreateUser(NutriFileDTO nutriDTO)
        {
            _nutriRepository.CreateCustomer(nutriDTO.Customers.Select(x => x.Name).First());


        }

        private NutriCustomerFileDTO CreateNutriCustomerFileDTO(string fileLine)
        {
            string[] columns = fileLine.Split(";");
            return new NutriCustomerFileDTO(
                columns[1],
                columns[2],
                columns[3],
                columns[4],
                columns[5],
                columns[6],
                columns[7],
                columns[8],
                columns[9],
                columns[10],
                columns[11],
                columns[12],
                columns[13],
                columns[14],
                columns[15]
                     );
        }
    }
}
