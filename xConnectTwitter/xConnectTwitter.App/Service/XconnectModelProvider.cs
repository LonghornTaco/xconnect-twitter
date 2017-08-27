using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.XConnect;
using Sitecore.XConnect.Schema;
using xConnectTwitter.App.Configuration;
using xConnectTwitter.App.Model;

namespace xConnectTwitter.App.Service
{
    public class XconnectModelProvider : IModelProvider
    {
        private readonly IXconnectConfiguration _xconnectConfiguration;

        private XdbModel _model;
        public XdbModel Model => _model ?? (_model = BuildModel());

        public XconnectModelProvider(IXconnectConfiguration xconnectConfiguration)
        {
            _xconnectConfiguration = xconnectConfiguration;
        }

        public void ExportModel()
        {
            var serializedModel = Sitecore.XConnect.Serialization.XdbModelWriter.Serialize(Model);
            File.WriteAllText(_xconnectConfiguration.OutputDirectory + Model.FullName + ".json", serializedModel);
        }

        private static XdbModel BuildModel()
        {
            var builder = new XdbModelBuilder("xConnectTwitter.Model", new XdbModelVersion(1, 0));
            builder.DefineFacet<Interaction, Tweet>(Tweet.FacetName);
            builder.DefineEventType<TweetEvent>(true);
            builder.ReferenceModel(Sitecore.XConnect.Collection.Model.CollectionModel.Model);
            var xdbModel = builder.BuildModel();
            return xdbModel;
        }
    }
}
