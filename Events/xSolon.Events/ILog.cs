﻿// xSolon.Events ILog.cs  8/24/2015 martin 

using System;
using System.Collections.Generic;
using System.Linq;

namespace xSolon.Events
{
    /// <summary>
    /// Alternative way of declaring Notifications methods. Preferred way is to inherit from <see cref="LoggedClass"/> 
    /// </summary>
    public interface ILog : IDisposable
    {
        #region LoggedClass Members

        event BubbleUpLogEvent NotifyEvent;

        void Notify(GetMessageDelegate messageCallback, int level);

        void Notify(int level, Func<string> messageCallback);

        void NotifyInformation(Dictionary<string, object> props, string message);

        void NotifyInformation(Dictionary<string, object> props, string message, params object[] args);

        void NotifyVerbose(Dictionary<string, object> props, string message);

        void NotifyVerbose(Dictionary<string, object> props);

        void NotifyVerbose(Dictionary<string, object> props, string message, params object[] args);

        void NotifyWarning(Dictionary<string, object> props, string message, params object[] args);

        void NotifyWarning(Dictionary<string, object> props, string message);

        void NotifyError(Dictionary<string, object> props, string message);

        void NotifyError(Dictionary<string, object> props, string message, params object[] args);

        void NotifyException(Dictionary<string, object> props, Exception ex);

        //--------------------------------

        void NotifyInformation(string message);
        void NotifyInformation(int eventId, string message);

        void NotifyInformation(string message, params object[] args);
        void NotifyInformation(int eventId, string message, params object[] args);

        void NotifyVerbose(string message);
        void NotifyVerbose(int eventId,string message);

        void NotifyVerbose(string message, params object[] args);
        void NotifyVerbose(int eventId,string message, params object[] args);

        void NotifyWarning(string message, params object[] args);
        void NotifyWarning(int eventId,string message, params object[] args);

        void NotifyWarning(string message);
        void NotifyWarning(int eventId,string message);

        void NotifyError(string message);
        void NotifyError(int eventId,string message);

        void NotifyError(string message, params object[] args);
        void NotifyError(int eventId,string message, params object[] args);

        void NotifyException(Exception ex);
        void NotifyException(int eventId,Exception ex);

        void NotifyGeneric(EventEntry entry);

        #endregion
    }
}