using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.XConnect;

namespace xConnectTwitter.Model
{
    public class Tweet : Facet
    {
        public const string FacetName = "Tweet";

        public string Text { get; set; }
    }
}
