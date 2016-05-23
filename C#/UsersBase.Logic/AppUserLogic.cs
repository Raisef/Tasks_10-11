using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersBase.DaoManager;
using UsersBase.Entities;
using UsersBase.LogicContracts;
using UsersBase.ExceptionLogger;

namespace UsersBase.Logic
{
    public class AppUserLogic : IAppUserLogic
    {
        private readonly ManagerDao DaoManager = ManagerDao.Instance;

        public int Create(string login, string password, string nickName)
        {
            if ((string.IsNullOrWhiteSpace(login) || login.Length < 2 || login.StartsWith(" ") || login.EndsWith(" ")) ||
                (string.IsNullOrWhiteSpace(password) || password.Length < 3 || password.Length > 14 || password.StartsWith(" ") || password.EndsWith(" ")) ||
                (string.IsNullOrWhiteSpace(nickName) || nickName.Length < 2 || nickName.StartsWith(" ") || nickName.EndsWith(" ")))
            {
                return -1;
            }
            try
            {
                return DaoManager.AppUserDao().Create(login, password, nickName);
            }
            catch (Exception ex)
            {
                ExceptionLog.LogError("BLL-AppUserLogic", $"Error on creating user: {ex.Message}", DateTime.Now);
                throw;
            }

        }

        public bool Delete(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return false;
            }
            try
            {
                return DaoManager.AppUserDao().Delete(login);
            }
            catch (Exception ex)
            {
                ExceptionLog.LogError("BLL-AppUserLogic", $"Error on deleting user: {ex.Message}", DateTime.Now);
                throw;
            }
        }

        public bool Edit(string login, string password = null, string nickName = null)
        {
            if (string.IsNullOrWhiteSpace(login) ||
                (string.IsNullOrWhiteSpace(password) &&
                string.IsNullOrWhiteSpace(nickName)))
            {
                return false;
            }
            if (!string.IsNullOrWhiteSpace(password) && (!password.EndsWith(" ") && !password.StartsWith(" ")) &&
                !string.IsNullOrWhiteSpace(nickName) && (!nickName.EndsWith(" ") && !nickName.StartsWith(" ")))
            {
                return DaoManager.AppUserDao().Edit(login, password, nickName);
            }
            else if ((!string.IsNullOrWhiteSpace(password) && (!password.EndsWith(" ") && !password.StartsWith(" "))) &&
                (nickName == null))
            {
                return DaoManager.AppUserDao().Edit(login, password: password);
            }
            else if ((password == null) &&
                (!string.IsNullOrWhiteSpace(nickName) && (!nickName.EndsWith(" ") && !nickName.StartsWith(" "))))
            {
                return DaoManager.AppUserDao().Edit(login, nickName: nickName);
            }
            return false;
        }

        public AppUser Get(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return null;
            }
            return DaoManager.AppUserDao().Get(login, password);
        }

        public IEnumerable<AppUser> GetAll()
        {
            return DaoManager.AppUserDao().GetAll();
        }

        public IEnumerable<string> GetAllRoles()
        {
            return DaoManager.AppUserDao().GetAllRoles();
            
        }

        public byte[] GetImage(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return null;
            }
            return DaoManager.AppUserDao().GetImage(login);
        }

        public string GetImageType(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return null;
            }
            return DaoManager.AppUserDao().GetImageType(login);
        }

        public string GetRole(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return null;
            }
            return DaoManager.AppUserDao().GetRole(login) ?? "";
        }

        public bool IsUserInRole(string login, string role)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(role))
            {
                return false;
            }
            return DaoManager.AppUserDao().IsUserInRole(login, role);
        }

        public bool RemoveImage(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return false;
            }
            return DaoManager.AppUserDao().RemoveImage(login);
        }

        public bool SetImage(string login, byte[] image, string imageType)
        {
            if (string.IsNullOrWhiteSpace(login) || image == null || imageType == null)
            {
                return false;
            }
            return DaoManager.AppUserDao().SetImage(login, image, imageType);
        }

        public bool SetRole(string login, string role)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(role))
            {
                return false;
            }
            return DaoManager.AppUserDao().SetRole(login, role);
        }
    }
}
