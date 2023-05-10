using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadS3Files.Data.Scripts
{
    public class NutriScripts
    {

        public static string CreateUser(string nome)
        {
            var query = @"INSERT INTO Nutricionista.dbo.teste
                  (nome)
                   VALUES('{0}')";

            return string.Format(query, nome);
        }
    }
}
