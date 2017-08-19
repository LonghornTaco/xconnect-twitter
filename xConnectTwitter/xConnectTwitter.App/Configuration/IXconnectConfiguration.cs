using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xConnectTwitter.App.Configuration
{
	public interface IXconnectConfiguration
	{
		string XconnectUrl { get; }
		string TwitterChannelId { get; }
		string TwitterEngagementGoalId { get; }
	}
}
