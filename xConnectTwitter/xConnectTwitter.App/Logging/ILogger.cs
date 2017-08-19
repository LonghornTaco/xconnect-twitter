using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xConnectTwitter.App.Logging
{
	public interface ILogger
	{
		void Write(string message);
		void WriteLine(string message);
	}
}
