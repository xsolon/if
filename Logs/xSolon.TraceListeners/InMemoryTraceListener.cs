using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace xSolon.TraceListeners
{
    public class InMemoryTraceListener : TraceListener
    {

        protected override string[] GetSupportedAttributes()
        {
            return new[] { "FullPrint" };
        }

        public InMemoryTraceListener()
            : base()
        {
            this.IndentSize = 5;

        }

        public bool? FullPrint = new bool?();

        public override void Write(string message)
        {
            message = FormatMessage(message);

            builder.Append(message);

            
        }

        string FormatMessage(string message)
        {

            if (!FullPrint.HasValue)
            {

                if (Attributes.ContainsKey("FullPrint") && Attributes["FullPrint"] == "true")
                {
                    FullPrint = true;
                }
                else
                    FullPrint = false;

            }
            if (FullPrint == true)
            {
            }
            else
            {
                var bits = message.Split(':');

                message = bits[bits.Length - 1];
            }

            return message;
        }

        public override void WriteLine(string message)
        {
            if (this.NeedIndent)
            {

                string myString = new string(' ', this.IndentSize * this.IndentLevel);
                builder.Append(myString);
            }
            builder.AppendLine(message);
        }

        StringBuilder builder = new StringBuilder();

        public override string GetTransaction()
        {

            return builder.ToString();
        }

    }
}
