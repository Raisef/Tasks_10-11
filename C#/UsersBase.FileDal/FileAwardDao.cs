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
    public class FileAwardDao : IAwardDao
    {
        private readonly string _awardsMaxId = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AwardsId.txt");
        private readonly string _awards = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Awards.txt");

        public FileAwardDao()
        {
            TouchFile(_awards);
            TouchFile(_awardsMaxId);
        }

        public bool Create(Award award)
        {
            int maxId = GetMaxId();

            award.Id = ++maxId;
            File.WriteAllText(_awardsMaxId, maxId.ToString(), Encoding.UTF8);
            File.AppendAllLines(_awards, new[] { $"{award.Id}|{award.Name}|{DictionaryToString(award.Owners)}" }, Encoding.UTF8);
            return true;
        }

        public bool RemoveUserAward(int userId, int awardId)
        {
            var awards = GetAll()?.ToList();
            if (awards == null || !awards.Any()) { return false; }
            bool success = false;
            for (var i = 0; i < awards.Count; i++)
            {
                if (awards[i].Id == awardId)
                {
                    if (!awards[i].Owners.ContainsKey(userId))
                    {
                        return false;
                    }
                    success = awards[i].Owners.Remove(userId);
                    break;
                }
            }
            return success ? EnumerableToTxt(awards) : false;
        }

        public IEnumerable<Award> GetAll()
        {
            return File.ReadAllLines(_awards, Encoding.UTF8)
                .Select(line =>
                {
                    var parts = line.Split(new[] { '|' }, 3, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Any() && parts.Length == 3)
                    {
                        return new Award
                        {
                            Id = int.Parse(parts[0]),
                            Name = parts[1],
                            Owners = StringOwnersToDictionary(parts[2]),
                        };
                    }
                    else if (parts.Any() && parts.Length == 2)
                    {
                        return new Award
                        {
                            Id = int.Parse(parts[0]),
                            Name = parts[1],
                            Owners = new Dictionary<int, string>(),
                        };
                    }
                    return null;
                }).ToList();
        }

        public bool AwardUser(KeyValuePair<int,string> user, int awardId)
        {
            var awards = GetAll()?.ToList();
            if (awards == null || !awards.Any()) { return false; }
            bool success = false;
            for (var i = 0; i < awards.Count; i++)
            {
                if (awards[i].Id == awardId)
                {
                    if (awards[i].Owners.ContainsKey(user.Key))
                    {
                        return false;
                    }
                    awards[i].Owners.Add(user.Key, user.Value);
                    success = true;
                    break;
                }
            }
            return success ? EnumerableToTxt(awards) : false;
        }

        private static void TouchFile(string fileName)
        {
            using (var file = File.Open(fileName, FileMode.OpenOrCreate))
            { }
        }

        private int GetMaxId()
        {
            string maxIdString = File.ReadAllText(_awardsMaxId, Encoding.UTF8);
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

        private static string DictionaryToString(Dictionary<int, string> n)
        {
            
            if (n != null)
            {
                var sb = new StringBuilder();
                foreach (var item in n)
                {
                    sb.Append($"{item.Key.ToString()}|{item.Value}").Append(",!,");
                }
                
                return sb.Length==0?sb.ToString():sb.Remove(sb.Length - 3, 3).ToString();
            }
            return "";
        }

        private static Dictionary<int, string> StringOwnersToDictionary(string users)
        {
            Dictionary<int, string> temp = new Dictionary<int, string>();
            if (string.IsNullOrWhiteSpace(users))
            {
                return temp;
            }
            foreach (var item in users.Split(new[] { ",!," }, StringSplitOptions.RemoveEmptyEntries))
            {
                var user = item.Split(new[] { '|' }, 2, StringSplitOptions.RemoveEmptyEntries);
                temp.Add(int.Parse(user[0]),user[1]);
            }
            return temp;
        }

        public Award Get(int awardId)
        {
            string awardString = File.ReadAllLines(_awards, Encoding.UTF8)
                .FirstOrDefault(line => {
                    var slash = line.IndexOf('|');
                    return line.Substring(0, slash) == awardId.ToString();
                });
            if (string.IsNullOrWhiteSpace(awardString))
            {
                return null;
            }
            var parts = awardString.Split(new[] { '|' }, 3, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Any() && int.Parse(parts[0]) == awardId)
            {
                if (parts.Length == 3)
                {
                    return new Award
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        Owners = StringOwnersToDictionary(parts[2]),
                    };
                }
                else if (parts.Length == 2)
                {
                    return new Award
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        Owners = new Dictionary<int, string>(),
                    };
                }
            }
            return null;
        }

        public bool Edit(int awardId, string awardName)
        {
            var awards = GetAll()?.ToList();
            if (awards == null || !awards.Any()) { return false; }
            var success = false;
            for (var i = 0; i <= awards.Count; i++)
            {
                if (awards[i].Id == awardId)
                {
                    awards[i].Name = awardName;
                    success = true;
                    break;
                }
            }
            return success ? EnumerableToTxt(awards) : false;
        }

        private bool EnumerableToTxt (IEnumerable<Award> awards)
        {
            using (StreamWriter writer = new StreamWriter(_awards, append: false, encoding: Encoding.UTF8))
            {
                foreach (var x in awards)
                {
                    writer.WriteLine($"{x.Id}|{x.Name}|{DictionaryToString(x.Owners)}");
                }
            }
            return true;
        }

        public bool Delete(int awardId)
        {
            var awards = GetAll()?.ToList();
            if (awards == null || !awards.Any()) { return false; }
            var success = false;
            for (var i = 0; i <= awards.Count; i++)
            {
                if (awards[i].Id == awardId)
                {
                    success = awards.Remove(awards[i]);
                    break;
                }
            }
            return success ? EnumerableToTxt(awards) : false;
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