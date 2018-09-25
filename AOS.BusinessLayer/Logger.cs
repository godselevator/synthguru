using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace AOS.BusinessLayer
{
    public static class Logger
    {
        public static bool LogEnabled;

        static Logger()
        {
            var logEnabledAppSetting = ConfigurationManager.AppSettings["Logging"];

            var testBool = false;

            if (string.IsNullOrEmpty(logEnabledAppSetting))
                LogEnabled = false;
            else
                testBool = bool.TryParse(logEnabledAppSetting, out LogEnabled);
        }

        public static void LogEvent(string page, LogType type, string message)
        {
            try
            {
                if (LogEnabled)
                {
                    var fileName = Path.Combine(HostingEnvironment.MapPath(@"~"), "AOSLog-" + DateTime.Today.ToString("yyyyMMdd") + ".log");

                    message = message.Replace("\n", " ").Replace("\r", "").Trim('\t');

                    var logItem = $"{DateTime.Now} [{type}] - {page} - {message}\n";

                    File.AppendAllText(fileName, logItem.Replace("\n", "\r\n"));
                }
            }
            catch { /* just... don't crash, ok? */ }
        }
    }

    public enum LogType
    {
        Info = 1,
        Warning = 2,
        Error = 3
    }
}
