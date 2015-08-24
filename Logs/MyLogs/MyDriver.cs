using System;
using xSolon.Events;

namespace MyEvents
{
    class MyDriver : LoggedClass
    {

        public MyDriver(LoggedClass parent) : base(parent)
        {

        }

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