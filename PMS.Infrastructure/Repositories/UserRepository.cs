using Dapper;
using PMS.Application.Interfaces;
using PMS.Domain.Entities;
using System.Data;

namespace PMS.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;
        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public User GetByUsername(string username)
        {
            return _connection.QueryFirstOrDefault<User>(
                "SELECT * FROM Users WHERE Username = @Username", new { Username = username });
        }
    }
}
