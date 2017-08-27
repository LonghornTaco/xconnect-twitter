using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;
using xConnectTwitter.App.Configuration;
using xConnectTwitter.App.Logging;
using xConnectTwitter.App.Repository;
using xConnectTwitter.App.Service;
using xConnectTwitter.App.Social;

namespace xConnectTwitter.App
{
    class Program
    {
        private static Container _container;
        private static AppRunner _appRunner;

        static void Main(string[] args)
        {
            SetupContainer();
            _appRunner = _container.GetInstance<AppRunner>();

            int inputResponse = -1;
            do
            {
                PrintMenu();
                inputResponse = GetInput();

                switch (inputResponse)
                {
                    case 1:
                        _appRunner.MonitorTwitter();
                        break;
                    case 2:
                        _appRunner.BuildContactModel();
                        break;
                    case 3:
                        _appRunner.ForgetAllContacts();
                        break;
                    case 4:
                        _appRunner.ShowAllContacts();
                        break;
                    case 5:
                        _appRunner.ViewSingleContact();
                        break;
                }

            } while (inputResponse != 0);

            _appRunner.Dispose();
            Environment.Exit(0);
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
            _container.Register<IModelProvider, XconnectModelProvider>();

            _container.Register<AppRunner>();
        }

        static void PrintMenu()
        {
            Console.WriteLine("########################################################");
            Console.WriteLine("##                                                    ##");
            Console.WriteLine("##                     Main Menu                      ##");
            Console.WriteLine("##                                                    ##");
            Console.WriteLine("########################################################\n");
            Console.WriteLine("What would you like to do?\n");
            Console.WriteLine("1. Monitor Twitter");
            Console.WriteLine("2. Build the model");
            Console.WriteLine("3. Forget all contacts");
            Console.WriteLine("4. Display all contacts");
            Console.WriteLine("5. View a single contact");

            Console.WriteLine("\n0. Exit");
            Console.Write("\n: ");
        }

        static int GetInput()
        {
            int returnValue = 0;
            var input = Console.ReadLine();
            if (!int.TryParse(input, out returnValue))
            {
                Console.WriteLine("Invalid entry");
            }
            else
            {
                Console.Clear();
            }
            return returnValue;
        }
    }
}