using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.XConnect;
using Sitecore.XConnect.Schema;
using Sitecore.XConnect.Serialization;
using xConnectTwitter.Model;

namespace xConnectTwitter.ModelExporterApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new XdbModelBuilder("twitterModel", new XdbModelVersion(0, 1));

            builder.DefineFacet<Interaction, Tweet>(Tweet.FacetName);

            builder.DefineEventType<TweetEvent>(true);

            var xdbModel = builder.BuildModel();

            var json = XdbModelWriter.Serialize(xdbModel);
            File.WriteAllText("custommodel.json", json);
        }
    }
}
