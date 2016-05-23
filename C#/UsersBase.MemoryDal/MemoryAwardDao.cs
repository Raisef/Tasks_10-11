using System;
using System.Collections.Generic;
using System.Linq;
using UsersBase.DalContracts;
using UsersBase.Entities;
using UsersBase.ExceptionLogger;


namespace UsersBase.MemoryDal
{
    public class MemoryAwardDao : IAwardDao
    {
        private static MemoryAwardDao _instance;
        private ICollection<Award> _awards;
        private int _lastId = 0;

        private MemoryAwardDao()
        {
            _awards = new HashSet<Award>
            {
                new Award { Id=0, Name = "Award" }
            };
        }

        public static MemoryAwardDao Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MemoryAwardDao();
                }
                return _instance;
            }
        }

        public IEnumerable<Award> GetAll()
        {
            List<Award> temp = new List<Award>();
            foreach (var award in _awards)
            {
                temp.Add(new Award { Id = award.Id, Name = award.Name, Owners = award.Owners });
            }
            return temp;
        }

        public bool Create(Award award)
        {
            if (!_awards.Any())
            {
                award.Id = 1;
                _lastId = 1;
            }
            else
            {
                award.Id = ++_lastId;
            }

            _awards.Add(award);
            return true;
        }

        public bool RemoveUserAward(int userId, int awardId)
        {
            if (_awards.Any(n => n.Id == awardId))
            {
                try
                {
                    Award targetAward = _awards.Single(award => award.Id == awardId);
                    if (targetAward.Owners != null && targetAward.Owners.Keys.Contains(userId))
                    {
                        targetAward.Owners.Remove(userId);
                        return true;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    ExceptionLog.LogError("DAL", $"Error on deleting user: there is more than 1 users with same id. {ex.Message}", DateTime.Now);
                    throw new InvalidOperationException("Data error: there is more than 1 users with same id.", ex);
                }
            }
            return false;
        }

        public bool AwardUser(KeyValuePair<int, string> user, int awardId)
        {
            if (_awards.Any(n => n.Id == awardId))
            {
                try
                {
                    Award targetAward = _awards.Single(award => award.Id == awardId);
                    if (targetAward.Owners == null || targetAward.Owners.Count == 0)
                    {
                        targetAward.Owners = new Dictionary<int, string> { [user.Key] = user.Value };
                    }
                    else if (!targetAward.Owners.Contains(user))
                    {
                        targetAward.Owners.Add(user.Key, user.Value);
                    }
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

        public Award Get(int awardId)
        {
            foreach (var award in _awards)
            {
                if (award.Id == awardId)
                {
                    return award;
                }
            }
            return null;
        }

        public bool Edit(int awardId, string awardName)
        {
            if (_awards.Any(n => n.Id == awardId))
            {
                try
                {
                    Award targetAward = _awards.Single(award => award.Id == awardId);
                    targetAward.Name = awardName;
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

        public bool Delete(int awardId)
        {
            throw new NotImplementedException();
        }

        public bool SetImage(int awardId, byte[] image)
        {
            throw new NotImplementedException();
        }

        public bool RemoveImage(int awardId)
        {
            throw new NotImplementedException();
        }

        public byte[] GetImage(int awardId)
        {
            throw new NotImplementedException();
        }

        int IAwardDao.Create(Award award)
        {
            throw new NotImplementedException();
        }

        public bool SetImage(int awardId, byte[] image, string imageType)
        {
            throw new NotImplementedException();
        }

        public string GetImageType(int awardId)
        {
            throw new NotImplementedException();
        }
    }
}