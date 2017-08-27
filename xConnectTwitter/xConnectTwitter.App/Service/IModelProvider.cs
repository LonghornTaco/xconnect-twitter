using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.XConnect.Schema;

namespace xConnectTwitter.App.Service
{
    public interface IModelProvider
    {
        XdbModel Model { get; }
        void ExportModel();
    }
}
