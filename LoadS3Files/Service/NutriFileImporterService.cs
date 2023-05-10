using Amazon.S3;
using LoadS3Files.DTO;
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

        public NutriFileImporterService()
        {

        }


        public bool ProcessNutriFile(Stream stream, S3Entity s3Event)
        {
            _reader = new StreamReader(stream, Encoding.GetEncoding("iso-8859-1"));

            BuildFile();
            return true;
        }

        private void BuildFile()
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

                var teste = nutriDTO;

                _reader.Close();
                _reader.Dispose();
            }
            catch (Exception)
            {

                throw;
            }
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
                columns[13]
                     );
        }
    }
}
