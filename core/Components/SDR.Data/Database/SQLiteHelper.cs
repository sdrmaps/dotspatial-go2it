using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace SDR.Data.Database
{
    /// <summary>
    /// This class contains methods for working with the
    /// SQLite database
    /// </summary>
    public static class SQLiteHelper
    {
        /// <summary>
        /// To get the full SQLite connection string given the SQLite database path, and validates connection
        /// </summary>
        public static string GetSQLiteConnectionString(string dbFileName)
        {
            if (String.IsNullOrEmpty(dbFileName))
                throw new ArgumentException("SQLite file path is null or empty.", dbFileName);

            var conn = new SQLiteConnectionStringBuilder { DataSource = dbFileName, Version = 3, FailIfMissing = true };
            conn.Add("Compress", true);

            using (var cnn = new SQLiteConnection(conn.ConnectionString))
            {
                cnn.Open();
                return cnn.ConnectionString;
            }
        }

        /// <summary>
        /// To get the SQLite database path given the SQLite connection string
        /// </summary>
        public static string GetSQLiteFileName(string sqliteConnString)
        {
            if (String.IsNullOrEmpty(sqliteConnString))
                throw new ArgumentException("Connection string is null or empty.", sqliteConnString);

            var conn = new SQLiteConnectionStringBuilder(sqliteConnString);
            return conn.DataSource;
        }

        /// <summary>
        /// Check if the path is the location of a valid SQLite database
        /// This function returns false, if the SQLite db
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
        /// Interact with the database for purposes other than a query.
        /// </summary>
        public static int ExecuteNonQuery(string conn, string sql)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Connection string is null or empty.", conn);

            if (String.IsNullOrEmpty(sql))
                throw new ArgumentException("SQL command is null or empty.", sql);

            using (var cnn = new SQLiteConnection(conn))
            {
                cnn.Open();
                var command = new SQLiteCommand(cnn) { CommandText = sql };
                var rowsUpdated = command.ExecuteNonQuery();
                return rowsUpdated;
            }
        }

        /// <summary>
        /// run a query against the Database.
        /// </summary>
        public static string[] GetAllTableNames(string conn)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Connection string is null or empty.", conn);

            var list = new List<string>();
            const string sql = "SELECT name FROM sqlite_master WHERE type='table';";
            using (var cnn = new SQLiteConnection(conn))
            {
                cnn.Open();
                var command = new SQLiteCommand(cnn) { CommandText = sql };
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader.GetString(0));
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// run a query against the Database.
        /// </summary>
        public static string[] GetResultsAsArray(string conn, string sql)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Connection string is null or empty.", conn);

            if (String.IsNullOrEmpty(sql))
                throw new ArgumentException("SQL command is null or empty.", sql);

            var list = new List<string>();
            using (var cnn = new SQLiteConnection(conn))
            {
                cnn.Open();
                var command = new SQLiteCommand(cnn) { CommandText = sql };
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader.GetString(0));
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// run a query against the Database.
        /// </summary>
        public static DataTable GetDataTable(string conn, string sql)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Connection string is null or empty.", conn);

            if (String.IsNullOrEmpty(sql))
                throw new ArgumentException("SQL command is null or empty.", sql);

            var dt = new DataTable();
            using (var cnn = new SQLiteConnection(conn))
            {
                cnn.Open();
                var command = new SQLiteCommand(cnn) { CommandText = sql };
                var reader = command.ExecuteReader();
                dt.Load(reader);
            }
            return dt;
        }

        /// <summary>
        /// retrieve single items from the DB.
        /// </summary> 
        public static string ExecuteScalar(string conn, string sql)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Connection string is null or empty.", conn);

            if (String.IsNullOrEmpty(sql))
                throw new ArgumentException("SQL command is null or empty.", sql);

            using (var cnn = new SQLiteConnection(conn))
            {
                cnn.Open();
                var command = new SQLiteCommand(cnn) { CommandText = sql };
                object value = command.ExecuteScalar();
                return value != null ? value.ToString() : "";
            }
        }

        /// <summary>
        /// update rows in the DB.
        ///
        /// The table to update.
        /// A dictionary containing Column names and their new values.
        /// The where clause for the update statement.
        /// A boolean true or false to signify success or failure.
        ///  </summary>
        public static int Update(string conn, String tableName, Dictionary<string,string> data, String where)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Connection string is null or empty.", conn);

            if (String.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name is null or empty.", tableName);

            if (String.IsNullOrEmpty(where))
                throw new ArgumentException("WHERE  clause is null or empty.", where);

            if (data == null)
                throw new ArgumentException("Data dictionary is null.");

            if (data.Count == 0)
                throw new ArgumentException("Data dictionary is empty.", data.ToString());

            var vals = "";
            if (data.Count >= 1)
            {
                vals = data.Aggregate(vals, (current, val) => current + String.Format(" {0} = '{1}',", val.Key, val.Value));
                vals = vals.Substring(0, vals.Length - 1);
            }
            return ExecuteNonQuery(conn, String.Format("update {0} set {1} where {2};", tableName, vals, where));
        }

        /// delete rows from the DB.
        ///
        /// The table from which to delete.
        /// The where clause for the delete.
        /// A boolean true or false to signify success or failure.
        public static int Delete(string conn, String tableName, String where)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Connection string is null or empty.", conn);

            if (String.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name is null or empty.", tableName);

            if (String.IsNullOrEmpty(where))
                throw new ArgumentException("WHERE  clause is null or empty.", where);

            return ExecuteNonQuery(conn, String.Format("delete from {0} where {1};", tableName, where));
        }

        ///
        /// insert into the DB
        ///
        /// The table into which we insert the data.
        /// A dictionary containing the column names and data for the insert.
        /// A boolean true or false to signify success or failure.
        public static int Insert(string conn, String tableName, Dictionary<string, string> data)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Connection string is null or empty.", conn);

            if (String.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name is null or empty.", tableName);

            if (data == null)
                throw new ArgumentException("Data dictionary is null.");

            if (data.Count == 0)
                throw new ArgumentException("Data dictionary is empty.", data.ToString());

            String columns = "";
            String values = "";
            foreach (var val in data)
            {
                columns += String.Format(" {0},", val.Key);
                values += String.Format(" '{0}',", val.Value);
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            return ExecuteNonQuery(conn, String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
        }

        ///
        /// delete all data from the DB.
        ///
        /// A boolean true or false to signify success or failure.
        public static void ClearDb(string conn)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Connection string is null or empty.", conn);

            DataTable tables = GetDataTable(conn, "select NAME from SQLITE_MASTER where type='table' order by NAME;");
            foreach (DataRow table in tables.Rows)
            {
                ClearTable(conn, table["NAME"].ToString());
            }
            tables.Dispose();
        }

        ///
        /// clear all data from a specific table.
        ///
        /// The name of the table to clear.
        /// A boolean true or false to signify success or failure.
        public static int ClearTable(string conn, String table)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Connection string is null or empty.", conn);

            if (String.IsNullOrEmpty(table))
                throw new ArgumentException("Table name is null or empty.", table);

            return ExecuteNonQuery(conn, String.Format("delete from {0};", table));
        }

        public static int DropTable(string conn, string table)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Connection string is null or empty.", conn);

            if (String.IsNullOrEmpty(table))
                throw new ArgumentException("Table name is null or empty.", table);

            return ExecuteNonQuery(conn, String.Format("drop table {0};", table));
        }

        public static int CreateTable(string conn, string table, Dictionary<string, string> fields)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Connection string is null or empty.", conn);

            if (String.IsNullOrEmpty(table))
                throw new ArgumentException("Table name is null or empty.", table);

            if (fields == null)
                throw new ArgumentException("Data dictionary is null.");

            if (fields.Count == 0)
                throw new ArgumentException("Data dictionary is empty.", fields.ToString());

            var sql = fields.Aggregate(string.Empty, (current, keyValuePair) => current + keyValuePair.Key + " " + keyValuePair.Value + ",");
            sql = sql.Remove(sql.Length - 1);
            return ExecuteNonQuery(conn, String.Format("create table {0} (" + sql + ")", table));
        }

        public static bool TableExists(string conn, string table)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Connection string is null or empty.", conn);

            if (String.IsNullOrEmpty(table))
                throw new ArgumentException("Table name is null or empty.", table);

            using (var cnn = new SQLiteConnection(conn))
            {
                cnn.Open();
                var rows = cnn.GetSchema("Tables").Select(string.Format("Table_Name = '{0}'", table));
                var tableExists = (rows.Length > 0);
                return tableExists;
            }
        }

        public static bool ColumnExists(string conn, string table, string column)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Connection string is null or empty.", conn);

            if (String.IsNullOrEmpty(table))
                throw new ArgumentException("Table name is null or empty.", table);

            if (String.IsNullOrEmpty(column))
                throw new ArgumentException("Column name is null or empty.", column);

            using (var cnn = new SQLiteConnection(conn))
            {
                cnn.Open();
                // get db schema for columns
                DataTable columns = cnn.GetSchema("Columns");
                bool exists = columns.Select("COLUMN_NAME='" + column + "' AND TABLE_NAME='" + table + "'").Length != 0;
                return exists;
            }
        }

        public static int CreateColumn(string conn, string table, string columnName, string columnType)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Connection string is null or empty.", conn);

            if (String.IsNullOrEmpty(table))
                throw new ArgumentException("Table name is null or empty.", table);

            if (String.IsNullOrEmpty(columnName))
                throw new ArgumentException("Column name is null or empty.", columnName);

            if (String.IsNullOrEmpty(columnType))
                throw new ArgumentException("Column type is null or empty.", columnType);

            var sql = "ALTER TABLE " + table + " ADD COLUMN " + columnName + " " + columnType;
            return ExecuteNonQuery(conn, sql);
        }
    }
}
