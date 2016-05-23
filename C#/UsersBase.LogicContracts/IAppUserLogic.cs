using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersBase.Entities;

namespace UsersBase.LogicContracts
{
    public interface IAppUserLogic
    {
        int Create(string login, string password, string nickName);
        bool Delete(string login);
        bool Edit(string login, string password = null, string nickName = null);
        IEnumerable<AppUser> GetAll();
        AppUser Get(string login, string password);
        bool SetImage(string login, byte[] image, string imageType);
        bool RemoveImage(string login);
        byte[] GetImage(string login);
        string GetRole(string login);
        bool SetRole(string login , string role);
        IEnumerable<string> GetAllRoles();
        bool IsUserInRole(string login, string role);
        string GetImageType(string login);
    }
}
