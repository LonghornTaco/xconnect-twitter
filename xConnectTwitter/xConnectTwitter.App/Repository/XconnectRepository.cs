using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Schema;
using xConnectTwitter.App.Configuration;
using xConnectTwitter.App.Logging;

namespace xConnectTwitter.App.Repository
{
	public class XconnectRepository : IContactRepository
	{
		private readonly IXconnectConfiguration _xconnectConfiguration;
		private readonly ILogger _logger;

		private XConnectClient _client = null;

		public XconnectRepository(IXconnectConfiguration xconnectConfiguration, ILogger logger)
		{
			_xconnectConfiguration = xconnectConfiguration;
			_logger = logger;

			Initialize();
		}

		public void SaveContact(string source, string id)
		{
			// check if contact exists already
			var contactReference = new IdentifiedContactReference(source, id);
			var contact = _client.Get(contactReference, new ExpandOptions());
			if (contact != null)
			{
				_logger.WriteLine("Contact already exists.");
			}
			else
			{
				var contactId = new ContactIdentifier(source, id, ContactIdentifierType.Known);
				contact = new Contact(contactId);
				_client.AddContact(contact);
			}

			var interaction = new Interaction(contact, InteractionInitiator.Contact, new Guid(_xconnectConfiguration.TwitterChannelId), source);
			var goal = new Goal(new Guid(_xconnectConfiguration.TwitterEngagementGoalId), DateTime.UtcNow);
			interaction.Events.Add(goal);

			//_client.AddInteraction(interaction);

			//_client.Submit();
			_logger.WriteLine($"Saving {id} to xConnect");
		}

		private void Initialize()
		{
			_logger.Write("Initializing xConnect...");

			//var config = new XConnectClientConfiguration(XConnectCoreModel.Instance, new Uri(_xconnectConfiguration.XconnectUrl));
			//config.Initialize();
			var config = new XConnectClientConfiguration(new XdbRuntimeModel(Sitecore.XConnect.Collection.Model.CollectionModel.Model), new Uri(_xconnectConfiguration.XconnectUrl), new Uri(_xconnectConfiguration.XconnectUrl));
			config.Initialize();

			_client = new XConnectClient(config);

			_logger.WriteLine("xConnect initialization complete");
		}
	}
}
