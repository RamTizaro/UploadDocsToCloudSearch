using System;
using System.Collections.Generic;
using System.IO;

namespace Tizaro.CloudSearch
{
    internal static class Logger
    {
      
        public static void StartLog(DateTime RunDate)
        {
            string logPath = Path.Combine(TizaroConfiguration.LogPath, "UploadTheDocsToCloudSearch - " + RunDate.ToString("yyyyMMdd") + ".log");
            _logWriter = new StreamWriter(logPath, true);
            _logWriter.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff") + " LOG START");
        }

        public static void CloseLog()
        {
            try
            {
                _logWriter.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff") + " LOG END");
                _logWriter.WriteLine("- - - - - - - - - - - - - - - - - -");
                _logWriter.Flush();
                _logWriter.Close();
            }
            catch
            {
            }
            finally
            {
                _logWriter.Dispose();
            }
        }
               
        public static void LogError(string Message)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff") + " ERROR: " + Message);
            _logWriter.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff") + " ERROR: " + Message);
        }

        public static void LogMessage(string Message)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff") + " INFO: " + Message);
            _logWriter.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff") + " INFO: " + Message);
        }

        public static void Warn(string Message)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff") + " WARN: " + Message);
            _logWriter.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff") + " WARN: " + Message);
        }

        private static StreamWriter _logWriter;
    }
}
