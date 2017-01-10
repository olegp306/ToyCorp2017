//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web;

namespace AdvantShop.Core
{
    public class UnhandledExceptionModule : IHttpModule
    {
        static int _unhandledExceptionCount = 0;
        static string _sourceName = null;
        static readonly object InitLock = new object();
        static bool _initialized = false;

        public void Init(HttpApplication app)
        {

            // Do this one time for each AppDomain.
            if (!_initialized)
            {
                lock (InitLock)
                {
                    if (!_initialized)
                    {

                        string webenginePath = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "webengine.dll");

                        if (!File.Exists(webenginePath))
                        {
                            throw new Exception(String.Format(CultureInfo.InvariantCulture,
                                                              "Failed to locate webengine.dll at '{0}'.  This module requires .NET Framework 2.0.",
                                                              webenginePath));
                        }

                        FileVersionInfo ver = FileVersionInfo.GetVersionInfo(webenginePath);

                        // Old
                        //_sourceName = string.Format(CultureInfo.InvariantCulture, "ASP.NET {0}.{1}.{2}.0",
                        //                            ver.FileMajorPart, ver.FileMinorPart, ver.FileBuildPart);

                        // WIth no CultureInfo.InvariantCulture
                        _sourceName = string.Format("ASP.NET {0}.{1}.{2}.0", ver.FileMajorPart, ver.FileMinorPart, ver.FileBuildPart);

                        if (!EventLog.SourceExists(_sourceName))
                        {
                            throw new Exception(String.Format(CultureInfo.InvariantCulture,
                                                              "There is no EventLog source named '{0}'. This module requires .NET Framework 2.0.",
                                                              _sourceName));
                        }

                        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);

                        _initialized = true;
                    }
                }
            }
        }

        public void Dispose()
        {
        }

        static void OnUnhandledException(object o, UnhandledExceptionEventArgs e)
        {
            // Let this occur one time for each AppDomain.
            if (Interlocked.Exchange(ref _unhandledExceptionCount, 1) != 0)
                return;

            var message = new StringBuilder("\r\n\r\nUnhandledException logged by UnhandledExceptionModule.dll:\r\n\r\nappId=");

            var appId = (string)AppDomain.CurrentDomain.GetData(".appId");
            if (appId != null)
            {
                message.Append(appId);
            }


            Exception currentException = null;
            for (currentException = (Exception)e.ExceptionObject; currentException != null; currentException = currentException.InnerException)
            {
                message.AppendFormat("\r\n\r\ntype={0}\r\n\r\nmessage={1}\r\n\r\nstack=\r\n{2}\r\n\r\n",
                                     currentException.GetType().FullName,
                                     currentException.Message,
                                     currentException.StackTrace);
                AdvantShop.Diagnostics.Debug.LogError(currentException, message.ToString());
            }
        }
    }
}