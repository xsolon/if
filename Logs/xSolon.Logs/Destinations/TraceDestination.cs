// TraceDestination.cs
// Last edited by martin: 2014_08_21 1:27 AM
// Created : 2014_08_21

#region imports

using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using xSolon.Events;

#endregion

namespace xSolon.Logs.Destinations
{
    public class TraceDestination : BaseLogDestination
    {
        private static bool? _ConsoleAvailable = new bool?();
        private static readonly object logListLock = new Object();
        public bool WriteConsole = false;

        public bool WriteDebug = true;
        public bool WriteTrace = true;

        public static bool ConsoleAvailable
        {
            get
            {
                if (!_ConsoleAvailable.HasValue)
                {
                    {
                        _ConsoleAvailable = IsConsoleAvailable();
                    }
                }

                return _ConsoleAvailable.Value;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        private static bool IsConsoleAvailable()
        {
            if (GetConsoleWindow() != IntPtr.Zero)
            {
                return true;
            }
            return false;
        }

        public override void Log(EventEntry entry)
        {
            lock (logListLock)
                base.Log(entry);
        }

        public override void Commit()
        {
            if (LogList != null && LogList.Count > 0)
            {
                lock (logListLock)
                {
                    if (LogList != null && LogList.Count > 0)
                    {
                        #region MyRegion

                        var temp = LogList.ToList();

                        LogList.Clear();

                        temp.ForEach(i =>
                        {
                            if (i != null)
                            {
                                var msg = GetMessage(i);

                                if (WriteTrace)
                                {
                                    Trace.WriteLine(msg);
                                }
                                else if (WriteDebug)
                                {
                                    Debug.WriteLine(msg);
                                }

                                if (WriteConsole && ConsoleAvailable)
                                {
                                    try
                                    {
                                        Console.WriteLine(msg);
                                    }
                                    catch (Exception ex)
                                    {
                                        Trace.WriteLine(ex.ToString());

                                        WriteConsole = false;

#if DEBUG

                                        WriteDebug = true;
#else
                        WriteTrace = true;
#endif
                                    }
                                }
                            }
                        });

                        #endregion
                    }
                }
            }
        }

        public override object Clone()
        {
            return new TraceDestination
            {
                WriteTrace = WriteTrace,
                WriteConsole = WriteConsole,
                WriteDebug = WriteDebug
            };
        }
    }
}