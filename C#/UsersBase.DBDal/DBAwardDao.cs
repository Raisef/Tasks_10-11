using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UsersBase.DalContracts;
using UsersBase.Entities;

namespace UsersBase.DBDal
{
    public class DBAwardDao : IAwardDao
    {
        private string _connectionString;

        public DBAwardDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Create(Award award)
        {
            if (award == null)
            {
                return 0;
            }
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "Award_Add";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Name", award.Name);
                command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Direction = ParameterDirection.Output });
                connection.Open();
                command.ExecuteNonQuery();
                result = (int)command.Parameters["@Id"].Value;
            }
            return result;
        }

        public bool Delete(int awardId)
        {
            if (awardId <= 0)
            {
                return false;
            }
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "Award_Delete";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", awardId);
                connection.Open();
                result = command.ExecuteNonQuery();
            }
            return result == 0 ? false : true;
        }

        public bool RemoveUserAward(int userId, int awardId)
        {
            if (userId <= 0 || awardId <= 0)
            {
                return false;
            }
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "User_RemoveAward";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@AwardId", awardId);
                connection.Open();
                result = command.ExecuteNonQuery();
            }
            return result == 0 ? false : true;
        }

        public bool Edit(int awardId, string awardName)
        {
            if (awardId <= 0 || string.IsNullOrWhiteSpace(awardName) || awardName.Length > 50)
            {
                return false;
            }
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "Award_ChangeName";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", awardId);
                command.Parameters.AddWithValue("@Name", awardName);
                connection.Open();
                result = command.ExecuteNonQuery();
            }
            return result == 0 ? false : true;
        }

        public Award Get(int awardId)
        {
            if (awardId <= 0)
            {
                return null;
            }
            Award award = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "Award_Get";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", awardId);
                connection.Open();
                string name = command.ExecuteScalar() as string;
                if (name != null) { award = new Award { Id = awardId, Name = name }; }
            }
            return award;
        }

        public IEnumerable<Award> GetAll()
        {
            var awards = new List<Award>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "Award_GetAll";
                command.CommandType = CommandType.StoredProcedure;
                               
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["Id"];
                        string name = (string)reader["Name"];
                        var owners = GetAllOwners(id);

                        awards.Add(new Award { Id = id, Name = name, Owners = owners });
                    }
                }
                
            }
            return awards;
        }

        public Dictionary<int, string> GetAllOwners(int awardId)
        {
            var ownersId = new List<int>();
            var owners = new Dictionary<int, string>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var cmdGetOwnersId = connection.CreateCommand();
                cmdGetOwnersId.CommandText = "Award_GetAllOwners";
                cmdGetOwnersId.CommandType = CommandType.StoredProcedure;
                cmdGetOwnersId.Parameters.AddWithValue("@Award_Id", awardId);
                var cmdGetOwnerName = connection.CreateCommand();
                cmdGetOwnerName.CommandText = "SELECT Name FROM dbo.Users WHERE Id = @Id";
                connection.Open();
                using (var reader = cmdGetOwnersId.ExecuteReader()){
                    while (reader.Read())
                    {
                        int ownerId = (int)reader["User_Id"];
                        
                        ownersId.Add(ownerId);
                    }
                }
                
                foreach(var owner in ownersId)
                {
                    cmdGetOwnerName.Parameters.Clear();
                    cmdGetOwnerName.Parameters.AddWithValue("@Id", owner);
                    string ownerName = (string)cmdGetOwnerName.ExecuteScalar();
                    owners.Add(owner, ownerName);
                }
            }
            return owners;
        }
            

        public bool AwardUser(KeyValuePair<int, string> user, int awardId)
        {
            if (awardId <= 0 || (user.Key <= 0 || string.IsNullOrWhiteSpace(user.Value)))
            {
                return false;
            }
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "User_Award";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", user.Key);
                command.Parameters.AddWithValue("@AwardId", awardId);
                connection.Open();
                result = command.ExecuteNonQuery();
            }
            return result == 0 ? false : true;
        }

        public bool SetImage(int awardId, byte[] image, string imageType)
        {
            if (awardId <= 0 || image == null)
            {
                return false;
            }
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "Award_SetImage";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", awardId);
                command.Parameters.AddWithValue("@Image_Type", imageType);
                command.Parameters.Add("@Image", SqlDbType.VarBinary, -1).Value = image;
                connection.Open();
                result = command.ExecuteNonQuery();
            }
            return result == 0 ? false : true;
        }

        public bool RemoveImage(int awardId)
        {
            if (awardId <= 0)
            {
                return false;
            }
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "Award_RemoveImage";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", awardId);
                connection.Open();
                result = command.ExecuteNonQuery();
            }
            return result == 0 ? false : true;
        }

        public byte[] GetImage(int awardId)
        {
            byte[] image = null;
            if (awardId <= 0)
            {
                return image;
            }
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "Award_GetImage";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", awardId);
                connection.Open();
                image = command.ExecuteScalar() as byte[];
            }
            return image;
        }

        public string GetImageType(int awardId)
        {
            if (awardId <= 0)
            {
                return null;
            }
            string imageType = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "Award_GetImageType";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", awardId);
                connection.Open();
                imageType = (string)command.ExecuteScalar();
            }
            return imageType;
        }
    }
}