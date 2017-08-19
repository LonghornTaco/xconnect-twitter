using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xConnectTwitter.App.Configuration
{
	public interface ITwitterConfiguration
	{
		string ConsumerKey { get; }
		string ConsumerSecret { get; }
		string UserToken { get; }
		string UserSecret { get; }
		string PhrasesToTrack { get; }
	}
}
