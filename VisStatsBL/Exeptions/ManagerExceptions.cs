using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VisStatsBL.Exeptions
{
    public class ManagerExceptions : Exception
    {
        public ManagerExceptions(string? message) : base(message)
        {
        }

        public ManagerExceptions(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
