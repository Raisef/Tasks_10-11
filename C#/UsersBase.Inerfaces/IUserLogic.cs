using System;
using System.Collections.Generic;
using UsersBase.Entities;

namespace UsersBase.Interfaces
{
    public interface IUserLogic
    {
        int AddUser(string userName, DateTime userBirthDate);
        bool DeleteUser(int userId);        
        IEnumerable<User> GetAll();
    }
}