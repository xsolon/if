// Last edited by martin: 2014_09_01 8:38 PM
// Created : 2014_08_19

#region imports

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using xSolon.Events;

#endregion

namespace xSolon.Logs.Destinations
{
    /// <summary>
    ///     Asynchronous implementation of RemoteDestionation
    /// </summary>
    public class AsyncRemoteDestination : RemoteDestination
    {
        private static readonly Object ListLock = new Object();

        public override string HttpPost(string uri, string parameters)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    var res = base.HttpPost(uri, parameters);

                    Debug.WriteLine(res);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            });

            const string result = "Scheduled";

            return result;
        }

        public override void Commit()
        {
            lock (ListLock)
                base.Commit();
        }

        public override void Log(EventEntry entry)
        {
            lock (ListLock)
                base.Log(entry);
        }

        public override object Clone()
        {
            var res = new AsyncRemoteDestination
            {
                AutoCommit = AutoCommit,
                Level = Level,
                LogList = LogList.ToList(),
                PreProcessEntries = PreProcessEntries,
                ServiceUrl = ServiceUrl
            };

            return res;
        }
    }
}