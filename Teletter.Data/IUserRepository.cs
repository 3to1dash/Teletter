using System;
using System.Collections.Generic;
using System.Text;
using Teletter.Core;

namespace Teletter.Data
{
    public interface IUserRepository
    {
        int SaveUser(string userName, string email, string password);
        User GetUserById(int id);
        List<User> getUserWithTweets(int userId);
        Result GetUserByEmailAndPassword(string email, string password);
    }
}
