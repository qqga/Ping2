using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Ping2
{

    public class Ping2
    {
        Ping _Ping = new Ping();

        public DateTime Created { get; } = DateTime.Now;
        string _FileName;
        public PingStatistic Statistic { get; private set; }
        readonly bool _LogFile;
        readonly bool _UseSound;

        bool _PrevPingOk = false;

        public int ErrorFrequency { get; set; } = 1800;
        public int ErrorDuration { get; set; } = 400;

        public int SuccessFrequency { get; set; } = 777;
        public int SuccessDuration { get; set; } = 777;


        public Ping2(bool logFile, bool useSound)
        {
            _UseSound = useSound;
            _LogFile = logFile;
            _FileName = Created.ToString("dd.MM.yyyy HH.mm.ss.fff") + ".txt";
        }

        public async Task Ping(string host, int timeout)
        {
            if(Statistic == null)
                Statistic = new PingStatistic();

            bool error = false;
            PingReply pingReply = null;
            try
            {
                pingReply = await _Ping.SendPingAsync(host, timeout);
            }
            catch(Exception ex)
            {
                Statistic.AddError(ex);
                
                Log("Error: " + ex.GetMsg(), ConsoleColor.Red);
                error = true;
                //throw;
            }
            finally
            {
                if(pingReply != null)
                {
                    Statistic.AddStatistic(pingReply);
                    Log(pingReply);
                }
                NotifySound(error, pingReply);
            }
        }

        void NotifySound(bool error, PingReply? pingReply)
        {
            if(!_UseSound)
                return;

            if(error || !(pingReply?.Status == IPStatus.Success))
            {
                Console.Beep(ErrorFrequency, ErrorDuration);
                _PrevPingOk = false;
            }
            else 
            {
                if(!_PrevPingOk)
                    Console.Beep(SuccessFrequency, SuccessDuration);
                _PrevPingOk = true;
            }
        }

        public async Task Ping(string host, int timeout, int period, CancellationToken cancellationToken)
        {

            while(!cancellationToken.IsCancellationRequested)
            {
                DateTime dt1 = DateTime.Now;
                await Ping(host, timeout);
                var duration = DateTime.Now - dt1;
                var delay = period - duration.TotalMilliseconds;
                if(delay > 0)
                    await Task.Delay((int)delay);

            }
        }

        void Log(PingReply pingReply)
        {
            string msg = $"{pingReply.Address}; {pingReply.Status}; {pingReply.RoundtripTime} ms";
            Log(msg, pingReply.Status == IPStatus.Success? ConsoleColor.Green: ConsoleColor.Red);
        }

        void Log(string msg, ConsoleColor consoleColor = ConsoleColor.White)
        {
            msg = $"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss:fff")}; {msg}";
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(msg);
            Console.ResetColor();
            LogFile(msg);
        }

        public void LogStatistic()
        {
            Console.ForegroundColor =  ConsoleColor.Cyan;
            Console.WriteLine(Statistic);
            Console.ResetColor();

            LogFile(Statistic.ToString());
        }
        void LogFile(string msg)
        {
            if(_LogFile)
                File.AppendAllText(_FileName, msg + "\r\n");
        }

        public class PingStatistic
        {
            public DateTime DateTimeStart { get; } = DateTime.Now;
            public TimeSpan Duration => DateTime.Now - DateTimeStart;
            public Dictionary<IPStatus, int> StatusStatistic { get; } = new Dictionary<IPStatus, int>();
            public long MinTime { get; private set; } = 0;
            public long MaxTime { get; private set; } = 0;
            public int Count { get; private set; } = 0;
            public long TotalTime { get; private set; } = 0;

            public List<string> Errors = new List<string>();

            public void AddStatistic(PingReply pingReply)
            {
                Count++;
                TotalTime += pingReply.RoundtripTime;

                if(pingReply.RoundtripTime > MaxTime)
                    MaxTime = pingReply.RoundtripTime;

                if(MinTime == 0 || MinTime > pingReply.RoundtripTime)
                    MinTime = pingReply.RoundtripTime;

                if(StatusStatistic.ContainsKey(pingReply.Status))
                    StatusStatistic[pingReply.Status] = StatusStatistic[pingReply.Status] + 1;
                else
                    StatusStatistic[pingReply.Status] = 1;

            }

            public override string ToString()
            {
                var msg =
@$"----------------------
Duration: {Duration}

AvgTime: {TotalTime / (Count - Errors.Count)};
MinTime: {MinTime};
MaxTime: {MaxTime};

Total count: {Count};
Errors: {Errors.Count};
Status:";
                foreach(KeyValuePair<IPStatus, int> iPStatus in StatusStatistic)
                {
                    msg += $"{iPStatus.Key}: {iPStatus.Value}";
                }

                msg += "\r\n----------------------";
                return msg;
            }

            public void AddError(global::System.Exception ex)
            {
                Count++;

                Errors.Add(ex.GetMsg());
            }
        }
    }
}
