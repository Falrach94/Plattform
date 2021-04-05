using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace ServerConsole
{
    class Program
    {
        [Serializable]
        class T
        {
            public T(int v)
            {
                a = v;
            }
            [DataMember]
            public int a { get; set; } = 60;
        }

        static Stream stream = new MemoryStream();
        static void Main(string[] args)
        {
            TestServer server = new TestServer();



            try
            {
                server.Start(6666).Wait();
                Console.Out.WriteLine("Server Running");

                while (server.Running)
                {
                    Task.Delay(1000).Wait();
                }

                Console.Out.WriteLine("Server stopped!");
            }
            catch
            {
                Console.Out.WriteLine("Failed to start!");
            }


        }
    }
}
