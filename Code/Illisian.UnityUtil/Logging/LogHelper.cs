using System;

namespace Illisian.UnityUtil.Logging
{
    public class LogHelper
    {
        /// <summary>
        /// Infoes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="objects">The objects.</param>
        public static void Info(string message, params object[] objects)
        {
            LogManager.Context.Log(LogType.Information, message, objects);
        }
        /// <summary>
        /// Warns the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="objects">The objects.</param>
        public static void Warn(string message, params object[] objects)
        {
            LogManager.Context.Log(LogType.Warning, message, objects);
        }
        /// <summary>
        /// Criticals the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="objects">The objects.</param>
        public static void Critical(string message, params object[] objects)
        {
            LogManager.Context.Log(LogType.Critical, message, objects);
        }

        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="objects">The objects.</param>
        public static void Debug(string message, params object[] objects)
        {
            LogManager.Context.Log(LogType.Debug, message, objects);
        }
    }
}
