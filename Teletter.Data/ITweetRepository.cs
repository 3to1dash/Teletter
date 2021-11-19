using System;
using System.Collections.Generic;
using System.Text;
using Teletter.Core;

namespace Teletter.Data
{
    public interface ITweetRepository
    {
        void saveTweet(int userId, string content);
        List<Tweet> getTweetsByUserId(int userId);
    }
}
