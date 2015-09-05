// Last edited by martin: 2014_09_01 8:31 PM
// Created : 2014_08_03

#region imports

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using xSolon.Events;

#endregion

namespace xSolon.Logs.Destinations
{
    /// <summary>
    ///     Destination that submits a POST request of serialized events to a service url endpoint
    /// </summary>
    public class RemoteDestination : BaseLogDestination
    {
        /// <summary>
        ///     Function to modify events before they are committed to service
        /// </summary>
        [XmlIgnore]
        public Func<List<EventEntry>, List<EventEntry>> PreProcessEntries = null;

        /// <summary>
        ///     Url of service that will receive outgoing logs
        /// </summary>
        public string ServiceUrl = "";

        /// <summary>
        ///     Commits this instance by submitting the current events to service url
        /// </summary>
        public override void Commit()
        {
            try
            {
                var temp = GetEntries().ToList();

                if (PreProcessEntries != null)
                {
                    temp = PreProcessEntries(temp);
                }

                var json = temp.ToJson();

                var res = HttpPost(ServiceUrl, json);

                Debug.WriteLine(res);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        public override object Clone()
        {
            var res = new RemoteDestination
            {
                AutoCommit = AutoCommit,
                Level = Level,
                //LogList = this.LogList.ToList(), PreProcessEntries = this.PreProcessEntries,
                ServiceUrl = ServiceUrl
            };

            return res;
        }

        /// <summary>
        ///     Submit events as a POST request to ServiceUrls
        /// </summary>
        /// <param name="uri">Url of service endpoint</param>
        /// <param name="parameters">Serialized Events</param>
        /// <returns></returns>
        public virtual string HttpPost(string uri, string parameters)
        {
            var req = WebRequest.Create(uri);

            req.ContentType = "application/x-www-form-urlencoded";

            req.Method = "POST";

            byte[] bytes = Encoding.ASCII.GetBytes(parameters);

            req.ContentLength = bytes.Length;

            using (var os = req.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);

                os.Close();
            }

            var resp = req.GetResponse();

            var stream = resp.GetResponseStream();

            if (stream != null)
                using (stream)
                using (var sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd().Trim();
                }
            return "Response was null";
        }
    }
}