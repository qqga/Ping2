using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ping2
{
    public static class ExceptionExtensions
    {
        public static string GetMsg(this Exception exception) => exception.Message + (exception.InnerException != null ? $"\r\n{GetMsg(exception.InnerException)}" : "") ;
    }
}
