using System.Collections.Generic;
using UsersBase.Entities;

namespace UsersBase.Interfaces
{
    public interface IUserDao
    {
        bool CreateUser(User user);
        bool DeleteUser(int userId);
        IEnumerable<User> GetAllUsers();
    }
}