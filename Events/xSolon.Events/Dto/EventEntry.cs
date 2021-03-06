﻿// xSolon.Events EventEntry.cs  8/23/2015 martin 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using xSolon.Events.Dto;

namespace xSolon.Events
{
    /// <summary>
    /// Represents a Log Entry. A log entry is part of a collection of log entries handled by <see cref="Logger"/>
    /// </summary>
    /// 
    [DataContract]
    public class EventEntry
    {

        [XmlIgnore()]
        public GetMessageDelegate GetMessage = null;

        public XSolonDictionary Props = new XSolonDictionary();

        public EventEntry()
        {
        }

        public EventEntry(string method, string message, int severity)
        {

            Time = DateTime.Now;
            Method = method;
            Message = message;
            Severity = severity;
            EventType = GetTraceEventType(severity);
        }

        public TraceEventType GetTraceEventType(int severity)
        {

            var res = TraceEventType.Verbose;

            if (severity < 3000)
            {

            }
            else if (severity < 5000)
            {
                res = TraceEventType.Information;
            }
            else if (severity < 7000)
            {
                res = TraceEventType.Warning;
            }
            else if (severity < 9000)
            {
                res = TraceEventType.Warning;
            }

            return res;

        }

        #region Properties

        [DataMember]
        public List<string> Categories = new List<string>();

        [DataMember]
        public string SessionId { get; set; }

        [DataMember]
        public string LoggedBy { get; set; }

        [DataMember]
        public int EventId { get; set; }

        [DataMember]
        public TraceEventType EventType { get; set; }

        [DataMember]
        public int Severity { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Method { get; set; }

        [DataMember]
        public DateTime Time { get; set; }

        #endregion

    }
}