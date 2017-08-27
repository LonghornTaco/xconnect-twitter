using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.XConnect;

namespace xConnectTwitter.App.Model
{
    public class TweetEvent : Event
    {
        public TweetEvent(Guid definitionId, DateTime timestamp) : base(definitionId, timestamp)
        {
        }

        public string TargetPhrase { get; set; }
    }
}
