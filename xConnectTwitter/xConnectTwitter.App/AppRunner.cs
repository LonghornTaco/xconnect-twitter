using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Events;
using xConnectTwitter.App.Configuration;
using xConnectTwitter.App.Repository;
using xConnectTwitter.App.Social;

namespace xConnectTwitter.App
{
	public class AppRunner : IDisposable
	{
		private readonly ISocialHarvester _socialHarvester;
		private readonly IContactRepository _contactRepository;

		public AppRunner(ISocialHarvester socialHarvester, IContactRepository contactRepository)
		{
			_socialHarvester = socialHarvester;
			_contactRepository = contactRepository;
		}

		public void Run()
		{
			_socialHarvester.Initialize();
			_socialHarvester.OnTweetReceived += SocialHarvesterOnTweetReceived;
			_socialHarvester.Start();
		}

		private void SocialHarvesterOnTweetReceived(object sender, MatchedTweetReceivedEventArgs tweetArgs)
		{
			_contactRepository.SaveContact("twitter", tweetArgs.Tweet.CreatedBy.ScreenName);
		}

		public void Dispose()
		{
			_socialHarvester.Stop();
			_socialHarvester.OnTweetReceived -= SocialHarvesterOnTweetReceived;
		}
	}
}
