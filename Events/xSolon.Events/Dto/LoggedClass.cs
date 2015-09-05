// Last edited by martin: 2014_08_29 11:35 AM
// Created : 2014_08_03

#region imports

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using xSolon.Events;
using xSolon.Events.Dto;

#endregion

namespace xSolon.Events
{
    /// <summary>
    ///     Abstract class implements notification methods and events to communicate activity to consumers
    /// </summary>
    public class LoggedClass : MarshalByRefObject, ILog
    {
        private static readonly Random rnd = new Random(DateTime.Now.Millisecond);

        /// <summary>
        ///     Identifier of the class. Used for tracking log stack.
        /// </summary>
        public string InstanceId = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public LoggedClass()
        {
            InstanceId = String.Format("{0}_{1}", GetType().Name, rnd.Next(99));
        }

        public LoggedClass(LoggedClass parent)
        {
            if (parent != null)
            {
                NotifyEvent += parent.NotifyGeneric;
            }

            InstanceId = String.Format("{0}_{1}", GetType().Name, rnd.Next(99));
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {

        }

        /// <summary>
        ///     Event to notify subscribers of events
        /// </summary>
        public event BubbleUpLogEvent NotifyEvent;

        /// <summary>
        ///     This method returns the name of the procedure that called one of the members of the LoggedClass interface
        ///     it should not be
        /// </summary>
        /// <returns>name of the procedure that called the loggin interface</returns>
        internal static string GetCallerMethodName()
        {
            // get call stack
            var stack = new StackTrace();

            string method = stack.GetFrame(2).GetMethod().Name;

            try
            {
                if (method.Contains("CreateEventEntry") && stack.FrameCount > 2)
                {
                    method = stack.GetFrame(3).GetMethod().Name;
                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex.Message);
            }

            try
            {
                if (method.Contains("Log") && stack.FrameCount > 3)
                {
                    method = stack.GetFrame(4).GetMethod().Name;
                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex.Message);
            }

            return method;
        }

        //public LoggedClass(LoggedClass logger)

        //{

        //    InstanceId = String.Format("{0}_{1}", GetType().Name, rnd.Next(99));

        //    if (logger != null)

        //        this.NotifyEvent += logger.NotifyGeneric;

        //}

        public event ProgressChangedEventHandler ProgressChanged;

        public EventEntry CreateEventEntry()
        {
            var entry = new EventEntry(GetCallerMethodName(), string.Empty, (int)EventSeverity.Verbose);

            entry.LoggedBy = InstanceId;

            return entry;
        }

        public EventEntry CreateEventEntry(string caller, int severity, GetMessageDelegate messageCallback)
        {
            var entry = CreateEventEntry(caller, severity, string.Empty);

            entry.GetMessage = messageCallback;

            return entry;
        }

        public EventEntry CreateEventEntry(string caller, int severity, string message)
        {
            var entry = new EventEntry(caller, message, severity);

            entry.LoggedBy = InstanceId;

            return entry;
        }

        public EventEntry CreateEventEntry(int severity, string message, Dictionary<string, object> props)
        {
            var entry = CreateEventEntry(GetCallerMethodName(), severity, message);

            if (props != null)
            {
                entry.Props = new XSolonDictionary(props as IDictionary<string, object>);
            }

            return entry;
        }

        public EventEntry CreateEventEntry(string caller, int severity, string message, IDictionary props)
        {
            var entry = CreateEventEntry(caller, severity, message);

            if (props != null)
            {
                var col = props;

                var enumer = col.GetEnumerator();

                var dic = new Dictionary<string, object>();

                while (enumer.MoveNext())
                {
                    try
                    {
                        var key = enumer.Key.ToString();

                        if (!dic.ContainsKey(key))
                        {
                            dic.Add(key, enumer.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                }

                entry.Props = new XSolonDictionary(dic as IDictionary<string, object>);
            }

            return entry;
        }

        public EventEntry CreateEventEntry(string caller, int severity, string message,
            Dictionary<string, object> props)
        {
            var entry = CreateEventEntry(caller, severity, message);

            if (props != null)
            {
                entry.Props = new XSolonDictionary(props as IDictionary<string, object>);
            }

            return entry;
        }

        #region LoggedClass Members

        public void Notify(GetMessageDelegate messageCallback, int level)
        {
            EventEntry entry = CreateEventEntry(GetCallerMethodName(), level, messageCallback);
            NotifyGeneric(entry);
        }

        public void Notify(int level, Func<string> messageCallback)
        {
            var del = new GetMessageDelegate(messageCallback);

            EventEntry entry = CreateEventEntry(GetCallerMethodName(), level, del);
            NotifyGeneric(entry);
        }

        public void NotifyException(Dictionary<string, object> props, Exception ex)
        {
            //if (NotifyEvent != null)
            {
                string message = string.Empty;

                Exception innerEx = ex;

                message = ex.ToString();

                EventEntry entry = CreateEventEntry(GetCallerMethodName(), (int)EventSeverity.Error, message, props);
                NotifyGeneric(entry);
            }
        }

        public virtual void NotifyInformation(Dictionary<string, object> props, string message)
        {
            //if (NotifyEvent != null)
            {
                EventEntry entry = CreateEventEntry(GetCallerMethodName(), (int)EventSeverity.Information, message, props);
                NotifyGeneric(entry);
            }
        }

        public virtual void NotifyInformation(Dictionary<string, object> props, string message, params object[] args)
        {
            var mssg = string.Format(message, args);

            EventEntry entry = CreateEventEntry(GetCallerMethodName(), (int)EventSeverity.Information, mssg, props);
            NotifyGeneric(entry);
        }

        public virtual void NotifyVerbose(Dictionary<string, object> props, string message)
        {
            EventEntry entry = CreateEventEntry(GetCallerMethodName(), (int)EventSeverity.Verbose, message, props);
            NotifyGeneric(entry);
        }

        public virtual void NotifyVerbose(Dictionary<string, object> props, string message, params object[] args)
        {
            var mssg = string.Format(message, args);

            EventEntry entry = CreateEventEntry(GetCallerMethodName(), (int)EventSeverity.Verbose, mssg, props);
            NotifyGeneric(entry);
        }

        public virtual void NotifyWarning(Dictionary<string, object> props, string message, params object[] args)
        {
            var mssg = string.Format(message, args);

            EventEntry entry = CreateEventEntry(GetCallerMethodName(), (int)EventSeverity.Warning, mssg, props);
            NotifyGeneric(entry);
        }

        public virtual void NotifyWarning(Dictionary<string, object> props, string message)
        {
            EventEntry entry = CreateEventEntry(GetCallerMethodName(), (int)EventSeverity.Warning, message, props);
            NotifyGeneric(entry);
        }

        public virtual void NotifyError(Dictionary<string, object> props, string message)
        {
            //if (NotifyEvent != null)
            {
                EventEntry entry = CreateEventEntry(GetCallerMethodName(), (int)EventSeverity.Error, message, props);
                NotifyGeneric(entry);
            }
        }

        public virtual void NotifyError(Dictionary<string, object> props, string message, params object[] args)
        {
            var mssg = string.Format(message, args);

            //if (NotifyEvent != null)

            {
                EventEntry entry = CreateEventEntry(GetCallerMethodName(), (int)EventSeverity.Error, mssg, props);
                NotifyGeneric(entry);
            }
        }

        public virtual void NotifyInformation(string message)
        {
            //if (NotifyEvent != null)
            {
                string name = GetCallerMethodName();
                EventEntry entry = CreateEventEntry(name, (int)EventSeverity.Information, message);
                NotifyGeneric(entry);
            }
        }

        public virtual void NotifyInformation(string message, params object[] args)
        {
            //if (NotifyEvent != null)
            {
                var mssg = string.Format(message, args);

                EventEntry entry = CreateEventEntry(GetCallerMethodName(), (int)EventSeverity.Information, mssg);
                NotifyGeneric(entry);
            }
        }

        /// <summary>
        ///     Notify properties to subscribers
        /// </summary>
        /// <param name="props">Properties to be logged</param>
        public void NotifyVerbose(Dictionary<string, object> props)
        {
            //if (NotifyEvent != null)
            {
                var log = CreateEventEntry((int)EventSeverity.Verbose, string.Empty, props);

                NotifyGeneric(log);
            }
        }

        public virtual void NotifyVerbose(string message)
        {
            //if (NotifyEvent != null)
            {
                EventEntry entry = CreateEventEntry(GetCallerMethodName(), (int)EventSeverity.Verbose, message);
                NotifyGeneric(entry);
            }
        }

        public virtual void NotifyVerbose(string message, params object[] args)
        {
            var mssg = string.Format(message, args);

            //if (NotifyEvent != null)

            {
                EventEntry entry = CreateEventEntry(GetCallerMethodName(), (int)EventSeverity.Verbose, mssg);
                NotifyGeneric(entry);
            }
        }

        public virtual void NotifyWarning(string message, params object[] args)
        {
            var mssg = string.Format(message, args);

            //if (NotifyEvent != null)

            {
                EventEntry entry = CreateEventEntry(GetCallerMethodName(), (int)EventSeverity.Warning, mssg);
                NotifyGeneric(entry);
            }
        }

        public virtual void NotifyWarning(string message)
        {
            //if (NotifyEvent != null)
            {
                EventEntry entry = CreateEventEntry(GetCallerMethodName(), (int)EventSeverity.Warning, message);
                NotifyGeneric(entry);
            }
        }

        public virtual void NotifyError(string message)
        {
            //if (NotifyEvent != null)
            {
                EventEntry entry = CreateEventEntry(GetCallerMethodName(), (int)EventSeverity.Error, message);
                NotifyGeneric(entry);
            }
        }

        public virtual void NotifyError(string message, params object[] args)
        {
            var mssg = string.Format(message, args);

            //if (NotifyEvent != null)

            {
                EventEntry entry = CreateEventEntry(GetCallerMethodName(), (int)EventSeverity.Error, mssg);
                NotifyGeneric(entry);
            }
        }

        public virtual void NotifyGeneric(EventEntry entry)
        {
            if (entry.LoggedBy == null)
            {
                entry.LoggedBy = InstanceId;
            }

            if (!entry.LoggedBy.Contains(InstanceId))
            {
                entry.LoggedBy = string.Format("{0}-{1}", InstanceId, entry.LoggedBy);
            }

#if DEBUG

            //Debug.WriteLine(string.Format("{0}\t{1}\t{2}", entry.Time, entry.Level.ToString(), entry.Message)); 

#endif

            if (NotifyEvent != null)
            {
                NotifyEvent(entry);
            }
        }

        public virtual void NotifyException(Exception ex)
        {
            string message = string.Empty;

            Exception innerEx = ex;

            message = ex.ToString();

            //if (NotifyEvent != null)

            {
                EventEntry entry = CreateEventEntry(GetCallerMethodName(), (int)EventSeverity.Error, message);
                NotifyGeneric(entry);
            }
        }

        public void Notify(int level, Func<EventEntry> messageCallback)
        {
            var entry = messageCallback();
            entry.Severity = level;

            NotifyGeneric(entry);
        }

        public void Notify(IDictionary props, int level, string message)
        {
            var EventEntry = CreateEventEntry(GetCallerMethodName(), level, message, props);

            NotifyGeneric(EventEntry);
        }

        public virtual void ReportProgress(int percentage, object state)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, new ProgressChangedEventArgs(percentage, state));
            }
        }

        #endregion
    }
}