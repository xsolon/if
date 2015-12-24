using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace xSolon.Tracing
{
    public class MemTraceListener : TraceListener
    {

        public bool FullPrint = false;
        public override void Write(string message)
        {
            //return;
            message = FormatMessage(message);

            builder.Append(message);
        }

        string FormatMessage(string message)
        {

            if (FullPrint)
            {

            }
            else
            {
                var bits = message.Split(':');

                message = bits[bits.Length -1];
            }

            return message;
        }

        public override void WriteLine(string message)
        {
            //message = FormatMessage(message);
            builder.AppendLine(message);
        }

        StringBuilder builder = new StringBuilder();

        public string GetFullTrace()
        {

            return builder.ToString();
        }

    }
}
