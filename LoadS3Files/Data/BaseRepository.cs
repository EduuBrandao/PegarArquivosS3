using Npgsql;
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace LoadS3Files.Data
{

    public class BaseRepository
    {
        private readonly string _connectionString;

        public BaseRepository(string connectionString) : base()
        {
            _connectionString = "Data Source=DESKTOP-FECD75M\\SQLEXPRESS;Initial Catalog=Nutricionista;Trusted_Connection=True;"; ;
        }

        protected IDbConnection Connection
        {
            get { return new SqlConnection(_connectionString); }
        }

        public void Execute(string bulkScript)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Execute(bulkScript);
            }
        }
    }
}
