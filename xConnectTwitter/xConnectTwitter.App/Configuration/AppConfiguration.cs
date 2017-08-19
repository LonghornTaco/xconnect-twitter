using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xConnectTwitter.App.Configuration
{
	public class AppConfiguration : ITwitterConfiguration, IXconnectConfiguration
	{
		public string ConsumerKey => GetValue<string>("ConsumerKey");
		public string ConsumerSecret => GetValue<string>("ConsumerSecret");
		public string UserToken => GetValue<string>("UserToken");
		public string UserSecret => GetValue<string>("UserSecret");
		public string PhrasesToTrack => GetValue<string>("PhrasesToTrack");

		public string XconnectUrl => GetValue<string>("XconnectUrl");
		public string TwitterChannelId => GetValue<string>("TwitterChannelId");
		public string TwitterEngagementGoalId => GetValue<string>("TwitterEngagementGoalId");

		private static T GetValue<T>(string key)
		{
			var returnValue = default(T);
			var converter = TypeDescriptor.GetConverter(typeof(T));
			object value = ConfigurationManager.AppSettings[key];
			if (value != null)
			{
				try
				{
					returnValue = (T)converter.ConvertFrom(value);
				}
				catch (Exception)
				{
					Trace.TraceError($"Failed trying to convert '{value}' to type '{key}'");
				}
			}
			else
			{
				Trace.TraceError($"Could not find the config value '{key}'");
			}
			return returnValue;
		}
	}
}
