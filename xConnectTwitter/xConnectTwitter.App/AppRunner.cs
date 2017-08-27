using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Events;
using xConnectTwitter.App.Configuration;
using xConnectTwitter.App.Logging;
using xConnectTwitter.App.Repository;
using xConnectTwitter.App.Service;
using xConnectTwitter.App.Social;
using TweetReceivedEventArgs = xConnectTwitter.App.Social.TweetReceivedEventArgs;

namespace xConnectTwitter.App
{
	public class AppRunner : IDisposable
	{
		private readonly ISocialHarvester _socialHarvester;
		private readonly IContactRepository _contactRepository;
	    private readonly IModelProvider _modelProvider;
	    private readonly ILogger _logger;

		public AppRunner(ISocialHarvester socialHarvester, IContactRepository contactRepository, IModelProvider modelProvider, ILogger logger)
		{
			_socialHarvester = socialHarvester;
			_contactRepository = contactRepository;
		    _modelProvider = modelProvider;
		    _logger = logger;
		}

		public void MonitorTwitter()
		{
			_socialHarvester.Initialize();
            _contactRepository.Initialize();
			_socialHarvester.OnTweetReceived += SocialHarvesterOnTweetReceived;
			_socialHarvester.Start();
		}

	    public void BuildContactModel()
	    {
	        _logger.Write("Generating your model...");
            _modelProvider.ExportModel();
            _logger.WriteLine("Your model has been generated!\n\nPress any key to continue...");
	        Console.ReadLine();
	    }

	    public void ForgetAllContacts()
	    {
	        _contactRepository.Initialize();
            _contactRepository.ForgetAllContacts();
            _contactRepository.Submit();
	        Console.ReadLine();
	        Console.Clear();
        }

	    public void ShowAllContacts()
	    {
	        _contactRepository.DisplayAllContacts();
	        Console.ReadLine();
	        Console.Clear();
        }

	    public void ViewSingleContact()
	    {
	        _logger.WriteLine("Please enter the identifier of the contact:");
	        var identifier = Console.ReadLine();
	        _contactRepository.ViewSingleContact(identifier);
	        Console.ReadLine();
            Console.Clear();
	    }

		private void SocialHarvesterOnTweetReceived(object sender, TweetReceivedEventArgs tweetReceivedEventArgs)
		{
		    _contactRepository.SaveContact("twitter", tweetReceivedEventArgs.User);
            _contactRepository.RegisterTweetEvent("twitter", tweetReceivedEventArgs.Tweet);
            _contactRepository.Submit();

        }

		public void Dispose()
		{
		    if (_socialHarvester != null)
		    {
		        _socialHarvester.Stop();
                _socialHarvester.OnTweetReceived -= SocialHarvesterOnTweetReceived;
            }
		}
	}
}
