using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class UserManager
    {
        private string _connectionString;

        public UserManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InsertUser(User user)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Users VALUES (@name, @email, @password) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@password", user.HashPassword);
            connection.Open();
            user.Id = (int)(decimal)cmd.ExecuteScalar();
            connection.Close();
            connection.Dispose();
        }

        public User LogIn(string email, string password)
        {
            User user = GetUserByEmail(email);
            if (user == null)
            {
                return null;
            }
            bool Valid = BCrypt.Net.BCrypt.Verify(password, user.HashPassword);
            if (!Valid)
            {
                return null;
            }
            else
            {
                return user;
            }
        }

        public User GetUserByEmail(string email)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT TOP 1 * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            User user = new User
            {
                Id = (int)reader["id"],
                Name = (string)reader["Name"],
                Email = (string)reader["Email"],
                HashPassword = (string)reader["HashPassword"]
            };
            connection.Close();
            connection.Dispose();
            return user;

        }

    }
}
