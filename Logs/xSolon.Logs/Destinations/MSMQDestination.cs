// xSolon.Instructions.DTO MSMQDestination.cs  4/17/2015 martinm 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Messaging;
using xSolon.Events;

namespace xSolon.Logs.Destinations
{
    public class MSMQDestination : BaseLogDestination, IDisposable
    {

        #region Props
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
                            typeof(List<EventEntry>),
                            typeof(EventEntry)
                        })
                    };
                }

                return myQueue;
            }
        }
        
        #endregion

        #region Ctor

        public MSMQDestination()
        {
        }
        public MSMQDestination(string source)
        {
            Source = source;
        }

        #endregion

        ~MSMQDestination()
        {

            this.Dispose();

        }

        public override object Clone()
        {
            var res = new MSMQDestination()
            {
                AutoCommit = AutoCommit,
                Level = Level,
                QueuePath = QueuePath,
                Source = Source,
                JsonSerialization = JsonSerialization
            };

            return res;
        }

        /// <summary>
        /// Commits this instance.
        /// </summary>
        public override void Commit()
        {
            List<EventEntry> temp = LogList.ToList();

            LogList.Clear();

            SendToMSMQ(temp);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing,
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {

            if (myQueue != null)
            {
                myQueue.Close();
                myQueue.Dispose();
                myQueue = null;
            }

            

        }

        public void SendToMSMQ(List<EventEntry> entries)
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
                    obj = entries.ToJson();

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