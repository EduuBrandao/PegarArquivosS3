using LoadS3Files.Data.Scripts;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadS3Files.Data
{
    public class NutriRepository : BaseRepository, INutriRepository
    {

        public NutriRepository(string connectionString) : base(connectionString)
        {

        }

        public void CreateCustomer(string nome)
        {
            var bulkScript = new StringBuilder();

            bulkScript.Append(NutriScripts.CreateUser(nome));

            if (!string.IsNullOrEmpty(bulkScript.ToString()))
            {
                Execute(bulkScript.ToString());
                bulkScript.Clear();
            }
        }
    }
}
