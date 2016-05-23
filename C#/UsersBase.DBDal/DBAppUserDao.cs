using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UsersBase.DalContracts;
using UsersBase.Entities;

namespace UsersBase.DBDal
{
    public class DBAppUserDao : IAppUserDao
    {
        private string _connectionString;

        public DBAppUserDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Create(string login, string password, string nickName)
        {
            if(string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(nickName))
            {
                return -1;
            }
            int result = 0;
            var hashPass = GetPassHash(password, login);
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "AppUser_Add";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", hashPass);
                command.Parameters.AddWithValue("@NickName", nickName);
                connection.Open();
                
                result = command.ExecuteNonQuery();
            }
            return result;
        }

        public bool Delete(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return false;
            }
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "AppUser_Delete";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Login", login);
                connection.Open();
                result = command.ExecuteNonQuery();
            }
            return result == 0 ? false : true;
        }

        public bool Edit(string login, string password = null, string nickName = null)
        {
            if (string.IsNullOrWhiteSpace(login) || (string.IsNullOrWhiteSpace(password) && string.IsNullOrWhiteSpace(nickName)))
            {
                return false;
            }
            int result = 0;
            if (nickName != null && nickName.Length <= 50)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "AppUser_ChangeNickName";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@NickName", nickName);
                    connection.Open();
                    result = command.ExecuteNonQuery();
                    if (result == 0)
                    {
                        return false;
                    }
                }
            }
            if (password != null && password.Length <= 14)
            {
                string passHash = GetPassHash(password, login, "Bliss");
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "User_ChangePassword";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", passHash);
                    connection.Open();
                    result = command.ExecuteNonQuery();
                }
            }
            return result == 0 ? false : true;
        }

        public AppUser Get(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }
            string passHash = GetPassHash(password, login);
            AppUser appUser = null;
            
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "AppUser_Get";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", passHash);
                connection.Open();
                var reader = command.ExecuteReader();
                reader.Read();
                if (reader.HasRows)
                {
                    var nickName = reader["NickName"] as string;
                    var imageType = reader["Image_Type"] as string;
                    if (!string.IsNullOrWhiteSpace(nickName))
                    {
                        appUser = new AppUser { Login = login, NickName = nickName, ImageType = imageType ?? null };
                    }
                }
                
            }
            if(appUser != null)
            {
                var role = GetRole(login);
                appUser.Role = role;
            }
            
            return appUser;
        }

        public IEnumerable<AppUser> GetAll()
        {
            var appUsers = new List<AppUser>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "AppUser_GetAll";
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string login = (string)reader["Login"];
                    string nickName = (string)reader["NickName"];
                    var role = GetRole(login);
                    appUsers.Add(new AppUser { Login = login, NickName = nickName, Role = role });
                }
            }
            return appUsers;
        }

        public byte[] GetImage(string login)
        {
            byte[] image = null;
            if (string.IsNullOrWhiteSpace(login))
            {
                return image;
            }
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "AppUser_GetImage";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Login", login);
                connection.Open();
                image = command.ExecuteScalar() as byte[];
            }
            return image;
        }

        private string GetPassHash(string userPass, string key, string salt = "Bliss", string hashAlgorithm = "SHA1", int passwordIterations = 2, string initialVector = "OFRna73m*aze01xY", int keySize = 256)
        {
            if (string.IsNullOrEmpty(userPass))
                return "";

            byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(userPass);

            PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(key, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;

            byte[] cipherTextBytes = null;

            using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initialVectorBytes))
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        cipherTextBytes = memStream.ToArray();
                        memStream.Close();
                        cryptoStream.Close();
                    }
                }
            }

            symmetricKey.Clear();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public string GetRole(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return null;
            }
            string role = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "AppUser_GetRole";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Login", login);
                connection.Open();
                role = command.ExecuteScalar() as string;
            }
            return role;
        }

        public bool RemoveImage(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return false;
            }
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "AppUser_RemoveImage";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Login", login);
                connection.Open();
                result = command.ExecuteNonQuery();
            }
            return result == 0 ? false : true;
        }

        public bool SetImage(string login, byte[] image, string imageType)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(imageType) || image == null)
            {
                return false;
            }
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "AppUser_SetImage";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Image_Type", imageType);
                command.Parameters.Add("@Image", SqlDbType.VarBinary, -1).Value = image;
                connection.Open();
                result = command.ExecuteNonQuery();
            }
            return result == 0 ? false : true;
        }

        public bool SetRole(string login, string role)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(role))
            {
                return false;
            }
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "AppUser_SetRole";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Role", role);
                connection.Open();
                result = command.ExecuteNonQuery();
            }
            return result == 0 ? false : true;
        }
        public bool IsUserInRole(string login, string role)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(role))
            {
                return false;
            }
            var userRole = GetRole(login);
            return role == userRole;
        }

        public string GetImageType(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return null;
            }
            string imageType = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "AppUser_GetImageType";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Login", login);
                connection.Open();
                imageType = command.ExecuteScalar() as string;
            }
            return imageType;
        }
        public IEnumerable<string> GetAllRoles()
        {
            var roles = new List<string>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "AppUser_GetAllRoles";
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string role = (string)reader["UserRole"];
                    roles.Add(role);
                }
            }
            return roles;
        }
    }
    


}
