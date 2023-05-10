using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadS3Files.DTO
{
    public class NutriFileDTO
    {

        public NutriFileDTO()
        {
            Customers = new List<NutriCustomerFileDTO>();
        }

        public List<NutriCustomerFileDTO> Customers { get; set; }
    }
}
