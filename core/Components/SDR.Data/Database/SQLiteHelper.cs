using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SDR.Data.Database
{
    /// <summary>
    /// This class contains methods for working with the
    /// SQLite database
    /// </summary>
    public static class SQLiteHelper
    {
        /// <summary>
        /// Interact with the database for purposes other than a query.
        /// </summary>
        public static int ExecuteNonQuery(string conn, string sql)
        {
            var cnn = new SQLiteConnection(conn);
            cnn.Open();
            var mycommand = new SQLiteCommand(cnn) {CommandText = sql};
            int rowsUpdated = mycommand.ExecuteNonQuery();
            cnn.Close();
            return rowsUpdated;
        }

        /// <summary>
        /// run a query against the Database.
        /// </summary>
        public static string[] GetAllTableNames(string conn)
        {
            var list = new List<string>();
            try
            {
                const string sql = "SELECT name FROM sqlite_master WHERE type='table';";
                var cnn = new SQLiteConnection(conn);
                cnn.Open();
                var mycommand = new SQLiteCommand(cnn) { CommandText = sql };
                var reader = mycommand.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader.GetString(0));
                }
                reader.Close();
                cnn.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return list.ToArray();
        }

        /// <summary>
        /// run a query against the Database.
        /// </summary>
        public static string[] GetResultsAsArray(string conn, string sql)
        {
            var list = new List<string>();
            try
            {
                var cnn = new SQLiteConnection(conn);
                cnn.Open();
                var mycommand = new SQLiteCommand(cnn) { CommandText = sql };
                var reader = mycommand.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader.GetString(0));
                }
                reader.Close();
                cnn.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return list.ToArray();
        }

        /// <summary>
        /// run a query against the Database.
        /// </summary>
        public static DataTable GetDataTable(string conn, string sql)
        {
            var dt = new DataTable();
            try
            {
                var cnn = new SQLiteConnection(conn);
                cnn.Open();
                var mycommand = new SQLiteCommand(cnn) {CommandText = sql};
                var reader = mycommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                cnn.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dt;
        }

        /// <summary>
        /// retrieve single items from the DB.
        /// </summary> 
        public static string ExecuteScalar(string conn, string sql)
        {
            var cnn = new SQLiteConnection(conn);
            cnn.Open();
            var mycommand = new SQLiteCommand(cnn) {CommandText = sql};
            object value = mycommand.ExecuteScalar();
            cnn.Close();
            if (value != null)
            {
                return value.ToString();
            }
            return "";
        }

        /// <summary>
        /// update rows in the DB.
        ///
        /// The table to update.
        /// A dictionary containing Column names and their new values.
        /// The where clause for the update statement.
        /// A boolean true or false to signify success or failure.
        ///  </summary>
        public static bool Update(string conn, String tableName, Dictionary<string,string> data, String where)
        {
            var vals = "";
            var returnCode = true;
            if (data.Count >= 1)
            {
                vals = data.Aggregate(vals, (current, val) => current + String.Format(" {0} = '{1}',", val.Key, val.Value));
                vals = vals.Substring(0, vals.Length - 1);
            }
            try
            {
                ExecuteNonQuery(conn, String.Format("update {0} set {1} where {2};", tableName, vals, where));
            }
            catch
            {
                returnCode = false;
            }
            return returnCode;
        }

        /// delete rows from the DB.
        ///
        /// The table from which to delete.
        /// The where clause for the delete.
        /// A boolean true or false to signify success or failure.
        public static bool Delete(string conn, String tableName, String where)
        {
            Boolean returnCode = true;
            try
            {
                ExecuteNonQuery(conn, String.Format("delete from {0} where {1};", tableName, where));
            }
            catch (Exception)
            {
                returnCode = false;
            }
            return returnCode;
        }

        ///
        /// insert into the DB
        ///
        /// The table into which we insert the data.
        /// A dictionary containing the column names and data for the insert.
        /// A boolean true or false to signify success or failure.
        public static bool Insert(string conn, String tableName, Dictionary<string, string> data)
        {
            String columns = "";
            String values = "";
            Boolean returnCode = true;
            foreach (var val in data)
            {
                columns += String.Format(" {0},", val.Key);
                values += String.Format(" '{0}',", val.Value);
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            try
            {
                ExecuteNonQuery(conn, String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
            }
            catch (Exception)
            {
                returnCode = false;
            }
            return returnCode;
        }

        ///
        /// delete all data from the DB.
        ///
        /// A boolean true or false to signify success or failure.
        public static bool ClearDb(string conn)
        {
            try
            {
                DataTable tables = GetDataTable(conn, "select NAME from SQLITE_MASTER where type='table' order by NAME;");
                foreach (DataRow table in tables.Rows)
                {
                    ClearTable(conn, table["NAME"].ToString());
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        ///
        /// clear all data from a specific table.
        ///
        /// The name of the table to clear.
        /// A boolean true or false to signify success or failure.
        public static bool ClearTable(string conn, String table)
        {
            try
            {
                ExecuteNonQuery(conn, String.Format("delete from {0};", table));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool DropTable(string conn, string table)
        {
            try
            {
                ExecuteNonQuery(conn, String.Format("drop table {0};", table));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CreateTable(string conn, string table, Dictionary<string, string> fields)
        {
            try
            {
                var sql = fields.Aggregate(string.Empty, (current, keyValuePair) => current + keyValuePair.Key + " " + keyValuePair.Value + ",");
                sql = sql.Remove(sql.Length - 1);
                ExecuteNonQuery(conn, String.Format("create table {0} (" + sql + ")", table));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TableExists(string conn, string table)
        {
            try
            {
                var cnn = new SQLiteConnection(conn);
                cnn.Open();
                var rows = cnn.GetSchema("Tables").Select(string.Format("Table_Name = '{0}'", table));
                var tableExists = (rows.Length > 0);
                return tableExists;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// To get the SQLite database path given the SQLite connection string
        /// </summary>
        public static string GetSQLiteFileName(string sqliteConnString)
        {
            if (String.IsNullOrEmpty(sqliteConnString))
                throw new ArgumentException("sqliteConnString is null or empty.", "sqliteConnString");

            var conn = new SQLiteConnectionStringBuilder(sqliteConnString);
            return conn.DataSource;
        }

        /// <summary>
        /// To get the full SQLite connection string given the SQLite database path
        /// </summary>
        public static string GetSQLiteConnectionString(string dbFileName)
        {
            if (String.IsNullOrEmpty(dbFileName))
                throw new ArgumentException("dbFileName is null or empty.", "dbFileName");

            var conn = new SQLiteConnectionStringBuilder { DataSource = dbFileName, Version = 3, FailIfMissing = true };
            conn.Add("Compress", true);

            return conn.ConnectionString;
        }

        /// <summary>
        /// Create the default .SQLITE database in the user-specified path
        /// </summary>
        /// <returns>true if database was created, false otherwise</returns>
        public static Boolean CreateSQLiteDatabase(string dbPath, string dbTemplate)
        {
            if (String.IsNullOrEmpty(dbPath))
            {
                throw new ArgumentException("dbPath is null or empty.", "dbPath");
            }
            var asm = Assembly.GetCallingAssembly();
            //to create the default.sqlite database file
            try
            {
                using (Stream input = asm.GetManifestResourceStream(dbTemplate))
                {
                    using (Stream output = File.Create(dbPath))
                    {
                        CopyStream(input, output);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine("Error creating the default database " + dbPath +
                    ". Please check your write permissions. " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error creating the default database " + dbPath +
                    ". Error details: " + ex.Message);
                return false;
            }
            return File.Exists(dbPath);
        }

        private static void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[8192];

            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }

        /// <summary>
        /// Check if the path is a valid SQLite database
        /// This function returns false, if the SQLite db
        /// file doesn't exist or if the file size is 0 Bytes
        /// </summary>
        public static bool DatabaseExists(string dbPath)
        {
            if (!File.Exists(dbPath))
            {
                return false;
            }
            var dbFileInfo = new FileInfo(dbPath);
            return dbFileInfo.Length != 0;
        }
    }
}
