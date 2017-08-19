using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xConnectTwitter.App.Logging
{
	public class ConsoleLogger : ILogger
	{
		public void WriteLine(string message)
		{
			Console.WriteLine(message);
		}
		public void Write(string message)
		{
			Console.Write(message);
		}
	}
}
