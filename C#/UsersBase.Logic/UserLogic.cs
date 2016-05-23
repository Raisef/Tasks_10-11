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
    public class UserLogic : IUserLogic
    {
        private readonly DateTime _minBirthDate = new DateTime(1900, 01, 01);
        private readonly DateTime _maxBirthDate = DateTime.Now.AddYears(-3);

        private readonly ManagerDao DaoManager = ManagerDao.Instance;

        public int Create(string userName, DateTime userBirthDate)
        {
            if ((!string.IsNullOrWhiteSpace(userName) && !userName.EndsWith(" ")) && 
                (userBirthDate >= _minBirthDate && userBirthDate <= _maxBirthDate))
            {
                User user = new User
                {
                    Name = userName,
                    BirthDate = userBirthDate
                };
                var userId = DaoManager.UserDao().Create(user);
                if (userId != 0)
                {
                    return userId;
                }
                throw new InvalidOperationException("Unknown error on user adding");
            }
            return 0;
        }

        public IEnumerable<User> GetAll()
        {
            var users = DaoManager.UserDao().GetAll();
            var userAwards = GetUsersAwards();

            if (users != null && userAwards != null)
            {
                List<User> usersList = users.ToList();
                for (int i = 0; i < usersList.Count; i++)
                {
                    if (userAwards != null && userAwards.ContainsKey(usersList[i].Id))
                    {
                        usersList[i] = new AwardedUser(usersList[i], userAwards[usersList[i].Id]);
                    }
                }
                users = usersList;
            }
            return users.ToList();
        }

        public bool Delete(int userId)
        {
            try
            {
                var userAwards = GetUserAwards(userId)?.ToList();
                if (userAwards != null)
                {
                    foreach (var award in userAwards)
                    {
                        DaoManager.AwardDao().RemoveUserAward(userId, award.Id);
                    }
                }
                return DaoManager.UserDao().Delete(userId);
            }
            catch (InvalidOperationException ex)
            {
                ExceptionLog.LogError("BLL-UserLogic", $"Error on deleting user: {ex.Message}", DateTime.Now);
                throw;
            }
            catch (IOException ex)
            {
                ExceptionLog.LogError("BLL-UserLogic", $"Error on deleting user: {ex.Message}", DateTime.Now);
                throw;
            }
        }

        public Dictionary<int, HashSet<string>> GetUsersAwards()
        {
            IEnumerable<Award> awards;
            try
            {
                awards = DaoManager.AwardDao().GetAll();
            }
            catch (IOException ex)
            {
                ExceptionLog.LogError("BLL-UserLogic", $"Error on getting users awards: {ex.Message}", DateTime.Now);
                throw;
            }
            Dictionary<int, HashSet<string>> userAwards = null;
            if (awards == null) { return userAwards; }
            userAwards = new Dictionary<int, HashSet<string>>();
            foreach (var award in awards)
            {
                if (award.Owners != null && award.Owners.Any())
                {
                    foreach (var user in award.Owners)
                    {
                        if (userAwards.ContainsKey(user.Key))
                        {
                            userAwards[user.Key].Add(award.Name);
                        }
                        else
                        {
                            userAwards[user.Key] = new HashSet<string> { award.Name };
                        }
                    }
                }
            }
            return userAwards;
        }

        private IEnumerable<Award> GetUserAwards(int userId)
        {
            IEnumerable<Award> awards = DaoManager.AwardDao().GetAll();
            List<Award> userAwards = null;
            if (awards == null || !awards.Any()) { return userAwards; }
            userAwards = new List<Award>();
            foreach (var award in awards)
            {
                if (award.Owners != null && award.Owners.Any())
                {
                    foreach (var user in award.Owners)
                    {
                        if (user.Key == userId)
                        {
                            userAwards.Add(award);
                        }
                    }
                }
            }
            return userAwards;
        }

        public User Get(int userId)
        {
            User user = null;
            try
            {
                user = DaoManager.UserDao().Get(userId);
            }
            catch (IOException ex)
            {
                ExceptionLog.LogError("BLL-UserLogic", $"Error on deleting user: {ex.Message}", DateTime.Now);
                throw;
            }
            var userAwards = GetUserAwards(userId);
            if (user != null && userAwards != null)
            {
                HashSet<string> awards = new HashSet<string>();
                foreach (var award in userAwards)
                {
                    awards.Add(award.Name);
                }
                AwardedUser awardedUser = new AwardedUser(user, awards);
                return awardedUser;
            }
            return user;
        }

        public bool Edit(int userId, string userName = null, DateTime userBirthDate = new DateTime())
        {
            if ((!string.IsNullOrWhiteSpace(userName) && !userName.EndsWith(" ")) && 
                (userBirthDate >= _minBirthDate && userBirthDate <= _maxBirthDate))
            {
                return DaoManager.UserDao().Edit(userId, userName, userBirthDate);
            }
            else if ((string.IsNullOrWhiteSpace(userName) || userName.EndsWith(" ")) && 
                (userBirthDate >= _minBirthDate && userBirthDate <= _maxBirthDate))
            {
                return DaoManager.UserDao().Edit(userId, userBirthDate: userBirthDate);
            }
            else if ((!string.IsNullOrWhiteSpace(userName) && !userName.EndsWith(" ")) &&
                (userBirthDate < _minBirthDate || userBirthDate > _maxBirthDate))
            {
                return DaoManager.UserDao().Edit(userId, userName: userName);
            }
            return false;
        }

        public bool SetImage(int userId, byte[] image, string imageType)
        {
            if(userId <= 0 || image == null || imageType == null)
            {
                return false;
            }
            return DaoManager.UserDao().SetImage(userId, image, imageType);
        }

        public byte[] GetImage(int userId)
        {
            if(userId <= 0)
            {
                return null;
            }
            return DaoManager.UserDao().GetImage(userId);
        }

        public bool RemoveImage(int userId)
        {
            if (userId <= 0)
            {
                return false;
            }
            return DaoManager.UserDao().RemoveImage(userId);
        }

        public string GetImageType(int userId)
        {
            if (userId <= 0)
            {
                return null;
            }
            return DaoManager.UserDao().GetImageType(userId);
        }
    }
}