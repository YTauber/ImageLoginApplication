using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class ImageManager
    {
        private string _connectionString;

        public ImageManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Image> GetImagesByUserId(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Images WHERE UserId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Image> Images = new List<Image>();
            while (reader.Read())
            {
                Images.Add(new Image
                {
                    Id = (int)reader["id"],
                    FileName = (string)reader["FileName"],
                    Views = (int)reader["Views"],
                    HashPassword = (string)reader["HashPassword"],
                    UserId = (int)reader["UserId"]
                });
            }
            connection.Close();
            connection.Dispose();
            return Images;
        }

        public void InsertImage(Image image)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Images VALUES (@fileName, @views, @password, @userId) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@fileName", image.FileName);
            cmd.Parameters.AddWithValue("@views", image.Views);
            cmd.Parameters.AddWithValue("@password", image.HashPassword);
            cmd.Parameters.AddWithValue("@userId", image.UserId);
            connection.Open();
            image.Id = (int)(decimal)cmd.ExecuteScalar();
            connection.Close();
            connection.Dispose();
        }

        public void UpdateView(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Images SET Views = Views + 1 WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
        }

        public Image GetImageById(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT TOP 1 * FROM Images WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Image> Images = new List<Image>();
            if (!reader.Read())
            {
                return null;
            }
            Image image = new Image
            {
                Id = (int)reader["id"],
                FileName = (string)reader["FileName"],
                Views = (int)reader["Views"],
                HashPassword = (string)reader["HashPassword"],
                UserId = (int)reader["UserId"]

            };
            connection.Close();
            connection.Dispose();
            return image;
        }
    }
}
