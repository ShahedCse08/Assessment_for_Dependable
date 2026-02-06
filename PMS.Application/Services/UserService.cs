using PMS.Application.Interfaces;
using PMS.Domain.Entities;

namespace PMS.Application.Services
{
    public interface IUserService
    {
        User GetByUsername(string username);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public User GetByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;
            return _userRepo.GetByUsername(username);
        }
    }
}
