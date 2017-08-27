using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Events;
using Tweetinvi.Models;

namespace xConnectTwitter.App.Social
{
    public class TweetReceivedEventArgs : MatchedTweetReceivedEventArgs
    {
        public IUser User { get; set; }

        public TweetReceivedEventArgs(ITweet tweet, string json, IUser user) : base(tweet, json)
        {
            User = user;
        }
    }
}
