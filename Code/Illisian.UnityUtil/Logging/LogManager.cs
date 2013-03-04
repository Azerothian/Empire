using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Illisian.UnityUtil.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public class LogManager : ContextAbstract<LogManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        /// <param name="objects">The objects.</param>
        public delegate void LogDelegate(LogType type, string message, params object[] objects);
        /// <summary>
        /// Occurs when [log event].
        /// </summary>
        public event LogDelegate LogEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="report">The report.</param>
       // public delegate void LogReportGeneratedDelegate(LogReport report);
        /// <summary>
        /// Occurs when [log report generated event].
        /// </summary>
       // public event LogReportGeneratedDelegate LogReportGeneratedEvent;

        /// <summary>
        /// Gets or sets the generate log report level.
        /// </summary>
        /// <value>
        /// The generate log report level.
        /// </value>
        public LogType GenerateLogReportLevel { get; set; }


        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        /// <value>
        /// The log level.
        /// </value>
        public LogType LogLevel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogManager"/> class.
        /// </summary>
        public LogManager()
        {
            GenerateLogReportLevel = LogType.Critical;
            LogLevel = LogType.Information;
        }
        /// <summary>
        /// Logs the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        /// <param name="objects">The objects.</param>
        public void Log(LogType type, string message, params object[] objects)
        {

            if (type >= LogLevel)
            {
                if (LogEvent != null)
                    LogEvent(type, message, objects);
             //   GenerateLogReport(type, message, objects);
            }

        }
        /// <summary>
        /// Logs the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        public void Log(string type, string message)
        {
            LogType etype = LogType.Information;
            switch (type.ToLower())
            {
                case "info":
                case "i":
                case "information":
                    etype = LogType.Information;
                    break;
                case "critical":
                case "c":
                case "crit":
                    etype = LogType.Critical;
                    break;
                case "warning":
                case "warn":
                case "w":
                    etype = LogType.Warning;
                    break;
                case "debug":
                case "d":
                    etype = LogType.Debug;
                    break;

            }
            Log(etype, message);

        }

        //private void GenerateLogReport(LogType type, string message, params object[] objects)
        //{
        //    if (type == GenerateLogReportLevel)
        //    {
        //        List<Exception> _exceptions = new List<Exception>();
        //        List<object> _genericObjects = new List<object>();
        //        foreach (var v in objects)
        //        {
        //            if (v is Exception)
        //            {
        //                _exceptions.Add(v as Exception);
        //            }
        //            else
        //            {
        //                _genericObjects.Add(v);
        //            }
        //        }
        //        LogReport report = new LogReport(type, message, _exceptions, _genericObjects);
        //        if (LogReportGeneratedEvent != null)
        //            LogReportGeneratedEvent(report);
        //    }
        //}

        /// <summary>
        /// Registers the app domain.
        /// </summary>
        /// <returns>
        /// bool
        /// </returns>
        public bool RegisterAppDomain()
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                return true;
            }
            catch (Exception ex)
            {
                Log(LogType.Warning, "Exception caught when Registering AppDomain.CurrentDomain.UnhandledException", this, ex);
                return false;
            }
        }
        /// <summary>
        /// Registers Exception Handling for all Application Threads.
        /// </summary>
        /// <returns>
        /// bool
        /// </returns>
        //public bool RegisterApplication()
        //{
        //    try
        //    {
        //        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
        //        Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log(LogType.Warning, "Exception caught when Registering 'Application.ThreadException' Event", this, ex);
        //        return false;
        //    }
        //}

        /// <summary>
        /// Registers the HTTP application.
        /// </summary>
        /// <returns></returns>
        //public bool RegisterHttpApplication()
        //{
        //    try
        //    {
        //        HttpContext.Current.ApplicationInstance.Error += new EventHandler(HttpApplicationInstance_Error);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log(LogType.Warning, "Exception caught when Registering 'HttpContext.Current.ApplicationInstance.Error' Event", this, ex);
        //        return false;
        //    }
        //}

        //void HttpApplicationInstance_Error(object sender, EventArgs e)
        //{
        //    Exception ex = HttpContext.Current.Server.GetLastError();
        //    HttpContext.Current.Server.ClearError();

        //    // All exceptions thrown by the main thread is handled over this method        
        //    Log(LogType.Critical, "Exception caught by 'HttpApplicationInstance_Error' Event", sender, ex);
        //}

        //void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        //{
        //    // All exceptions thrown by the main thread is handled over this method        
        //    Log(LogType.Critical, "Exception caught by 'Application_ThreadException' Event", sender, e.Exception);

        //}

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // All exceptions thrown by additional threads are handled in this method
            Log(LogType.Critical, "Exception caught by 'Application_ThreadException' Event", sender, e.ExceptionObject as Exception);
            // Suspend the current thread for now to stop the exception from throwing.
            //Thread.CurrentThread.Suspend(); // let it throw, let it throw

        }


    }
}
