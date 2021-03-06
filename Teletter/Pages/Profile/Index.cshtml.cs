using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Teletter.Core;
using Teletter.Data;

namespace Teletter.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly ITweetRepository _tweetRepository;

        public IndexModel(IUserRepository userRepository, ITweetRepository tweetRepository)
        {
            _userRepository = userRepository;
            _tweetRepository = tweetRepository;
        }

        public User user { get; set; }
        public List<Tweet> tweets { get; set; }

        public void OnGet(int UserId)
        {
            user = _userRepository.GetUserById(UserId);
            tweets = _tweetRepository.getTweetsByUserId(UserId);
        }

        public JsonResult OnPost([FromBody] InputModel input)
        {
            var currentUser = this.User;
            var userId = Int32.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
            Tweet tweet = _tweetRepository.saveTweet(userId, input.Content);
            return new JsonResult(tweet);
        }

        public class InputModel
        {
            [Required]
            [StringLength(345, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
            public string Content { get; set; }
        }
    }
}
