using Microsoft.Data.Sqlite;
using SafeVault.Models;
using System.Text.RegularExpressions;

namespace SafeVault.Services
{
    public class UserService
    {
        private readonly string _connectionString;

        public UserService(string connectionString)
        {
            _connectionString = connectionString;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = """
                CREATE TABLE IF NOT EXISTS Users (
                    UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Email TEXT NOT NULL,
                    PasswordHash TEXT NOT NULL,
                    Role TEXT NOT NULL
                );
            """;
            cmd.ExecuteNonQuery();
        }

        public bool AddUser(string username, string email, string passwordHash, string role)
        {
            var sanitizedUsername = SanitizeInput(username);
            var sanitizedEmail = SanitizeInput(email);
            var sanitizedRole = SanitizeInput(role);

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Username, Email, PasswordHash, Role) VALUES (@u, @e, @p, @r)";
            cmd.Parameters.AddWithValue("@u", sanitizedUsername);
            cmd.Parameters.AddWithValue("@e", sanitizedEmail);
            cmd.Parameters.AddWithValue("@p", passwordHash);
            cmd.Parameters.AddWithValue("@r", sanitizedRole);

            return cmd.ExecuteNonQuery() > 0;
        }

        public User? GetUserByUsername(string username)
        {
            var sanitizedUsername = SanitizeInput(username);

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Username, Email, PasswordHash, Role FROM Users WHERE Username = @username";
            cmd.Parameters.AddWithValue("@username", sanitizedUsername);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Username = reader.GetString(0),
                    Email = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    Role = reader.GetString(3)
                };
            }

            return null;
        }

        public bool UserExists(string username)
        {
            return GetUserByUsername(username) != null;
        }

        public string SanitizeInput(string input)
        {
            var sanitized = Regex.Replace(input, "<.*?>", string.Empty);
            sanitized = Regex.Replace(sanitized, @"[^\w@\.-]", string.Empty);
            return sanitized.Trim();
        }
    }
}
