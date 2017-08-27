using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Schema;
using Sitecore.XConnect.Search.Queries;
using Tweetinvi.Models;
using xConnectTwitter.App.Configuration;
using xConnectTwitter.App.Logging;
using xConnectTwitter.App.Model;
using xConnectTwitter.App.Service;

namespace xConnectTwitter.App.Repository
{
    public class XconnectRepository : IContactRepository
    {
        private readonly IXconnectConfiguration _xconnectConfiguration;
        private readonly ITwitterConfiguration _twitterConfiguration;
        private readonly IModelProvider _modelProvider;
        private readonly ILogger _logger;

        private XConnectClient _client = null;
        private Contact _contact = null;

        public XconnectRepository(IXconnectConfiguration xconnectConfiguration, ILogger logger, IModelProvider modelProvider, ITwitterConfiguration twitterConfiguration)
        {
            _xconnectConfiguration = xconnectConfiguration;
            _logger = logger;
            _modelProvider = modelProvider;
            _twitterConfiguration = twitterConfiguration;

            Initialize();
        }

        public void SaveContact(string source, IUser user)
        {
            _logger.WriteLine("Searching for an existing user...");
            var contactReference = new IdentifiedContactReference(source, user.ScreenName);
            _contact = _client.Get(contactReference, new ExpandOptions() {FacetKeys = { "Personal", "Emails" } });

            if (_contact != null)
            {
                _logger.WriteLine("The user was found");
                if (!string.IsNullOrEmpty(user.UserDTO.Email))
                {
                    var emailFacet = _contact.GetFacet<EmailAddressList>(EmailAddressList.DefaultFacetKey);
                    if (emailFacet != null)
                    {
                        emailFacet.PreferredEmail = new EmailAddress(user.UserDTO.Email, true);
                        emailFacet.PreferredKey = "twitter";
                        _client.SetFacet(_contact, EmailAddressList.DefaultFacetKey, emailFacet);
                    }
                }
            }
            else
            {
                _logger.WriteLine("A contact was not found so one is being created");
                var contactId = new ContactIdentifier(source, user.ScreenName, ContactIdentifierType.Known);
                _contact = new Contact(contactId);

                _logger.WriteLine("Updating contact information");
                var firstName = string.Empty;
                var lastName = string.Empty;

                if (!string.IsNullOrEmpty(user.Name))
                {
                    if (user.Name.Contains(" "))
                    {
                        var split = user.Name.Split(' ');
                        firstName = split[0] ?? string.Empty;
                        lastName = split[1] ?? string.Empty;
                    }
                }

                var personalInfoFacet = _contact.GetFacet<PersonalInformation>() ?? new PersonalInformation();
                personalInfoFacet.FirstName = firstName;
                personalInfoFacet.LastName = lastName;
                personalInfoFacet.Nickname = user.Name;

                _client.AddContact(_contact);
                _client.SetFacet(_contact, PersonalInformation.DefaultFacetKey, personalInfoFacet);

                if (!string.IsNullOrEmpty(user.UserDTO.Email))
                {
                    var emailFacet = new EmailAddressList(new EmailAddress(user.UserDTO.Email, true), "twitter");
                    _client.SetFacet(_contact, EmailAddressList.DefaultFacetKey, emailFacet);
                }

                _logger.WriteLine($"A contact for {user.Name} was successfully created.");
            }
        }

        public void RegisterTweetEvent(string source, ITweet tweet)
        {
             _logger.WriteLine($"Registering a tweet event for {_contact.Personal()?.Nickname ?? _contact.Id.ToString()}");
            var interaction = new Interaction(_contact, InteractionInitiator.Contact, Guid.Parse(_xconnectConfiguration.TwitterChannelId), source);
            _client.SetFacet<Tweet>(interaction, Tweet.FacetName, new Tweet() { Text = tweet.FullText });

            var goal = new Goal(Guid.Parse(_xconnectConfiguration.TwitterEngagementGoalId), DateTime.UtcNow);
            interaction.Events.Add(goal);

            var tweetEvent = new TweetEvent(Guid.Parse(_xconnectConfiguration.MonitoredPhraseOnTwitterEventId), DateTime.UtcNow)
            {
                TargetPhrase = GetPhraseMatches(tweet.FullText)
            };
            interaction.Events.Add(tweetEvent);

            _client.AddInteraction(interaction);
            _logger.WriteLine("Tweet event successfully registered");
        }

        public void Submit()
        {
            try
            {
                _client.Submit();
                _logger.WriteLine("Data successfull saved to xConnect");
            }
            catch (Exception ex)
            {
                _logger.WriteLine($"There was a problem saving the data to xConnect: {ex.Message}");
                //throw;
            }
        }

        public void ForgetAllContacts()
        {
            _logger.WriteLine("Forgetting all contacts...");
            var enumerator = _client.Contacts.GetBatchEnumeratorSync();
            
            while (enumerator.MoveNext())
            {
                var batch = enumerator.Current;
                foreach (var contact in batch)
                {
                    _logger.WriteLine($"Forgetting {contact.Id}");
                    _client.ExecuteRightToBeForgotten(contact);
                }
            }
            _logger.WriteLine("All contacts forgotten.");
        }

        public void DisplayAllContacts()
        {
            var enumerator = _client.Contacts.GetBatchEnumeratorSync();

            _logger.WriteLine($"Found {enumerator.TotalCount} contacts");

            _logger.WriteLine("Identifier\t\tID");
            _logger.WriteLine("#################################################################################");
            while (enumerator.MoveNext())
            {
                var batch = enumerator.Current;
                foreach (var contact in batch)
                {
                    _logger.WriteLine($"{contact.Identifiers.First().Identifier}\t\t{contact.Id}");
                }
            }
            _logger.WriteLine("");
            _logger.WriteLine("Press any key to continue...");
        }

        public void ViewSingleContact(string identifier)
        {
            var expandOptions = new ContactExpandOptions()
            {
                Interactions = new RelatedInteractionsExpandOptions(Tweet.FacetName)
                {
                    EndDateTime = DateTime.UtcNow,
                    StartDateTime = DateTime.UtcNow.AddDays(-10)
                }
            };

            var contactReference = new IdentifiedContactReference("twitter", identifier);
            var contact = _client.Get(contactReference, expandOptions);

            if (contact == null)
            {
                _logger.WriteLine("Contact not found");
                return;
            }

            _logger.WriteLine("####################################");
            foreach (var interaction in contact.Interactions)
            {
                var date = string.Empty;
                var phrase = string.Empty;
                var tweetText = string.Empty;

                var tweet = interaction.GetFacet<Tweet>();
                if (tweet != null)
                {
                    date = interaction.StartDateTime.ToShortDateString();
                    tweetText = tweet.Text;
                }

                var tweetEvent = interaction.Events.FirstOrDefault(x => x is TweetEvent) as TweetEvent;
                if (tweetEvent != null)
                {
                    phrase = tweetEvent.TargetPhrase;
                }

                //_logger.WriteLine($"{date}\t\t{phrase}\t\t{tweetText}");
                _logger.WriteLine($"Date:\t{date}");
                _logger.WriteLine($"Phrase:\t{phrase}");
                _logger.WriteLine($"Tweet:\t{tweetText}");
                _logger.WriteLine("####################################");
                _logger.WriteLine("");
                _logger.WriteLine("Press any key to continue...");
            }
        }

        public void Initialize()
        {
            _logger.Write("Initializing xConnect...");

            var model = _modelProvider.Model;
            var config = new XConnectClientConfiguration(new XdbRuntimeModel(model), new Uri(_xconnectConfiguration.XconnectUrl), new Uri(_xconnectConfiguration.XconnectUrl));
            config.Initialize();

            _client = new XConnectClient(config);

            _logger.WriteLine("xConnect initialization complete");
        }

        private string GetPhraseMatches(string text)
        {
            var found = string.Empty;
            var phrases = _twitterConfiguration.PhrasesToTrack.Split(',').ToList();
            foreach (var phrase in phrases)
            {
                if (text.ToLower().Contains(phrase.ToLower()))
                {
                    found += $"{phrase},";
                }
            }
            return found;
        }
    }
}
