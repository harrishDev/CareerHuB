using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerHub.exception
{
    public class NegativeSalaryException : Exception
    {
        public NegativeSalaryException(string message) : base("Salary Should not be in negative. Give valid amount") { }
    }
}
