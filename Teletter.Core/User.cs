using System;
using System.Collections.Generic;

namespace Teletter.Core
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<Tweet> Tweets { get; set; }
    }
}
