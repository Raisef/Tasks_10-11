using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UsersBase.Entities;
using UsersBase.ExceptionLogger;

using UsersBase.Logic;
using UsersBase.LogicContracts;

namespace UsersBase.ConsoleUI
{
    internal class Program
    {
        private static IUserLogic _userLogic;
        private static IAwardLogic _awardLogic;

        private static void Main(string[] args)
        {
            try
            {
                _userLogic = new UserLogic();
                _awardLogic = new AwardLogic();
            }
            catch (ConfigurationErrorsException ex)
            {
                ExceptionLog.LogError($"PL-{ex.Source}", $"Error in configurations: {ex.Message}", DateTime.Now);

                ConsoleColor originalColor = Console.ForegroundColor;
                Console.WriteLine($"Error in configuration file:");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.BareMessage);
                Console.ForegroundColor = originalColor;
                return;
            }

            while (true)
            {
                Console.WriteLine("-------------------------------");
                Console.WriteLine(@"             Menu              ");
                Console.WriteLine("-------------------------------");
                Console.WriteLine("1 - Add user");
                Console.WriteLine("2 - Delete user by id");
                Console.WriteLine("3 - Show all users");
                Console.WriteLine("4 - Show user by id");
                Console.WriteLine("5 - Create award");
                Console.WriteLine("6 - Show all awards");
                Console.WriteLine("7 - Show award by id");
                Console.WriteLine("8 - Add award to user by id");
                Console.WriteLine("9 - Remove award from user");
                Console.WriteLine("0 - Exit");
                Console.WriteLine("-------------------------------");

                string userChoise = Console.ReadLine();
                Console.WriteLine("-------------------------------");
                switch (userChoise)
                {
                    case "1":
                        try
                        {
                            AddUser();
                        }
                        catch (ArgumentException ex)
                        {
                            ExceptionLog.LogError($"PL-{ex.Source}", $"Error on adding user: {ex.Message}", DateTime.Now);

                            Console.WriteLine($"\n Incorrect input: {ex.Message}");
                        }
                        catch (InvalidOperationException ex)
                        {
                            ExceptionLog.LogError($"PL-{ex.Source}", $"Error on adding user: {ex.Message}", DateTime.Now);

                            ConsoleColor originalColor = Console.ForegroundColor;
                            Console.Write("Critical error:");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(ex.Message);
                            Console.ForegroundColor = originalColor;
                            Console.WriteLine(", program will closed");
                            return;
                        }
                        break;

                    case "2":
                        try
                        {
                            DeleteUser();
                            break;
                        }
                        catch (InvalidOperationException ex)
                        {
                            ExceptionLog.LogError($"PL-{ex.Source}", $"Error on deleting user: {ex.Message}", DateTime.Now);

                            ConsoleColor originalColor = Console.ForegroundColor;
                            Console.Write("Critical error:");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(ex.Message);
                            Console.ForegroundColor = originalColor;
                            Console.WriteLine(", program will closed");
                            return;
                        }
                        catch (IOException ex)
                        {
                            ExceptionLog.LogError($"PL-{ex.Source}", $"Error on deleting user: {ex.Message}", DateTime.Now);

                            ConsoleColor originalColor = Console.ForegroundColor;
                            Console.Write("Critical error:");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(ex.Message);
                            Console.ForegroundColor = originalColor;
                            Console.WriteLine(", program will closed");
                            return;
                        }

                    case "3":
                        try
                        {
                            ShowUsers();
                            break;
                        }
                        catch (Exception ex)
                        {
                            ExceptionLog.LogError($"PL-{ex.Source}", $"Error on showing users list: {ex.Message}", DateTime.Now);
                            ConsoleColor originalColor = Console.ForegroundColor;
                            Console.Write("Critical error:");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"Unknown error on showing users list {ex.Message}");
                            Console.ForegroundColor = originalColor;
                            Console.WriteLine(", program will closed");
                            return;
                        }

                    case "4":
                        try
                        {
                            ShowUserById();
                        }
                        catch (ArgumentException ex)
                        {
                            ExceptionLog.LogError($"PL-{ex.Source}", $"Error on showing user by id: {ex.Message}", DateTime.Now);
                        }
                        catch (IOException ex)
                        {
                            ExceptionLog.LogError($"PL-{ex.Source}", $"Error on showing user by id: {ex.Message}", DateTime.Now);
                            ConsoleColor originalColor = Console.ForegroundColor;
                            Console.Write("Critical error:");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"Unknown error on showing user by id {ex.Message}");
                            Console.ForegroundColor = originalColor;
                            Console.WriteLine(", program will closed");
                            return;
                        }
                        break;

                    case "5":
                        try
                        {
                            CreateAward();
                        }
                        catch (ArgumentException ex)
                        {
                            ExceptionLog.LogError($"PL-{ex.Source}", $"Error on creating award: {ex.Message}", DateTime.Now);

                            Console.WriteLine($"\n Incorrect input: {ex.Message}");
                        }
                        catch (IOException ex)
                        {
                            ExceptionLog.LogError($"PL-{ex.Source}", $"Error on creating award: {ex.Message}", DateTime.Now);

                            ConsoleColor originalColor = Console.ForegroundColor;
                            Console.Write("Critical error:");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(ex.Message);
                            Console.ForegroundColor = originalColor;
                            Console.WriteLine(", program will closed");
                            return;
                        }
                        break;

                    case "6":
                        try
                        {
                            ShowAwards();
                            break;
                        }
                        catch (Exception ex)
                        {
                            ExceptionLog.LogError($"PL-{ex.Source}", $"Error on showing awards list: {ex.Message}", DateTime.Now);
                            ConsoleColor originalColor = Console.ForegroundColor;
                            Console.Write("Critical error:");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"Unknown error on showing users list {ex.Message}");
                            Console.ForegroundColor = originalColor;
                            Console.WriteLine(", program will closed");
                            return;
                        }

                    case "7":
                        try
                        {
                            ShowAwardById();
                            break;
                        }
                        catch (Exception ex)
                        {
                            ExceptionLog.LogError($"PL-{ex.Source}", $"Error on showing awards list: {ex.Message}", DateTime.Now);
                            ConsoleColor originalColor = Console.ForegroundColor;
                            Console.Write("Critical error:");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"Unknown error on showing users list {ex.Message}");
                            Console.ForegroundColor = originalColor;
                            Console.WriteLine(", program will closed");
                            return;
                        }

                    case "8":
                        try
                        {
                            AddAwardToUser();
                            break;
                        }
                        catch (InvalidOperationException ex)
                        {
                            ExceptionLog.LogError($"PL-{ex.Source}", $"Error on adding award to user: {ex.Message}", DateTime.Now);

                            ConsoleColor originalColor = Console.ForegroundColor;
                            Console.Write("Critical error:");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(ex.Message);
                            Console.ForegroundColor = originalColor;
                            Console.WriteLine(", program will closed");
                            return;
                        }
                        catch (IOException ex)
                        {
                            ExceptionLog.LogError($"PL-{ex.Source}", $"Error on adding award to  user: {ex.Message}", DateTime.Now);

                            ConsoleColor originalColor = Console.ForegroundColor;
                            Console.Write("Critical error:");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(ex.Message);
                            Console.ForegroundColor = originalColor;
                            Console.WriteLine(", program will closed");
                            return;
                        }

                    case "9":
                        try
                        {
                            RemoveUserAward();
                            break;
                        }
                        catch (InvalidOperationException ex)
                        {
                            ExceptionLog.LogError($"PL-{ex.Source}", $"Error on removing award from user: {ex.Message}", DateTime.Now);

                            ConsoleColor originalColor = Console.ForegroundColor;
                            Console.Write("Critical error:");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(ex.Message);
                            Console.ForegroundColor = originalColor;
                            Console.WriteLine(", program will closed");
                            return;
                        }
                        catch (IOException ex)
                        {
                            ExceptionLog.LogError($"PL-{ex.Source}", $"Error on removing award from user: {ex.Message}", DateTime.Now);

                            ConsoleColor originalColor = Console.ForegroundColor;
                            Console.Write("Critical error:");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(ex.Message);
                            Console.ForegroundColor = originalColor;
                            Console.WriteLine(", program will closed");
                            return;
                        }

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Incorrect input");
                        break;
                }
            }
        }

        private static void ShowAwardById()
        {
            Console.WriteLine("What award do you want to assign?");
            int awardId = ChooseAward();
            if (awardId == 0) { return; }
            Award award = _awardLogic.Get(awardId);
            if (award != null)
            {
                Console.WriteLine($"{award.Id}. {award.Name}; Awarded users: {DictionaryToString(award.Owners)}");
            }
            else
            {
                Console.WriteLine($"There is no award with this id: {awardId}");
            }
        }

        private static void ShowUserById()
        {
            Console.WriteLine("Enter user id:");
            int userId = ChooseUser();
            if (userId == 0) { return; }
            User user = _userLogic.Get(userId);
            if (user != null)
            {
                var awardedUser = user as AwardedUser;
                Console.WriteLine($"{user.Id}. Name:{user.Name}; Awards: {EnumerableToString(awardedUser?.Awards)}; Age:{user.Age()}; ({user.BirthDate.ToShortDateString()})");
            }
            else
            {
                Console.WriteLine($"There is no user with this id: {userId}");
            }
        }

        private static void RemoveUserAward()
        {
            Console.WriteLine("Choose the award.");
            int awardId = ChooseAward();
            if (awardId == 0) { return; }
            Console.WriteLine("Choose user.");
            int userId = ChooseUser();
            if (userId == 0) { return; }
            if (_awardLogic.DeleteUserAward(userId, awardId))
            {
                Console.WriteLine("Award was succesfully removed");
                return;
            }
            Console.WriteLine("Something goes wrong.");
        }

        private static void AddAwardToUser()
        {
            Console.WriteLine("What award do you want to assign?");
            int awardId = ChooseAward();
            if (awardId == 0) { return; }
            Console.WriteLine("Enter user id:");
            int userId = ChooseUser();
            if (userId == 0) { return; }
            _awardLogic.RewardUser(userId, awardId);
        }

        private static KeyValuePair<int, int> ShowAwards()
        {
            IEnumerable<Award> awards = _awardLogic.GetAll();
            int maxId = 0;
            int minId = 0;
            if (awards != null && awards.Any())
            {
                Console.WriteLine("Awards:");

                foreach (var award in awards.OrderBy(award => award.Id))
                {
                    int id = award.Id;
                    Console.WriteLine($"{award.Id}. {award.Name}; Awarded users: {DictionaryToString(award.Owners)}");
                    minId = minId == 0 ? id : Math.Min(minId, id);
                    maxId = id > maxId ? id : maxId;
                }
            }
            else
            {
                Console.WriteLine("There is no awards yet.");
            }
            return new KeyValuePair<int, int>(minId, maxId);
        }

        private static void CreateAward()
        {
            Console.WriteLine("Enter award name:");
            string award = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(award) || award.EndsWith(" "))
            {
                Console.WriteLine("Award name should not be empty or ends with \"space\"");
                return;
            }
            _awardLogic.Create(award);
        }

        private static KeyValuePair<int, int> ShowUsers()
        {
            IEnumerable<User> users = _userLogic.GetAll();
            int maxId = 0;
            int minId = 0;
            if (users != null && users.Any())
            {
                Console.WriteLine("Users:");
                foreach (var user in users.OrderBy(user => user.Id))
                {
                    minId = minId == 0 ? user.Id : Math.Min(minId, user.Id);
                    maxId = user.Id > maxId ? user.Id : maxId;
                    var awardedUser = user as AwardedUser;

                    Console.WriteLine($"{user.Id}. Name:{user.Name}; Awards: {EnumerableToString(awardedUser?.Awards)}; Age:{user.Age()}; ({user.BirthDate.ToShortDateString()})");
                }
            }
            else
            {
                Console.WriteLine("There is no users yet.");
            }
            return new KeyValuePair<int, int>(minId, maxId);
        }

        

        private static void DeleteUser()
        {
            Console.WriteLine("Insert user id you want to delete:");
            int userId = ChooseUser();
            if (userId == 0) { return; }
            if (_userLogic.Delete(userId))
            {
                Console.WriteLine($"User with id: {userId} was deleted");
                return;
            }
            Console.WriteLine($"There is no user with id:{userId}");
        }

        private static void AddUser()
        {
            Console.WriteLine("Insert user name:");
            string userName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(userName) || userName.EndsWith(" "))
            {
                Console.WriteLine("Name can not be empty or ends with \"space\"");
                return;
            }
            Console.WriteLine($"Insert user birth date (ex. {DateTime.Now: dd.MM.yyyy}):");
            string userBirthDate = Console.ReadLine();
            DateTime tempBirthDate;
            if (!DateTime.TryParseExact(userBirthDate, "dd.MM.yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out tempBirthDate))
            {
                Console.WriteLine("You have input incorect date.");
                return;
            }
            int id = _userLogic.Create(userName, tempBirthDate);
            if (id != 0)
            {
                Console.WriteLine($"User {userName} with id:{id} was added.");
            }
            else
            {
                Console.WriteLine("Incorrect input");
            }
        }

        private static string DictionaryToString(Dictionary<int, string> n)
        {
            if (n == null || n.Count == 0)
            {
                return "No awarded users.";
            }
            var sb = new StringBuilder();
            foreach (var item in n)
            {
                sb.Append(item.Value).Append(",");
            }
            return sb.Remove(sb.Length - 1, 1).ToString();
        }

        private static string EnumerableToString(IEnumerable<string> n)
        {
            if (n == null || !n.Any())
            {
                return "No awards";
            }
            var sb = new StringBuilder();
            foreach (var item in n)
            {
                sb.Append(item).Append(",");
            }
            return sb.Remove(sb.Length - 1, 1).ToString();
        }

        private static int ChooseAward()
        {
            var minMaxAwardId = ShowAwards();
            if (minMaxAwardId.Key == 0) { return 0; }
            string awardIdString = Console.ReadLine();
            int awardId;
            if (!int.TryParse(awardIdString, out awardId) || awardId < minMaxAwardId.Key || awardId > minMaxAwardId.Value)
            {
                Console.WriteLine($"Id must be in range {minMaxAwardId.Key}-{minMaxAwardId.Value}!");
                return 0;
            }
            return awardId;
        }

        private static int ChooseUser()
        {
            var minMaxUserId = ShowUsers();
            if (minMaxUserId.Key == 0) { return 0; }
            string userIdString = Console.ReadLine();
            int userId;
            if (!int.TryParse(userIdString, out userId) || userId < minMaxUserId.Key || userId > minMaxUserId.Value)
            {
                Console.WriteLine($"Id must be in range {minMaxUserId.Key}-{minMaxUserId.Value}!");
                return 0;
            }
            return userId;
        }
    }
}