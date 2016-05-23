using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UsersBase.DaoManager;
using UsersBase.Entities;
using UsersBase.ExceptionLogger;
using UsersBase.LogicContracts;

namespace UsersBase.Logic
{
    public class AwardLogic : IAwardLogic
    {
        private readonly ManagerDao DaoManager = ManagerDao.Instance;

        public int Create(string awardName)
        {
            Award award = new Award
            {
                Name = awardName,
                Owners = new Dictionary<int, string>(),
            };
            try
            {
                int awardId = DaoManager.AwardDao().Create(award);
                if (awardId != 0)
                {
                    return award.Id;
                }
            }
            catch (IOException ex)
            {
                ExceptionLog.LogError("BLL-AwardLogic", $"Error on creating award: {ex.Message}", DateTime.Now);
                throw;
            }
            throw new InvalidOperationException("Unknown error on award adding");
        }

        public Dictionary<int, string> GetUsersNames()
        {
            IEnumerable<User> users;
            try
            {
                users = DaoManager.UserDao().GetAll();
            }
            catch (IOException ex)
            {
                ExceptionLog.LogError("BLL-AwardLogic", $"Error on getting all users: {ex.Message}", DateTime.Now);
                throw;
            }
            Dictionary<int, string> usersNames = null;
            if (users != null && users.Any())
            {
                usersNames = new Dictionary<int, string>();
                foreach (var user in users)
                {
                    usersNames.Add(user.Id, user.Name);
                }
            }
            return usersNames;
        }

        public bool DeleteUserAward(int userId, int awardId)
        {
            try
            {
                return DaoManager.AwardDao().RemoveUserAward(userId, awardId);
            }
            catch (InvalidOperationException ex)
            {
                ExceptionLog.LogError("BLL-AwardLogic", $"Error on deleting user award: {ex.Message}", DateTime.Now);

                throw;
            }
            catch (IOException ex)
            {
                ExceptionLog.LogError("BLL-AwardLogic", $"Error on deleting user award: {ex.Message}", DateTime.Now);

                throw;
            }
        }

        public IEnumerable<Award> GetAll()
        {
            try
            {
                return DaoManager.AwardDao().GetAll();
            }
            catch (IOException ex)
            {
                ExceptionLog.LogError("BLL-AwardLogic", $"Error on getting all awards: {ex.Message}", DateTime.Now);
                throw;
            }
        }

        public bool RewardUser(int userId, int awardId)
        {
            if(userId <= 0 || awardId <= 0)
            {
                return false;
            }
            User user = DaoManager.UserDao().Get(userId);
            try
            {
                return DaoManager.AwardDao().AwardUser(new KeyValuePair<int, string> (user.Id, user.Name), awardId);
            }
            catch (InvalidOperationException ex)
            {
                ExceptionLog.LogError("BLL-AwardLogic", $"Error on rewarding user: {ex.Message}", DateTime.Now);
                throw;
            }
            catch (IOException ex)
            {
                ExceptionLog.LogError("BLL-AwardLogic", $"Error on rewarding user: {ex.Message}", DateTime.Now);
                throw;
            }
        }

        public Award Get(int awardId)
        {
            try
            {
                return DaoManager.AwardDao().Get(awardId);
            }
            catch (IOException ex)
            {
                ExceptionLog.LogError("BLL-AwardLogic", $"Error on rewarding user: {ex.Message}", DateTime.Now);
                throw;
            }
        }

        public bool Edit (int awardId, string awardName)
        {
            if (string.IsNullOrWhiteSpace(awardName) || awardId <= 0)
            {
                return false;
            }
            return DaoManager.AwardDao().Edit(awardId, awardName);
        }

        public bool Delete(int awardId)
        {
            if(awardId <= 0)
            {
                return false;
            }
            return DaoManager.AwardDao().Delete(awardId);
        }

        public bool SetImage(int awardId, byte[] image, string imageType)
        {
            if (awardId <= 0 || image == null)
            {
                return false;
            }
            return DaoManager.AwardDao().SetImage(awardId, image, imageType);
        }

        public byte[] GetImage(int awardId)
        {
            if (awardId <= 0)
            {
                return null;
            }
            return DaoManager.AwardDao().GetImage(awardId);
        }

        public bool RemoveImage(int awardId)
        {
            if (awardId <= 0)
            {
                return false;
            }
            return DaoManager.AwardDao().RemoveImage(awardId);
        }

        public string GetImageType(int awardId)
        {
            if (awardId <= 0)
            {
                return null;
            }
            return DaoManager.AwardDao().GetImageType(awardId);
        }
    }
}