using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using xSolon.Events;

namespace MyEvents
{
    class Program
    {
        static void Main(string[] args)
        {

            var bll = new MyDriver();

            bll.NotifyEvent +=(e) =>{
                Trace.WriteLine(e.Message);
            };

            bll.Run();

        }
    }
}
