using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Controller;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerController server = new ServerController();
            server.StartServer();
            Console.ReadLine();

            server.CloseServer();
        }
    }
}
