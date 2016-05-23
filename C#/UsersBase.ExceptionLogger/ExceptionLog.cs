using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersBase.ExceptionLogger
{
    public static class ExceptionLog
    {
        public static void LogError(string category, string text, DateTime exceptionTime)
        {
            File.AppendAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorsLog.txt"), new[] { $"{exceptionTime}:[{category}] - {text}" }, Encoding.Unicode);
        }
    }
}
