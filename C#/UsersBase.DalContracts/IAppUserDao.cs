using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersBase.Entities;

namespace UsersBase.DalContracts
{
    public interface IAppUserDao
    {
        int Create(string login, string password, string nickName);
        bool Edit(string login, string password = null, string nickName = null);
        bool SetImage(string login, byte[] image, string imageType);
        bool Delete(string login);
        AppUser Get(string login, string password);
        IEnumerable<AppUser> GetAll();
        bool RemoveImage(string login);
        byte[] GetImage(string login);
        string GetImageType(string login);
        string GetRole(string login);
        IEnumerable<string> GetAllRoles();
        bool SetRole(string login, string role);
        bool IsUserInRole(string login, string Role);
    }
}
