using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UsersBase.DalContracts;
using UsersBase.Entities;

namespace UsersBase.DBDal
{
    public class DBUserDao : IUserDao
    {
        private string _connectionString;

        public DBUserDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Create(User user)
        {
            if (user == null)
            {
                return 0;
            }
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "User_Add";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@Birth_Date", SqlDbType.Date).Value = user.BirthDate;
                command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Direction = ParameterDirection.Output });
                connection.Open();
                command.ExecuteNonQuery();
                result = (int)command.Parameters["@Id"].Value;
            }
            return result;
        }

        public bool Delete(int userId)
        {
            if (userId <= 0)
            {
                return false;
            }
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "User_Delete";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", userId);
                connection.Open();
                result = command.ExecuteNonQuery();
            }
            return result == 0 ? false : true;
        }

        public bool Edit(int userId, string userName = null, DateTime userBirthDate = default(DateTime))
        {
            if (userId <= 0 || (userName == null && userBirthDate == default(DateTime)))
            {
                return false;
            }
            int result = 0;
            if (userName != null && userName.Length <= 50)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "User_ChangeName";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", userId);
                    command.Parameters.AddWithValue("@Name", userName);
                    connection.Open();
                    result = command.ExecuteNonQuery();
                    if (result == 0)
                    {
                        return false;
                    }
                }
            }
            if (userBirthDate != default(DateTime))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "User_ChangeBirthDate";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", userId);
                    command.Parameters.AddWithValue("@Birth_Date", SqlDbType.Date).Value = userBirthDate;
                    connection.Open();
                    result = command.ExecuteNonQuery();
                }
            }
            return result == 0 ? false : true;
        }

        public User Get(int userId)
        {
            if (userId <= 0)
            {
                return null;
            }
            User user = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "User_Get";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", userId);
                connection.Open();
                var reader = command.ExecuteReader();
                reader.Read();
                string name = (string)reader["Name"];
                DateTime birthDate = (DateTime)reader["Birth_Date"];
                user = new User { Id = userId, Name = name, BirthDate = birthDate };
            }
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            var users = new List<User>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "User_GetAll";
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = (int)reader["Id"];
                    string name = (string)reader["Name"];
                    DateTime birthDate = (DateTime)reader["Birth_Date"];
                    users.Add(new User { Id = id, Name = name, BirthDate = birthDate });
                }
            }
            return users;
        }

        public byte[] GetImage(int userId)
        {
            byte[] image = null;
            if (userId <= 0)
            {
                return image;
            }
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "User_GetImage";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", userId);
                connection.Open();
                image = command.ExecuteScalar() as byte[];
            }
            return image;
        }

        public bool RemoveImage(int userId)
        {
            if (userId <= 0)
            {
                return false;
            }
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "User_RemoveImage";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", userId);
                connection.Open();
                result = command.ExecuteNonQuery();
            }
            return result == 0 ? false : true;
        }

        public bool SetImage(int userId, byte[] image, string imageType)
        {
            if (userId <= 0 || image == null || imageType == null)
            {
                return false;
            }
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "User_SetImage";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", userId);
                command.Parameters.AddWithValue("@Image_Type", imageType);
                command.Parameters.Add("@Image", SqlDbType.VarBinary, -1).Value = image;
                connection.Open();
                result = command.ExecuteNonQuery();
            }
            return result == 0 ? false : true;
        }

        public string GetImageType(int userId)
        {
            if (userId <= 0)
            {
                return null;
            }
            string imageType = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "User_GetImageType";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", userId);
                connection.Open();
                imageType = (string)command.ExecuteScalar();
            }
            return imageType;
        }
    }
}