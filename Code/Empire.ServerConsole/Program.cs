using Empire.Core;
using Illisian.Lidgren3;
using Illisian.UnityUtil.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Empire.ServerConsole
{
    class Program
    {

        static Server _server;

        static void Main(string[] args)
        {
            LogManager.Context.LogEvent += Context_LogEvent;

            _server = new Server();
            _server.Initialise();
            do
            {
                switch (Console.ReadLine())
                {
                    case "quit":
                        _server.Shutdown();
                        Console.WriteLine("Press the any key to quit..");
                        Console.ReadKey();
                        Environment.Exit(Environment.ExitCode);
                        break;
                }
            } while (true);
        }

        public static void Context_LogEvent(LogType type, string message, params object[] objects)
        {
            switch (type)
            {
                case LogType.Information:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[INFO] {0}", message);
                    break;
                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[WARN] {0}", message);
                    break;
                case LogType.Critical:
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("[CRITICAL] {0}", message);
                    break;
                case LogType.Debug:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("[DEBUG] {0}", message);
                    break;
            }
            Console.ResetColor();
        }
    }
}
