using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UsersBase.DalContracts;
using UsersBase.Entities;
using UsersBase.ExceptionLogger;


namespace UsersBase.FileDal
{
    public class FileUserDao : IUserDao
    {
        private readonly string _usersMaxId = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UsersId.txt");
        private readonly string _usersBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UsersBase.txt");

        public FileUserDao()
        {
            TouchFile(_usersMaxId);
            TouchFile(_usersBase);            
        }

        public bool Create(User user)
        {
            int maxId = GetMaxId();

            user.Id = ++maxId;
            File.WriteAllText(_usersMaxId, maxId.ToString(), Encoding.UTF8);
            File.AppendAllLines(_usersBase, new[] { $"{user.Id}|{user.BirthDate}|{user.Name}" }, Encoding.UTF8);
            return true;
        }

        public IEnumerable<User> GetAll()
        {
            return File.ReadAllLines(_usersBase, Encoding.UTF8)
                .Select(line =>
                {
                    var parts = line.Split(new[] { '|' }, 3, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Any())
                    {
                        return new User
                        {
                            Id = int.Parse(parts[0]),
                            BirthDate = DateTime.Parse(parts[1]),
                            Name = parts[2]
                        };
                    }
                    return null;
                }).ToList();
        }

        public bool Delete(int userId)
        {
            Dictionary<int, string> usersBase = File.ReadAllLines(_usersBase, Encoding.UTF8)
                .Select(line =>
                {
                    var parts = line.Split(new[] { '|' }, 2);
                    return new KeyValuePair<int, string>(int.Parse(parts[0]), parts[1]);
                }).ToDictionary(item => item.Key, item => item.Value);
            if (usersBase.ContainsKey(userId))
            {
                usersBase.Remove(userId);
                try
                {
                    BaseToTxt(usersBase);
                    return true;
                }
                catch (IOException ex)
                {
                    ExceptionLog.LogError($"DAL-{ex.Source}", $"Error on writing data to UsersBase file: {ex.Message}", DateTime.Now);
                    throw;
                }
            }
            return false;
        }

        private void BaseToTxt(Dictionary<int, string> usersBase)
        {
            using (StreamWriter writer = new StreamWriter(_usersBase, append: false, encoding: Encoding.UTF8))
            {
                foreach (var x in usersBase.OrderBy(line => line.Key))
                {
                    writer.WriteLine($"{x.Key}|{x.Value}");
                }
            }
        }

        private int GetMaxId()
        {
            string maxIdString = File.ReadAllText(_usersMaxId, Encoding.UTF8);
            int maxId;
            if (string.IsNullOrEmpty(maxIdString))
            {
                return maxId = 0;
            }
            else
            {
                return maxId = int.Parse(maxIdString);
            }
        }

        private static void TouchFile(string fileName)
        {
            using (var file = File.Open(fileName, FileMode.OpenOrCreate))
            { }
        }

        public User Get(int userId)
        {
            var userString = File.ReadAllLines(_usersBase, Encoding.UTF8)
                .FirstOrDefault(line => {
                    var slash = line.IndexOf('|');
                    return line.Substring(0, slash) == userId.ToString();
                });
            if(string.IsNullOrWhiteSpace(userString))
            {
                return null;
            }
            var parts = userString.Split(new[] { '|' }, 3, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Any() && int.Parse(parts[0]) == userId)
            {
                return new User
                {
                    Id = int.Parse(parts[0]),
                    BirthDate = DateTime.Parse(parts[1]),
                    Name = parts[2]
                };
            }
            return null;
        }

        public bool Edit(int userId, string userName = null, DateTime userBirthDate = new DateTime())
        {
            if (userName == null && userBirthDate == new DateTime())
            {
                return false;
            }
            Dictionary<int, string> usersBase = File.ReadAllLines(_usersBase, Encoding.UTF8)
                .Select(line =>
                {
                    var parts = line.Split(new[] { '|' }, 2);
                    return new KeyValuePair<int, string>(int.Parse(parts[0]), parts[1]);
                }).ToDictionary(item => item.Key, item => item.Value);
            if (usersBase.ContainsKey(userId))
            {

                var userParts = usersBase[userId].Split(new[] { '|' }, 2);
                try
                {
                    if ((!string.IsNullOrWhiteSpace(userName) && !userName.EndsWith(" ")) &&
                (userBirthDate != new DateTime()))
                    {
                        usersBase[userId] = $"{userBirthDate}|{userName}";
                    }
                    else if ((string.IsNullOrWhiteSpace(userName) || userName.EndsWith(" ")) &&
                        (userBirthDate != new DateTime()))
                    {
                        usersBase[userId] = $"{userBirthDate}|{userParts[1]}";
                    }
                    else if ((!string.IsNullOrWhiteSpace(userName) && !userName.EndsWith(" ")) &&
                        (userBirthDate == new DateTime()))
                    {
                        usersBase[userId] = $"{userParts[0]}|{userName}";
                    }
                    BaseToTxt(usersBase);
                    return true;
                }
                catch (IOException ex)
                {
                    ExceptionLog.LogError($"DAL-{ex.Source}", $"Error on writing data to UsersBase file: {ex.Message}", DateTime.Now);
                    throw;
                }
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