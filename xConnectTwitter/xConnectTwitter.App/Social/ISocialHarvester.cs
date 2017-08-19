using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Events;

namespace xConnectTwitter.App.Social
{
	public interface ISocialHarvester
	{
		void Initialize();
		void Start();
		void Stop();
		event EventHandler<MatchedTweetReceivedEventArgs> OnTweetReceived;
	}
}
