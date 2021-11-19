using System;
using System.Collections.Generic;
using System.Text;

namespace Teletter.Core
{
    public class Tweet
    {
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
