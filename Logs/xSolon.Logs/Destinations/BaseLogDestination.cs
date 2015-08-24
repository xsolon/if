// Last edited by martin: 2014_09_01 8:32 PM
// Created : 2014_08_03

#region imports

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using xSolon.Events;
using xSolon.Logs;

#endregion

namespace xSolon.Instructions.DTO
{
    public abstract class BaseLogDestination : ILogDestination, ICloneable
    {
        public bool AutoCommit = false;
        public int Level = (int)EventSeverity.Verbose;
        public List<EventEntry> LogList = new List<EventEntry>();

        [XmlIgnore]
        public LoggerClass Logger = null;
        public abstract object Clone();

        /// <summary>
        ///     Checks if the Level will be logged by the destination. Allows checking before a message is processed
        /// </summary>
        /// <param name="level">The level to be checked.</param>
        /// <returns>true|false</returns>
        public virtual bool ShouldLog(int level)
        {
            return ShouldLog(new EventEntry(string.Empty, string.Empty, level));
        }

        /// <summary>
        ///     Checks if the entry will be logged by the destination using the entry Level
        /// </summary>
        /// <param name="EventEntry"></param>
        /// <returns>true|false</returns>
        public virtual bool ShouldLog(EventEntry EventEntry)
        {
            return ShouldLog(EventEntry, Level);
        }

        public virtual void Log(EventEntry entry)
        {
            try
            {
                if (entry == null)
                {
                    return;
                }

                if (LogList == null) LogList = new List<EventEntry>();
                
                OnNotifyEvent(entry);

                LogList.Add(entry);

                if (AutoCommit)
                {
                    Commit();

                    LogList.Clear();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        public event BubbleUpLogEvent NotifyEvent;

        protected virtual void OnNotifyEvent(EventEntry entry)
        {
            BubbleUpLogEvent handler = NotifyEvent;
            if (handler != null) handler(entry);
        }

        public abstract void Commit();

        public List<EventEntry> GetEntries()
        {
            return LogList;
        }

        public static bool ShouldLog(EventEntry EventEntry, int Level)
        {
            if ((int)Level <= (int)EventEntry.Severity)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Creates a string from all events captured
        /// </summary>
        /// <returns></returns>
        protected virtual string GetMessage(List<EventEntry> Entries)
        {
            if (Entries.Count == 0)
            {
                return string.Empty;
            }

            bool addEndLine = false;

            string LogMessage = string.Empty;

            if (Level != (int)EventSeverity.None)
            {
                string endLine =
                    string.Format("------------------------------------------------------------------------{0}",
                        Environment.NewLine);

                foreach (EventEntry EventEntry in Entries)
                {
                    #region Filter Log

                    //High = 20,                    High
                    //Medium = 50,                  Medium
                    //Monitorable = 15,             Unexpected
                    //Unexpected = 10,              Monitorable
                    //Verbose = 100,                Verbose
                    //None = 200 //MJM 
                    bool log = ShouldLog(EventEntry);

                    #endregion

                    if (log)
                    {
                        LogMessage += GetMessage(EventEntry);

                        if (addEndLine)
                        {
                            LogMessage += endLine;
                        }
                    }
                }

                LogMessage += endLine;
            }

            return LogMessage;
        }

        /// <summary>
        ///     Get formatted message from a  eventEntry
        /// </summary>
        /// <param name=" eventEntry"> eventEntry to be processed</param>
        /// <returns>Formatted  eventEntry string</returns>
        /// <example></example>
        public static string GetMessage(EventEntry  eventEntry)
        {
            string template = "\n\t{0,-15}\t{1,-15}\t{2, 15}\t{3}\t{4}\n";

            if (string.Empty ==  eventEntry.Message &&  eventEntry.GetMessage != null)
            {
                try
                {
                     eventEntry.Message =  eventEntry.GetMessage();
                }
                catch (Exception ex)
                {
                     eventEntry.Message = ex.ToString();
                }
            }

            var result = string.Format(template,  eventEntry.Time,  eventEntry.Severity,  eventEntry.Method,  eventEntry.Message,
                 eventEntry.LoggedBy);

            if ( eventEntry.Props != null &&  eventEntry.Props.Count > 0)
            {
                var propTemplate = "\t{0}: \t{1}\n";

                var enumer =  eventEntry.Props.GetEnumerator();

                while (enumer.MoveNext())
                {
                    try
                    {
                        var i = enumer.Current;

                        ///TODO: RawPrint
                        result += string.Format(propTemplate, i.Name,
                            (i.Value ?? "").ToString()
                            //.RawPrint()
                                .Replace("<?xml version=\"1.0\"?>", "")
                                .Replace("\n", "")
                                .Replace("\t", "")
                                .Replace("\r", "")
                                .Trim());
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                }
            }

            return result;
        }

        //public  virtual int CalculateEffectiveLevel(List<EventEntry> entries)

        //{

        //    var maxLevel = (int)(int)EventSeverity.None;

        //    entries.ForEach(i =>

        //        {

        //            var entryLevel = (int)i.Level;

        //            //if (entryLevel < maxLevel)

        //                //maxLevel = entryLevel

        //        });

        //}
    }
}