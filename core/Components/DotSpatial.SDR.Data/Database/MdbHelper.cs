﻿using System;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace DotSpatial.SDR.Data.Database
{
    /// <summary>
    /// This class contains methods for working with the
    /// Ms Access databases
    /// </summary>
    public static class MdbHelper
    {
        /// <summary>
        /// To get the full MDB connection string given the MDB database path,  and validates the connection
        /// </summary>
        public static string GetMdbConnectionString(string dbFileName)
        {
            if (String.IsNullOrEmpty(dbFileName))
                throw new ArgumentException("Database filename is null or empty.", dbFileName);

            var conn = new OleDbConnectionStringBuilder
            {
                DataSource = dbFileName,
                Provider = "Microsoft.Jet.Oledb.4.0"
            };

            using (var cnn = new OleDbConnection(conn.ConnectionString))
            {
                cnn.Open();
                return cnn.ConnectionString;
            }
        }

        /// <summary>
        /// Check if the path is a an actual MDB database
        /// This function returns false, if the mdb db
        /// file doesn't exist or if the file size is 0 Bytes
        /// </summary>
        public static bool DatabaseFileExists(string dbPath)
        {
            if (String.IsNullOrEmpty(dbPath))
                throw new ArgumentException("Database path is null or empty.", dbPath);

            if (!File.Exists(dbPath))
            {
                return false;
            }
            var dbFileInfo = new FileInfo(dbPath);
            return dbFileInfo.Length != 0;
        }

        /// <summary>
        /// Check if a table exists within an mdb database
        /// </summary>
        /// <param name="conn">Connection string to database</param>
        /// <param name="table">Table Name to check</param>
        /// <returns></returns>
        public static bool TableExists(string conn, string table)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Database connection string is null or empty.", conn);

            if (String.IsNullOrEmpty(table))
                throw new ArgumentException("Table name is null or empty.", table);

            using (var cnn = new OleDbConnection(conn))
            {
                cnn.Open();
                var exists = cnn.GetSchema("Tables", new[] { null, null, table, "TABLE" }).Rows.Count > 0;
                return exists;
            }
        }

        /// <summary>
        /// Check if a given field exists within a mdb table
        /// </summary>
        /// <param name="conn">Connection string to database</param>
        /// <param name="table">table to check</param>
        /// <param name="field">field to check</param>
        /// <returns></returns>
        public static bool FieldExists(string conn, string table, string field)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Database connection string is null or empty.", conn);

            if (String.IsNullOrEmpty(table))
                throw new ArgumentException("Table name is null or empty.", table);

            if (String.IsNullOrEmpty(field))
                throw new ArgumentException("Field name is null or empty.", field);

            using (var cnn = new OleDbConnection(conn))
            {
                cnn.Open();
                var dbTable = new DataTable();
                var sqlString = "SELECT TOP 1 + FROM " + table;
                var dbAdapter = new OleDbDataAdapter(sqlString, cnn);
                dbAdapter.Fill(dbTable);
                var i = dbTable.Columns.IndexOf(field);
                return i != -1;
            }            
        }
    }
}