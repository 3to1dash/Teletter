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

        public Tweet getTheLastestAddedTweet()
        {
            var sql = @"SELECT TOP 1 Content, CreatedAt, UpdatedAt
                        From Tweets
                        ORDER BY TweetID DESC";
            using var connect = new SqlConnection(_configuration.GetConnectionString("TeletterDB"));
            var tweet = connect.Query<Tweet>(sql).FirstOrDefault();
            return tweet;
        }

        public Tweet saveTweet(int userId, string content)
        {
            var param = new DynamicParameters();
            param.Add("@Content", content, DbType.String, ParameterDirection.Input, content.Length);
            param.Add("@UserId", userId, DbType.Int32, ParameterDirection.Input, userId.ToString().Length);
            var sql = @"INSERT INTO Tweets (Content, UserId)
                        VALUES (@Content, @UserId)";

            using var connection = new SqlConnection(_configuration.GetConnectionString("TeletterDB"));

            try
            {
                connection.Execute(sql, param);
            }
            catch (Exception)
            {
                return null;
            } 

            return getTheLastestAddedTweet();
        }
    }
}
