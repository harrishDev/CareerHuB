using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerHub.exception
{
    public class DatabaseConnectionException : Exception
    {
        public DatabaseConnectionException(string message) : base("DataBase Connection Error") { }
    }
}
