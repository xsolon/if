// MyLogs Program.cs  8/24/2015 martin 

using System;
using System.Linq;
using MyEvents;
using xSolon.Instructions.DTO.Logs;
using xSolon.Logs;

namespace MyLogs
{
    internal class Program
    {

        private static void Main(string[] args)
        {

            using (var logger = new LoggerClass("MyLogs", new TraceDestination() { AutoCommit = true, WriteConsole = true }))
            {

                var bll = new MyDriver(logger);

                bll.Run();
            }

        }
    }
}
