using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ping2
{
    public class Config
    {
        public ConfigData Data { get; } = new ConfigData();

        public Config(string fileName, string[] args)
        {
            new Microsoft.Extensions.Configuration.ConfigurationManager()
                .AddJsonFile(fileName)
                .AddCommandLine(args, KeyMaping.ToDictionary(_ => _.Key, _ => _.Value.name))
                .Build()
                .Bind(Data);
        }

        public static Dictionary<string, (string name, string description)> KeyMaping = new Dictionary<string, (string, string)>()
        {
            { "-h", ("Host", "Host name or address.") },
            { "-t", ("Timeout", "Ping timeout. Default 4000.") },
            { "-p", ("Period", "Ping period. If not set - send ping one time.") },
            { "-f", ("LogFile", "Log ping results into file. Default true.") },

            { "-s", ("SoundEnabled", "Beep when ping status error or status ok after error. Default true.") },
            { "-ef", ("ErrorFrequency", "Default 1800.") },
            { "-ed", ("ErrorDuration", "Default 400.") },
            { "-sf", ("SuccessFrequency", "Default 777.") },
            { "-sd", ("SuccessDuration", "Default 777.") },
        };

    }

    public class ConfigData
    {
        public string Host { get; set; }
        public int Timeout { get; set; } = 4000;
        public int? Period { get; set; } = null;
        public bool LogFileEnabled { get; set; } = true;


        public bool SoundEnabled { get; set; } = true;
        public int ErrorFrequency { get; set; } = 1800;
        public int ErrorDuration { get; set; } = 400;
        public int SuccessFrequency { get; set; } = 777;
        public int SuccessDuration { get; set; } = 777;


        public override string ToString()
        {
            return $"Host:'{Host}'; Timeout:{Timeout}; Period:{Period}";
        }

        public string? GetErrors()
        {
            if(String.IsNullOrEmpty(Host))
                return "Host is null or empty.";

            return null;
        }
    }
}
