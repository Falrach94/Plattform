using System;
using TestProject;
using TextParserUtils.Parser;

namespace GameServer
{
    class Program
    {
        // static private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main()
        {
            //Target.Register<ServerLogTarget>("ServerLog");

            TestServer server = new TestServer();

            server.Start(100);

            while (server.Running)
            {
                string input = System.Console.ReadLine();
                try
                {
                    var output = server.TextInterface.CommandInterface.Command(input);
                    System.Console.WriteLine(output);
                }
                catch (ParseException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            System.Console.ReadKey();

        }
    }
}
