using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;
using xConnectTwitter.App.Configuration;
using xConnectTwitter.App.Logging;
using xConnectTwitter.App.Repository;
using xConnectTwitter.App.Social;

namespace xConnectTwitter.App
{
	class Program
	{
		private static Container _container;
		private static AppRunner _appRunner;

		static void Main(string[] args)
		{
			Console.CancelKeyPress += (sender, a) =>
			{
				_appRunner.Dispose();
				Environment.Exit(0);
			};

			Console.WriteLine("Press CTRL+C to exit");

			SetupContainer();

			_appRunner = _container.GetInstance<AppRunner>();
			_appRunner.Run();
		}

		static void SetupContainer()
		{
			var appConfig = new AppConfiguration();

			_container = new Container();
			_container.RegisterSingleton<ITwitterConfiguration>(appConfig);
			_container.RegisterSingleton<IXconnectConfiguration>(appConfig);
			_container.Register<ISocialHarvester, TwitterHarvester>();
			_container.Register<ILogger, ConsoleLogger>();
			_container.Register<IContactRepository, XconnectRepository>();

			_container.Register<AppRunner>();
		}
	}
}