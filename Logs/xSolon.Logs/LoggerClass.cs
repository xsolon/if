// xSolon.Instructions.DTO BaseLogger.cs  4/17/2015 martinm 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using xSolon.Events;
using xSolon.Logs.Destinations;

namespace xSolon.Logs
{
    /// <summary>
    /// Implementation of the ILog, ILoggerClass
    /// Provides a basic logging router
    /// </summary>
    public class LoggerClass : LoggedClass, ILoggerClass, ICloneable
    {

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerClass" /> class.
        /// </summary>
        public LoggerClass()
            : base()
        {
            //if (NotifyEvent != null)
            //NotifyEvent += LoggerClass_NotifyEvent;
        }

        public LoggerClass(string category)
            : base()
        {
            if (!String.IsNullOrEmpty(category))
            {
                Categories.Add(category);
            }
        }

        public LoggerClass(string category = "", params BaseLogDestination[] dests)
        {

            if (!String.IsNullOrEmpty(category))
            {
                Categories.Add(category);
            }

            if (dests != null && dests.Length > 0)
            {

                this.Destinations.AddRange(dests);
            }
        }
        #endregion

        /// <summary>
        /// Occurs when [on commit].
        /// </summary>
        public event EventHandler<ILoggerOnCommitArgs> OnCommit;

        public List<string> Categories = new List<string>();

        void Init(string cat)
        {
            //if (!String.IsNullOrEmpty(cat))
            //    InitializeLogger(cat);

        }

        //public event OnCommitHandler OnCommit = null;

        /// <summary>
        /// Pushes the log entry to all registered <see cref="BaseLogDestination"/>s, in addition to calling notifying all NotifyEvent consumers.
        /// </summary>
        /// <param name="EventEntry">The log entry.</param>
        public override void NotifyGeneric(EventEntry EventEntry)
        {
            if (Categories != null && Categories.Count > 0)
            {
                EventEntry.Categories.AddRange(Categories);

                EventEntry.Categories = EventEntry.Categories.Distinct().ToList();
            }

            base.NotifyGeneric(EventEntry);

            foreach (BaseLogDestination destination in Destinations)
            {
                if (destination.ShouldLog(EventEntry))
                {
                    destination.Log(EventEntry);
                }
            }

        }

        /// <summary>
        /// List of Destinations that will get log entries
        /// </summary>
        public List<BaseLogDestination> Destinations = new List<BaseLogDestination>();

        public override void Dispose()
        {
            ((ILoggerClass)this).CommitLog();

            try
            {
                Destinations.ForEach(i =>
                {
                    if (i is IDisposable)
                    {
                        (i as IDisposable).Dispose();
                    }
                });
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            base.Dispose();
        }

        /// <summary>
        /// Commits any log entries that have not being commited to the registered destinations. Used if AutoCommit is false. Allows sending multiple log entries as one.
        /// </summary>
        public void CommitLog()
        {
            if (OnCommit != null)
            {
                OnCommit(this, new ILoggerOnCommitArgs()
                {
                    Logger = this,
                    Destinations = Destinations
                });
            }

            foreach (BaseLogDestination destination in Destinations)
            {
                destination.Logger = this;

                try
                {
                    destination.Commit();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }

                destination.GetEntries().Clear();
            }
        }

        public object Clone()
        {
            var logger = new LoggerClass
            {
                Categories = Categories.ToList(),
                InstanceId = InstanceId,
                Destinations = Destinations.ToList()
            };

            return logger;
        }

    }
}