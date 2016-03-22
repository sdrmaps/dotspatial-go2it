// Logging code lifted from HydroDesktop Application

using System;
using System.Diagnostics;
using System.IO;
using SdrConfig = SDR.Configuration;

namespace SDR.Common.logging
{
    class TraceLogInitializer : ILogInitializer
    {
        private const string LogFileName = "trace{0}.log";

        public TraceLogInitializer()
        {
            Destination = CreateTraceFile();
        }

        #region Implementation of ILogInitializer

        public string Destination { get; private set; }

        #endregion

        private static string CreateTraceFile()
        {
            // try to put the trace.log into the documents path
            var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SDR", SdrConfig.Settings.Instance.ApplicationName);
            var tempPath = Path.Combine(Path.GetTempPath(), SdrConfig.Settings.Instance.ApplicationName);

            var stream = TryToCreateLogFile(documentsPath) ?? TryToCreateLogFile(tempPath);

            // create the trace listener
            if (stream == null) return null;
            var myTextListener = new TextWriterTraceListener(stream);
            Trace.Listeners.Add(myTextListener);
            return stream.Name;
        }

        private static FileStream TryToCreateLogFile(string logFileDirectory)
        {
            if (!Directory.Exists(logFileDirectory))
            {
                try
                {
                    Directory.CreateDirectory(logFileDirectory);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Unable to create directory {0}: {1}", logFileDirectory, ex.Message);
                    return null;
                }
            }
            if (!Directory.Exists(logFileDirectory))
            {
                return null;
            }

            // at this point the directory exists
            var logFileName = string.Format(LogFileName, string.Empty);
            var fullPath = Path.Combine(logFileDirectory, logFileName);
            try
            {
                // rename previous file to LOG_FILE_NAME_yyyyMMdd.log
                if (File.Exists(fullPath))
                {
                    var lastTime = File.GetLastWriteTime(fullPath);
                    if (lastTime.Date != DateTime.Now.Date)
                    {
                        File.Move(fullPath, Path.Combine(logFileDirectory, string.Format(LogFileName, "_" + lastTime.Date.ToString("yyyyMMdd"))));
                    }
                }

                // add to existing log file or create new
                return new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unable to create log file {0}: {1}", fullPath, ex.Message);
                return null;
            }
        }

        public void Dispose()
        {
            Trace.Flush();
        }
    }
}
