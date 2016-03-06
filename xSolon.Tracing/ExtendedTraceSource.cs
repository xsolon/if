using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace xSolon.Tracing
{
    public class ExtendedTraceSource : TraceSource
    {
        public void Indent()
        {

            foreach (TraceListener listener in this.Listeners)
            {
                listener.IndentLevel++;
            }
        }

        public void UnIndent()
        {

            foreach (TraceListener listener in this.Listeners)
            {
                listener.IndentLevel--;
            }

        }

        #region ctor
        public ExtendedTraceSource(string sourceName) : base(sourceName)
        {

        }

        public ExtendedTraceSource() : base("xSolon")
        {

        }

        #endregion

    }

}
