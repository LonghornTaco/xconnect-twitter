using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Events;
using Tweetinvi.Streaming;
using xConnectTwitter.App.Configuration;
using xConnectTwitter.App.Logging;

namespace xConnectTwitter.App.Social
{
	public class TwitterHarvester : ISocialHarvester
	{
		private readonly ITwitterConfiguration _configuration;
		private readonly ILogger _logger;

		public event EventHandler<TweetReceivedEventArgs> OnTweetReceived;

		private IFilteredStream _stream;

		public TwitterHarvester(ITwitterConfiguration configuration, ILogger logger)
		{
			_configuration = configuration;
			_logger = logger;
		}

		public void Initialize()
		{
			_logger.Write("Configuring Twitter contact source...");

			Auth.SetUserCredentials(_configuration.ConsumerKey, _configuration.ConsumerSecret, _configuration.UserToken, _configuration.UserSecret);

			_stream = Stream.CreateFilteredStream();
			_stream.AddTrack(_configuration.PhrasesToTrack);
			_stream.MatchingTweetReceived += Stream_MatchingTweetReceived;

			_logger.WriteLine("Initialization of Twitter Harvester complete");
		}

		public void Start()
		{
			_stream?.StartStreamMatchingAllConditions();
		}

		public void Stop()
		{
            _stream?.StopStream();
		}

		private void Stream_MatchingTweetReceived(object sender, Tweetinvi.Events.MatchedTweetReceivedEventArgs e)
		{
			_logger.WriteLine($"Got a tweet from {e.Tweet.CreatedBy} [{e.Tweet.CreatedBy.ScreenName}]");
		    var user = User.GetUserFromId(e.Tweet.CreatedBy.Id);
			OnTweetReceived?.Invoke(this, new TweetReceivedEventArgs(e.Tweet, e.Json, user));
		}
	}
}
