using PMS.Domain.Entities;

namespace PMS.Application.Interfaces
{
    public interface IUserRepository
    {
        User GetByUsername(string username);
    }
}
