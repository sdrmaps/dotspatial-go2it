using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

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
        public static string GetSqlConnectionString(string connString)
        {
           if (String.IsNullOrEmpty(connString))
                throw new ArgumentException("connString is null or empty.", connString);

            var conn = new SqlConnectionStringBuilder(connString);
            return conn.ConnectionString;
        }
    }
}
