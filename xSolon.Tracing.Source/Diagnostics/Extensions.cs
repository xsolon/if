namespace System.Diagnostics
{
    public static class Extensions
    {
        public static T Wrap<T>(this T target) where T : TracedClass
        {
            var proxy = new TracedProxy<T>(target);

            var res = (T)proxy.GetTransparentProxy();

            return res;
        }

        public static void Indent(this TraceSource target)
        {

            foreach (TraceListener listener in target.Listeners)
            {
                listener.IndentLevel++;
            }
        }

        public static void Unindent(this TraceSource target)
        {

            foreach (TraceListener listener in target.Listeners)
            {
                listener.IndentLevel--;
            }

        }
    }
}
