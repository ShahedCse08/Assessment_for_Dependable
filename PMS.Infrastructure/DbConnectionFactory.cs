using System.Data;
using System.Data.SqlClient;

namespace PMS.Infrastructure
{
    public class DbConnectionFactory
    {
        private readonly string _connectionString;
        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
