using System;
using System.Data.SqlClient;

namespace SDR.Data.Database
{
    /// <summary>
    /// This class contains methods for working with the
    /// sql server databases
    /// </summary>
    public static class SqlServerHelper
    {
        /// <summary>
        /// Converts and validates slq server connection strings
        /// </summary>
        public static string GetSqlServerConnectionString(string connString)
        {
            if (String.IsNullOrEmpty(connString))
                throw new ArgumentException("Connection string is null or empty.", connString);

            var conn = new SqlConnectionStringBuilder(connString);
            using (var cnn = new SqlConnection(conn.ConnectionString))
            {
                cnn.Open();
                return cnn.ConnectionString;
            }
        }

        /// <summary>
        /// Check if a named table exists in a database
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static bool TableExists(string conn, string table)
        {
            if (String.IsNullOrEmpty(conn))
                throw new ArgumentException("Connection string is null or empty.", conn);

            if (String.IsNullOrEmpty(table))
                throw new ArgumentException("Table name is null or empty.", table);

            using (var cnn = new SqlConnection(conn))
            {
                cnn.Open();
                var dTable = cnn.GetSchema("TABLES", new[] { null, null, table });
                return dTable.Rows.Count > 0;
            }
        }

        /// <summary>
        /// Check if a StoredProcedure exists in a database
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="proc"></param>
        /// <returns></returns>
        public static bool StoredProcedureExists(string connStr, string proc)
        {
            if (String.IsNullOrEmpty(connStr))
                throw new ArgumentException("Connection string is null or empty.", connStr);

            if (String.IsNullOrEmpty(proc))
                throw new ArgumentException("StoredProcedure name is null or empty.", proc);

            var query = "SELECT * FROM sysobjects WHERE type = 'P' AND name = '" + proc + "'";
            var exists = false;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            exists = true;
                            break;
                        }
                    }
                    
                }
            }
            return exists;
        }
    }
}
