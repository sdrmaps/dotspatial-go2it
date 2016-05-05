﻿// originally based on code lifted from hydrodesktop application
using System;
using System.Collections.Generic;
using System.IO;

namespace SDR.Configuration
{
    /// <summary>
    /// Application level mode propeerty setting enum
    /// </summary>
    public enum AppMode
    {
        Dispatch,
        Responder
    }

    /// <summary>
    /// Application - level settings including web service URLs and database locations
    /// </summary>
    public class Settings
    {
        // app mode lookup for int based value
        private AppMode _appMode; // (responder/dispatch)
        private static readonly List<AppMode> AppModes = new List<AppMode>(new[] { AppMode.Dispatch, AppMode.Responder});

        /// <summary>
        /// Allocate ourselves. We have a private constructor, so no one else can.
        /// </summary>
        static readonly Settings SettingsInstance = new Settings();

        /// <summary>
        /// Access SiteStructure.Instance to get the singleton object.
        /// Then call methods on that instance.
        /// </summary>
        public static Settings Instance
        {
            get { return SettingsInstance; }
        }

        /// <summary>
        /// Creates a new settings object with default values.
        /// This is a private constructor, meaning no outsiders have access.
        /// </summary>
        private Settings()
        {
        }

        /// <summary>
        /// Gets the Current Application name
        /// </summary>
        public string ApplicationName
        {
            get;
            set;
        }
        
        public void SetApplicationMode(int mode)
        {
            _appMode = AppModes[mode];
        }

        /// <summary>   
        /// Gets the Active Application mode
        /// </summary>
        public AppMode ApplicationMode
        {
            get { return _appMode; }
        }

        /// <summary>
        /// The application-defined app settings repository connection string
        /// </summary>
        public string ApplicationRepoConnectionString
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets the application data directory
        /// </summary>
        public string ApplicationDataDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Current user data directory (the full path is returned)
        /// </summary>
        public string CurrentUserDataDirectory
        {
            get { return Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)).FullName; }
        }

        /// <summary>
        /// Gets the list of recent project files
        /// </summary>
        public System.Collections.Specialized.StringCollection RecentProjectFiles
        {
            get { return Properties.AppSettings.Default.RecentProjectFiles; }
        }

        /// <summary>
        /// Add a project file name to the list of recent project files
        /// </summary>
        /// <param name="fileName"></param>
        public void AddFileToRecentFiles(string fileName)
        {
            const int maximumNumberOfRecentFiles = 10;

            if (Properties.AppSettings.Default.RecentProjectFiles.Contains(fileName))
            {
                Properties.AppSettings.Default.RecentProjectFiles.Remove(fileName);
            }

            if (Properties.AppSettings.Default.RecentProjectFiles.Count >= maximumNumberOfRecentFiles)
            {
                Properties.AppSettings.Default.RecentProjectFiles.RemoveAt(
                    Properties.AppSettings.Default.RecentProjectFiles.Count - 1);
            }

            // insert value at the top of the list
            Properties.AppSettings.Default.RecentProjectFiles.Insert(0, fileName);
            //save settings
            Properties.AppSettings.Default.Save();
        }

        #region Events

        ///// <summary>
        ///// This event occurs when the main database connection string has changed
        ///// </summary>
        //public event EventHandler DatabaseChanged;

        //private void OnDatabaseChanged()
        //{
        //    var handler = DatabaseChanged;
        //    if (handler != null)
        //        handler(this, EventArgs.Empty);
        //}

        #endregion Events
    }
}
