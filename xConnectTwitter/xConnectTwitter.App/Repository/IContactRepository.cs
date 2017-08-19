using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xConnectTwitter.App.Repository
{
	public interface IContactRepository
	{
		void SaveContact(string source, string id);
	}
}
