using System;
using System.Collections.Generic;
using System.Text;
using Teletter.Core;

namespace Teletter.Data
{
    public interface ITweetRepository
    {
        Tweet saveTweet(int userId, string content);
        Tweet getTheLastestAddedTweet();
        List<Tweet> getTweetsByUserId(int userId);
    }
}
