using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Messaging;
using System.Text;

namespace xSolon.TraceListeners
{
    public class MsmqTraceListener : TraceListener, IDisposable
    {
        #region Ctor

        public MsmqTraceListener()
        {

        }

        public MsmqTraceListener(string path)
        {

            Debug.WriteLine(string.Format("MsmqTraceListener: {0}", path));

            QueuePath = path;

        }

        ~MsmqTraceListener()
        {

            this.Dispose();

        }
        #endregion

        public override void Write(string message)
        {
            //Debug.Write("Debug: " + message);

            //this.SendToMSMQ(new string[] { message }.ToList());
        }

        public override void WriteLine(string message)
        {
            //Debug.WriteLine("Debug: " + message);

        }

        #region Props
        List<string> LogList = new List<string>();
        public string QueuePath = @"FormatName:DIRECT=OS:.\private$\LogQueue";

        public bool JsonSerialization = true;
        private string Source = string.Empty;

        private MessageQueue myQueue;

        /// <summary>
        /// Gets or sets my queue.
        /// </summary>
        /// <value>My queue.</value>
        public MessageQueue MyQueue
        {
            get
            {
                if (myQueue == null)
                {
                    myQueue = new MessageQueue(QueuePath)
                    {
                        Path = QueuePath,
                        Formatter = new XmlMessageFormatter(new Type[]
                        {
                            typeof(string),
                        })
                    };
                }

                return myQueue;
            }
        }

        #endregion

        /// <summary>
        /// Commits this instance.
        /// </summary>
        public void Commit()
        {
            List<string> temp = LogList.ToList();

            LogList.Clear();

            SendToMSMQ(temp);
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            base.TraceData(eventCache, source, eventType, id, data);
        }
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {

            var msg = "";

            if (eventType == TraceEventType.Critical || eventType == TraceEventType.Error)
            {

                msg = new
                {
                    EventType = eventType,
                    Id = id,
                    Message = string.Format(format, args ?? new object[0]),
                    Stack = eventCache.Callstack,
                    Time = eventCache.DateTime
                }.ToString();

            }
            else
            {

                msg = new
                {
                    EventType = eventType,
                    Id = id,
                    Message = string.Format(format, args ?? new object[0]),
                    //Stack = eventCache.Callstack,
                    Time = eventCache.DateTime
                }.ToString();

            }

            this.SendToMSMQ(new string[] { msg }.ToList());

            base.TraceEvent(eventCache, source, eventType, id, format, args);

        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing,
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {

            Debug.WriteLine("Disposing MsmqTraceListener");

            if (myQueue != null)
            {
                myQueue.Close();
                myQueue.Dispose();
                myQueue = null;
            }



        }

        public void SendToMSMQ(List<string> entries)
        {
            if (entries == null || entries.Count <= 0)
            {
                return;
            }

            try
            {

                object obj = entries;

                if (JsonSerialization)
                {
                    string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(entries);

                    obj = json;

                }

                var m = new System.Messaging.Message(obj);

                MyQueue.Send(m, Source);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
    }
}
