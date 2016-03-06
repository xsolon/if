﻿using System;
using System.Diagnostics;
using System.Xml;

namespace xSolon.Tracing
{
    /// <summary>
    /// Sets the correlation manager ActivityId for the life of the object,
    /// resetting it when disposed, and optionally logging activity messages.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The sequence of events follows the convention used in WCF logging.
    /// </para>
    /// <para>
    /// When created, the object logs a Transfer event, changes the ActivityId,
    /// and then logs a Start event.
    /// </para>
    /// <para>
    /// When disposed, the object logs a Transfer event (back to the original),
    /// a Stop event, and then changes the ActivityId (back to the original).
    /// </para>
    /// </remarks>
    public class ActivityScope : IDisposable
    {
        // NOTE: Seems overly gratuitous logging, i.e. little distinction 
        // between transfer/start, and transfer/stop)

        // Principles:
        // 1. Need ActivityId to correlate at all, so outer wrapper must be new activity.
        // 2. ActivityId transfer also allows asynchronous actions to also be correlated
        //    i.e. where there is not necessarily a transfer back, such as separate threads.
        // 3. For simple stack operations, i.e. method calls, the LogicalOperationStack may be simpler.

        // Think about activity, with logical op stack inside, and then a transfer...?
        // Activities are kind of like [logical] threads vs logical op stack like the call stack.

        // WCF sets an ActivityId for you, but with other contexts it may not be set.

        Guid _previousActivityId;
        TraceSource _source;
        int _startId;
        int _stopId;
        int _transferInId;
        int _transferOutId;
        string _transferInMessage = null;
        string _startMessage = null;
        string _transferOutMessage = null;
        string _stopMessage = null;

        /// <summary>
        /// Constructor. Sets the ActivityId for the life of the object without logging any events.
        /// </summary>
        public ActivityScope()
            : this((TraceSource)null, 0, 0, 0, 0)
        {
        }

        public ActivityScope(TraceSource source,string activityName)
            : this(source, 1600, 1601,1602,1603,null,null,null,null,activityName)
        {
            
        }
        /// <summary>
        /// Constructor. Sets the ActivityId for the life of the object, logging events with the specified event ID's.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public ActivityScope(TraceSource source, int transferInId, int startId, int transferOutId, int stopId)
        {
            _source = source;
            _startId = startId;
            _stopId = stopId;
            _transferInId = transferInId;
            _transferOutId = transferOutId;

            _previousActivityId = Trace.CorrelationManager.ActivityId;

            // Log Transfer In
            Guid newActivity = Guid.NewGuid();
            if (_source != null)
            {
                _source.TraceTransfer(_transferInId, ActivityScope_Transfer, newActivity);
            }
            Trace.CorrelationManager.ActivityId = newActivity;

            // Log Start Message
            if (_source != null)
            {
                _source.TraceEvent(TraceEventType.Start, _startId, ActivityScope_Start);
            }
        }

        private readonly string ActivityScope_Transfer = "TransferIn";
        private readonly string ActivityScope_Start= "Start";
        private readonly string ActivityScope_Stop="Stop";
        private readonly string ActivityScope_TransferOut="TransferOut";

        /// <summary>
        /// Constructor. Sets the ActivityId for the life of the object, logging events with the specified event ID's. 
        /// Additionally set custom messages for transfer-in, start, transfer-out and stop messages. Empty messages will be honored; if null values, the default messages will be used instead.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public ActivityScope(TraceSource source, int transferInId, int startId, int transferOutId, int stopId,
 string transferInMessage, string startMessage, string transferOutMessage, string stopMessage)
        {

            _source = source;
            _startId = startId;
            _stopId = stopId;
            _transferInId = transferInId;
            _transferOutId = transferOutId;

            _previousActivityId = Trace.CorrelationManager.ActivityId;

            _transferInMessage = transferInMessage;
            _startMessage = startMessage;
            _transferOutMessage = transferOutMessage;
            _stopMessage = stopMessage;

            // Log Transfer In
            Guid newActivity = Guid.NewGuid();
            if (_source != null)
            {
                _source.TraceTransfer(_transferInId, _transferInMessage ?? ActivityScope_Transfer, newActivity);
            }
            Trace.CorrelationManager.ActivityId = newActivity;

            // Log Start Message
            if (_source != null)
            {
                _source.TraceEvent(TraceEventType.Start, _startId, _startMessage ?? ActivityScope_Start);
            }
        }

        /// <summary>
        /// Constructor. Sets the ActivityId for the life of the object, logging events with the specified event ID's. 
        /// Additionally set custom messages for transfer-in, start, transfer-out and stop messages. Empty messages will be honored; if null values, the default messages will be used instead.
        /// Use this overload when using XmlWriterTraceListener. Activity name only applies for XmlWriterTraceListener.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public ActivityScope(TraceSource source, int transferInId, int startId, int transferOutId, int stopId,
 string transferInMessage, string startMessage, string transferOutMessage, string stopMessage, string activityName)
        {

            _source = source;
            _startId = startId;
            _stopId = stopId;
            _transferInId = transferInId;
            _transferOutId = transferOutId;

            _previousActivityId = Trace.CorrelationManager.ActivityId;

            _transferInMessage = transferInMessage;
            _startMessage = startMessage;
            _transferOutMessage = transferOutMessage;
            _stopMessage = stopMessage;

            // Log Transfer In
            Guid newActivity = Guid.NewGuid();
            if (_source != null)
            {
                _source.TraceTransfer(_transferInId, _transferInMessage ?? ActivityScope_Transfer, newActivity);
            }
            Trace.CorrelationManager.ActivityId = newActivity;

            // Log Start Message
            if (_source != null)
            {
                var xml = string.Format(@"<TraceRecord Severity='Start' xmlns='http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord'>
                     <TraceIdentifier></TraceIdentifier>
                     <Description>{0}</Description>
                     <AppDomain>{1}</AppDomain>
                     <ExtendedData xmlns='http://schemas.microsoft.com/2006/08/ServiceModel/DictionaryTraceRecord'>
                         <ActivityName>{2}</ActivityName>
                         <ActivityType>Construct</ActivityType>
                     </ExtendedData>
                 </TraceRecord>",
                 _startMessage ?? ActivityScope_Start,
                AppDomain.CurrentDomain.FriendlyName,
                activityName ?? _startMessage ?? ActivityScope_Start
                );

                var doc = new XmlDocument();

                doc.LoadXml(xml);

                _source.TraceData(TraceEventType.Start, _startId, doc.CreateNavigator());

            }
        }
        /// <summary>
        /// Disposes of the object, resetting the ActivityId.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose of the object, resetting the ActivityId.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Log Transfer Out
                if (_source != null)
                {
                    _source.TraceTransfer(_transferOutId, _transferOutMessage ?? ActivityScope_TransferOut, _previousActivityId);
                }

                // Log Stop Message
                if (_source != null)
                {
                    _source.TraceEvent(TraceEventType.Stop, _stopId, _stopMessage ?? ActivityScope_Stop);
                }
                Trace.CorrelationManager.ActivityId = _previousActivityId;
            }
        }
    }
}
