using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Teletter.Core;

namespace Teletter.Data
{
    public class TweetRepository : ITweetRepository
    {
        private readonly IConfiguration _configuration;

        public TweetRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Tweet> getTweetsByUserId(int userId)
        {
            var sql = @"SELECT Content, CreatedAt, UpdatedAt
                        FROM Tweets
                        WHERE UserID = @userId
                        Order By CreatedAt DESC";

            using var connect = new SqlConnection(_configuration.GetConnectionString("TeletterDB"));
            var tweets = connect.Query<Tweet>(sql, new { userId }).Distinct().ToList();
            return tweets;
        }

        public void saveTweet(int userId, string content)
        {
            var param = new DynamicParameters();
            param.Add("@Content", content, DbType.String, ParameterDirection.Input, content.Length);
            param.Add("@UserId", userId, DbType.Int32, ParameterDirection.Input, userId.ToString().Length);
            var sql = @"INSERT INTO Tweets (Content, UserId)
                        VALUES (@Content, @UserId)";

            using var connection = new SqlConnection(_configuration.GetConnectionString("TeletterDB"));

            connection.Execute(sql, param);
        }
    }
}
