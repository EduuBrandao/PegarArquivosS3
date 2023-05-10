using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadS3Files.DTO
{
    public class NutriCustomerFileDTO
    {
        public NutriCustomerFileDTO(string name, string peso, string altura,
            string document, string address, string numberAddress, string district, string city, string state, string cep,
            string phone1, string phone2, string email)
        {
            Name = name;
            Document = document;
            Address = address;
            NumberAddress = numberAddress;
            District = district;
            City = city;
            State = state;
            Cep = cep;
            Phone1 = phone1;
            Phone2 = phone2;
            Peso = peso;
            Altura = altura;
            Email = email;
        }

        public string Peso { get; set; }
        public string Altura { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string NumberAddress { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Cep { get; private set; }
        public string Document { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Email { get; set; }
    }
}
