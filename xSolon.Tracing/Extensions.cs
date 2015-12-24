using System;
using System.Collections.Generic;
using System.Text;

namespace xSolon.Tracing
{
    public static class Extensions
    {
        public static T Wrap<T>(T target) where T : TracedClass
        {
            var proxy = new TracedProxy<T>(target);

            var res = (T)proxy.GetTransparentProxy();

            return res;
        }

    }
}
