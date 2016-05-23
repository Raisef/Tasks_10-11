using System;
using System.Collections.Generic;
using System.Linq;
using UsersBase.DalContracts;
using UsersBase.Entities;
using UsersBase.ExceptionLogger;

namespace UsersBase.MemoryDal
{
    public class MemoryUserDao : IUserDao
    {
        private static MemoryUserDao _instance;
        private ICollection<User> _users;
        private int _lastId = 0;

        private MemoryUserDao()
        {
            _users = new HashSet<User>
            {
                new User { Id=0, Name = "User", BirthDate = new DateTime(1900, 01, 01) }
            };
        }

        public static MemoryUserDao Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MemoryUserDao();
                }
                return _instance;
            }
        }

        public IEnumerable<User> GetAll()
        {
            List<User> temp = new List<User>();
            foreach (var user in _users)
            {
                temp.Add(new User { Id = user.Id, Name = user.Name, BirthDate = user.BirthDate });
            }
            return temp;
        }

        public bool Create(User user)
        {
            if (!_users.Any())
            {
                user.Id = 1;
                _lastId = 1;
            }
            else
            {
                user.Id = ++_lastId;
            }

            _users.Add(user);
            return true;
        }

        public bool Delete(int userId)
        {
            if (_users.Any(n => n.Id == userId))
            {
                try
                {
                    User targetUser = _users.Single(user => user.Id == userId);
                    _users.Remove(targetUser);
                    return true;
                }
                catch (InvalidOperationException ex)
                {
                    ExceptionLog.LogError("DAL", $"Error on deleting user: there is more than 1 users with same id. {ex.Message}", DateTime.Now);
                    throw new InvalidOperationException("Data error: there is more than 1 users with same id.", ex);
                }
            }
            return false;
        }

        public User Get(int userId)
        {
            foreach (var user in _instance._users)
            {
                if (user.Id == userId)
                {
                    return user;
                }
            }
            return null;
        }

        public bool Edit(int userId, string userName = null, DateTime userBirthDate = default(DateTime))
        {
            if (userName == null && userBirthDate == new DateTime())
            {
                return false;
            }
            if (_users.Any(user => user.Id == userId))
            {
                User targetUser;
                try
                {
                    targetUser = _users.Single(user => user.Id == userId);
                }
                catch (InvalidOperationException ex)
                {
                    ExceptionLog.LogError("DAL", $"Error on deleting user: there is more than 1 users with same id. {ex.Message}", DateTime.Now);
                    throw new InvalidOperationException("Data error: there is more than 1 users with same id.", ex);
                }
                if ((!string.IsNullOrWhiteSpace(userName) && !userName.EndsWith(" ")) &&
                (userBirthDate != new DateTime()))
                {
                    targetUser.Name = userName;
                    targetUser.BirthDate = userBirthDate;
                }
                else if ((string.IsNullOrWhiteSpace(userName) || userName.EndsWith(" ")) &&
                    (userBirthDate != new DateTime()))
                {
                    targetUser.BirthDate = userBirthDate;
                }
                else if ((!string.IsNullOrWhiteSpace(userName) && !userName.EndsWith(" ")) &&
                    (userBirthDate == new DateTime()))
                {
                    targetUser.Name = userName;
                }
                return true;
            }
            return false;
        }

        public bool SetImage(int userId, byte[] image)
        {
            throw new NotImplementedException();
        }

        public bool RemoveImage(int userId)
        {
            throw new NotImplementedException();
        }

        public byte[] GetImage(int userId)
        {
            throw new NotImplementedException();
        }

        int IUserDao.Create(User user)
        {
            throw new NotImplementedException();
        }

        public bool SetImage(int userId, byte[] image, string imageType)
        {
            throw new NotImplementedException();
        }

        public string GetImageType(int userId)
        {
            throw new NotImplementedException();
        }
    }
}