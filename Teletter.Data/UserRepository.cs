using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using Teletter.Core;
using System.Linq;
using System.Data;

namespace Teletter.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int SaveUser(string userName, string email, string password)
        {
            var param = new DynamicParameters();
            param.Add("@UserName", userName, DbType.AnsiString, ParameterDirection.Input, userName.Length);
            param.Add("@Email", email, DbType.AnsiString, ParameterDirection.Input, email.Length);
            var passwordHash = PasswordHelpers.HashPassword(password);
            param.Add("@PasswordHash", passwordHash, DbType.AnsiString, ParameterDirection.Input, passwordHash.Length);

            var sql = @"INSERT INTO Users (UserName, Email, PasswordHash)
                        VALUES (@UserName, @Email, @PasswordHash)";

            using var connection = new SqlConnection(_configuration.GetConnectionString("TeletterDB"));

            return connection.Execute(sql, param);
        }

        public User GetUserById(int id)
        {
            string sql = "SELECT * FROM Users WHERE UserId = @Id;";

            using var connection = new SqlConnection(_configuration.GetConnectionString("TeletterDB"));

            var user = connection.Query<User>(sql, new { Id = id }).FirstOrDefault();
            if (user == null)
            {
                return null;
            }

            return user;
        }

        public Result GetUserByEmailAndPassword(string email, string password)
        {
            var sql = "SELECT * FROM USERS WHERE Email = @email";
            var param = new { email };

            using var connection = new SqlConnection(_configuration.GetConnectionString("TeletterDB"));

            var dbUser = connection.Query<DatabaseUser>(sql, param).FirstOrDefault();
            if (dbUser == null)
            {
                return new Result
                {
                    User = null,
                    Message = "Could not find a user with that email!"
                };
            }

            var passwordHash = Convert.FromBase64String(dbUser.PasswordHash);

            if (PasswordHelpers.VerifyHashedPassword(passwordHash, password))
            {
                var user = new User
                {
                    UserId = dbUser.UserId,
                    UserName = dbUser.UserName,
                    Email = dbUser.Email
                };

                return new Result
                {
                    User = user,
                    Message = "Success"
                };
            }

            return new Result
            {
                User = null,
                Message = "Password Incorrect!"
            };
        }

        public List<User> getUserWithTweets(int userId)
        {
            var sql = @"SELECT u.UserId, UserName, Email, Content, CreatedAt, UpdatedAt
                        FROM Users as u
                        INNER JOIN Tweets as t
                        ON u.UserID = @userId and t.UserID = @userId;";

            using var connection = new SqlConnection(_configuration.GetConnectionString("TeletterDB"));

            var userWithTweets = connection.Query<User, Tweet, User>(sql,
                (user, tweet) =>
                {
                    User userEntry;

                    userEntry = user;
                    userEntry.Tweets = new List<Tweet>();
                    userEntry.Tweets.Add(tweet);
                    return userEntry;
                },
                new { userId },
                splitOn: "Content")
                .Distinct()
                .ToList();

            if (userWithTweets == null) return null;
            return userWithTweets;
        }
    }

    public struct Result
    {
        public User User;
        public string Message;
    }
}
