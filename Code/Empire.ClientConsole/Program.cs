﻿using Empire.Core;
using Illisian.UnityUtil.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire.ClientConsole
{
    class Program
    {
        static Client _server;

        static void Main(string[] args)
        {
            LogManager.Context.LogEvent += Context_LogEvent;
            Console.WriteLine("Press the any key to start..");
            Console.ReadKey();
            _server = new Client();
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
