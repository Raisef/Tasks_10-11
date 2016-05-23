using System;
using System.Collections.Generic;
using UsersBase.Entities;

namespace UsersBase.LogicContracts
{
    public interface IUserLogic
    {
        int Create(string userName, DateTime userBirthDate);
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