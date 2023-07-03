using Microsoft.Extensions.Configuration;
using System;
using System.Net.NetworkInformation;

namespace Ping2
{
    internal class Program
    {
        static string ConfigFileName = "AppConfig.json";

        static void Main(string[] args)
        {
            Console.WriteLine("Ping");


            if(args.Length > 0 && (args[0] == "--help" || args[0] == "/help"))
                ShowHelp();

            var cfg = new Config(ConfigFileName, args);
            if(cfg.Data.GetErrors() is string error)
            {
                Console.WriteLine("Error: " +error);
                ShowHelp();
            }

            Console.WriteLine(cfg.Data);
            Console.WriteLine("Press Ctrl + E to exit. Press Ctrl + S to show statistic.");

            var ping = new Ping2(cfg.Data.LogFileEnabled, cfg.Data.SoundEnabled)
            {
                ErrorDuration = cfg.Data.ErrorDuration,
                ErrorFrequency = cfg.Data.ErrorFrequency,
                SuccessDuration = cfg.Data.SuccessDuration,
                SuccessFrequency = cfg.Data.SuccessFrequency,
            };

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            if(cfg.Data.Period.HasValue)
                _ = ping.Ping(cfg.Data.Host, cfg.Data.Timeout, cfg.Data.Period.Value, cancellationTokenSource.Token);
            else
                _ = ping.Ping(cfg.Data.Host, cfg.Data.Timeout);

            while(Console.ReadKey() is ConsoleKeyInfo key)
            {
                if(key.Key == ConsoleKey.E && key.Modifiers == ConsoleModifiers.Control)
                {
                    cancellationTokenSource.Cancel();
                    ping.LogStatistic();
                    Environment.Exit(0);
                }
                if(key.Key == ConsoleKey.S && key.Modifiers == ConsoleModifiers.Control)
                {
                    ping.LogStatistic();
                }
            }
        }

        static void ShowHelp()
        {
            foreach(var keys in Config.KeyMaping)
            {
                Console.WriteLine($"{keys.Key},{keys.Value.name}  -  {keys.Value.description}");
            }
            Environment.Exit(0);
        }
    }
}