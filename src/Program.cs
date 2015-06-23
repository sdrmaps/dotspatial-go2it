using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SDR.Common;
using SDR.Common.logging;
using SDR.Data.Database;
using SdrConfig = SDR.Configuration;

namespace Go2It
{
    internal static class Program
    {
        /// <summary>
        /// Main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // load application level settings 
            LoadApplicationConfig();

            // set up a logger for the application
            var log = AppContext.Instance.Get<ILog>();
            Application.ApplicationExit += delegate
                {
                    log.Info("Application Exit");
                    AppContext.Instance.Dispose();
                };
            // log all unhandled exceptions
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (sender, e) => ProcessUnhandled(e.Exception, false);
            AppDomain.CurrentDomain.UnhandledException +=
                    delegate(object sender, UnhandledExceptionEventArgs e)
                    {
                        ProcessUnhandled((Exception)e.ExceptionObject, true);
                        AppContext.Instance.Dispose();
                    };

            log.Info("Application Started");

            var mainForm = new MainForm();
            if (args.Length > 0 && File.Exists(args[0]))
            {
                log.Info("Opening Project (Program.cs): " + args[0]);
                // a project is being loaded from command line or double click
                // mainForm.AppManager.SerializationManager.OpenProject(args[0]);
            }
            Application.Run(mainForm);
        }

        private static void LoadApplicationConfig()
        {
            // set the name of the application on startup to the SdrConfig assembly
            SdrConfig.Settings.Instance.ApplicationName = Properties.Resources.AppName;
            string path = Assembly.GetCallingAssembly().Location;
            string dir = Path.GetDirectoryName(path);
            // set the application startup directory
            SdrConfig.Settings.Instance.ApplicationDataDirectory = dir;
            // assign the application SdrConfig settings database now
            string conString = SQLiteHelper.GetSQLiteConnectionString(dir + "\\Resources\\settingsDatabase.sqlite");
            SdrConfig.Settings.Instance.ApplicationRepoConnectionString = conString;
        }

        private static void ProcessUnhandled(Exception ex, bool isFatal)
        {
            var log = AppContext.Instance.Get<ILog>();
            log.Error(isFatal ? "Fatal" : "Unhandled", ex);
        }
    }
}
