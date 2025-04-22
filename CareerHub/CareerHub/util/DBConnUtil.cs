using CareerHub.exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace CareerHub.util
{
    public static class DBConnUtil
    {
        private static readonly string connectionString = "Server=DESKTOP-HNVF699;Database=CareerHub;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException("Failed to connect to the database: " + ex.Message);
            }
        }

        // Optional: expose connection string if needed
        public static string GetConnectionString()
        {
            return connectionString;
        }
    }
}
