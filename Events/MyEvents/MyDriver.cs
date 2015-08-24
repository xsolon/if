using System;
using xSolon.Events;

namespace MyEvents
{
    class MyDriver : LoggedClass
    {
        internal void Run()
        {
            this.NotifyVerbose("Running");

            try
            {
            }
            catch (Exception ex)
            {
                NotifyException(ex);
            }
            finally
            {
                this.NotifyVerbose("Done");
            }
        }
    }
}