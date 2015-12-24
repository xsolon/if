using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;

namespace xSolon.TraceListeners
{
    internal class TracedProxy<T> : RealProxy where T : TraceListener
    {
        private readonly T _dal;
        //private readonly LoggerClass _logger;

        public TracedProxy(T dal)
            : base(dal.GetType())
        {
            //this._logger = logger;
            _dal = dal;

            typeName = _dal.GetType().Name;
            //_dal.NotifyEvent += logger.NotifyGeneric;

        }

        private string typeName = "";

        private Dictionary<string, object> GetParams(IMethodCallMessage methodCall)
        {
            var res = new Dictionary<string, object>();

            for (var i = 0; i < methodCall.InArgCount; i++)
            {
                var key = methodCall.GetInArgName(i);
                var val = methodCall.GetInArg(i);

                var stringVal = GetSerializedObject(val);

                res.Add(key, stringVal);
            }

            return res;
        }

        private static string GetSerializedObject(object obj)
        {
            var res = string.Empty;

            if (obj == null)
            {
                res = "NULL";
            }
            else
            {
                var seri = new System.Web.Script.Serialization.JavaScriptSerializer();

                #region Serialize Value
                bool error = false;
                try
                {
                    res = seri.Serialize(obj);
                }
                catch
                {
                    error = true;
                }

                if (error)
                {
                    try
                    {
                        res = obj.ToString();
                        error = false;
                    }
                    catch
                    {
                        error = true;
                    }
                }

                #endregion
            }

            return res;
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            var methodInfo = methodCall.MethodBase as MethodInfo;
            var inParams = GetParams(methodCall);

            //_dal.NotifyVerbose(inParams, "{1} - Before executing '{0}'", methodCall.MethodName, typeName);

            try
            {

                var result = methodInfo.Invoke(_dal, methodCall.InArgs);

                inParams.Add("Result", GetSerializedObject(result));

                Trace.TraceInformation("{1} - After executing '{0}' ", methodCall.MethodName, typeName);

                Trace.Indent();

                inParams.ToList().ForEach(i =>
                {

                    Trace.WriteLine(i.Value,i.Key);

                });

                Trace.Unindent();

                return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
            }
            catch (Exception e)
            {
                Trace.TraceError("{1} - Exception {0} executing '{1}'", e, methodCall.MethodName, typeName);

                Trace.WriteLine(e.ToString());

                throw;
                //return new ReturnMessage(e, methodCall);
            }
        }

        public new T GetTransparentProxy()
        {

            if (true)
                return base.GetTransparentProxy() as T;
            else
                return _dal;
        }
    }

    public static class Extensions
    {
        public static T Wrap<T>(T target) where T : TraceListener
        {
            var proxy = new TracedProxy<T>(target);

            var res = (T)proxy.GetTransparentProxy();

            return res;
        }

    }
}
