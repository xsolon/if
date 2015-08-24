using System;
using System.Collections.Generic;
using xSolon.Events;

namespace xSolon.Instructions.DTO
{
    public interface ILogDestination
    {

        bool ShouldLog(EventEntry EventEntry);

        bool ShouldLog(int level);

        void Log(EventEntry entry);

        List<EventEntry> GetEntries();

        void Commit();
    }
}