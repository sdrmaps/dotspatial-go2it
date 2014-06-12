﻿// code originally lifted from HydroDesktop Application
// modified for use within sdr dot spatial framework

using System;
using System.IO;

namespace SDR.Configuration
{
    /// <summary>
    /// Helper methods for finding and creating directories
    /// </summary>
    public static class ConfigurationHelper
    {
        public static string FindOrCreateAppDataDirectory(string appName)
        {
            string baseAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // check if this directory can be created             
            string theAppData = Path.Combine(baseAppData, appName);
            CheckDirectory(theAppData);
            return theAppData;
        }

        public static string FindOrCreateTempDirectory(string appName)
        {
            string basePath = Path.GetTempPath();
            // check if this directory can be created             
            string theTempDir = Path.Combine(basePath, appName);
            CheckDirectory(theTempDir);
            return theTempDir;
        }

        private static void CheckDirectory(string directoryName)
        {
            if (Directory.Exists(directoryName)) return;
            try
            {
                Directory.CreateDirectory(directoryName);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Error creating directory " +
                                                      directoryName + ". " + ex.Message);
            }
        }
    }
}
