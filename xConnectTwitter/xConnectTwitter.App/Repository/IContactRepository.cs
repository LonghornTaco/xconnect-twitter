using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.XConnect;
using Tweetinvi.Models;

namespace xConnectTwitter.App.Repository
{
	public interface IContactRepository
	{
	    void Initialize();
		void SaveContact(string source, IUser user);
	    void RegisterTweetEvent(string source, ITweet tweet);
	    void Submit();
	    void ForgetAllContacts();
	    void DisplayAllContacts();
	    void ViewSingleContact(string identifier);
	}
}
