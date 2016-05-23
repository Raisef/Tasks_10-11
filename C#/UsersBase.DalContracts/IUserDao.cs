using System;
using System.Collections.Generic;
using UsersBase.Entities;

namespace UsersBase.DalContracts
{
    public interface IUserDao
    {
        int Create(User user);
        bool Delete(int userId);
        bool Edit(int userId, string userName = null, DateTime userBirthDate = new DateTime());
        IEnumerable<User> GetAll();
        User Get(int userId);
        bool SetImage(int userId, byte[] image, string imageType);
        bool RemoveImage(int userId);
        byte[] GetImage(int userId);
        string GetImageType(int userId);
    }
}