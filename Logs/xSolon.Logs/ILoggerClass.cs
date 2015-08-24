using System;
using System.Collections.Generic;
using System.Linq;
using xSolon.Events;

namespace xSolon.Instructions.DTO
{
    public interface ILoggerClass : ILog
    {
        event EventHandler<ILoggerOnCommitArgs> OnCommit;

        EventEntry CreateEventEntry(string caller, int  severity, string message);

        EventEntry CreateEventEntry();

        EventEntry CreateEventEntry(string caller, int  severity, string message, Dictionary<string, object> props);

        EventEntry CreateEventEntry(int  severity, string message, Dictionary<string, object> props);

        void CommitLog();

    }

    public class ILoggerOnCommitArgs : EventArgs
    {
        public ILoggerClass Logger;
        public List<BaseLogDestination> Destinations;
    }
}
