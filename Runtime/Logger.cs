using System;
using UnityEngine;

namespace Logging.Runtime
{
    public static class Logger
    {
        private const string LogColor = "#00ffff";
        /// <summary>
        /// Prints a log message. 
        /// </summary>
        /// <param name="message"> Message to be printed </param>
        /// <param name="requester"> The object the calling this function</param>
        /// <param name="editorOnly"> Optional. If this message should only be printed in the Editor. </param>
        public static void Log(string message, object requester, bool editorOnly = true) =>
            Print(Debug.Log, evt => OnMessageLogged?.Invoke(evt), message, LogColor, requester, editorOnly);
        

        private const string WarningColor = "#f5bd45";
        /// <summary>
        /// Prints a warning message. 
        /// </summary>
        /// <param name="message"> Message to be printed </param>
        /// <param name="requester"> The object the calling this function</param>
        /// <param name="editorOnly"> Optional. If this message should only be printed in the Editor. </param>
        public static void LogWarning(string message, object requester, bool editorOnly = true) =>
            Print(Debug.LogWarning, evt => OnWarningLogged?.Invoke(evt), message, WarningColor, requester, editorOnly);
        
        private const string ErrorColor = "#ff7369";
        /// <summary>
        /// Prints an error message. 
        /// </summary>
        /// <param name="message"> Message to be printed </param>
        /// <param name="requester"> The object the calling this function</param>
        public static void LogError(string message, object requester) =>
            Print(Debug.LogError, evt => OnErrorLogged?.Invoke(evt), message, ErrorColor, requester, false);

        private static void Print(Action<string> logFunction, Action<LogEvent> callEvent, string message, string color, object requester, bool editorOnly) {
            var messagePlain = FormatTextPLain(message, requester);
            var messageRich = FormatTextRich(message, color, requester);
            
            if (Application.platform is RuntimePlatform.WebGLPlayer && !editorOnly)
                logFunction(messagePlain);
            else if (Application.platform is RuntimePlatform.WindowsEditor or RuntimePlatform.OSXEditor or RuntimePlatform.LinuxEditor) 
                logFunction(messageRich);
            
            callEvent(new LogEvent(messagePlain, messageRich, requester));
        }
        
        private const string LogFormatRichText = "<b><color={0}>[{1}]</color></b> {2}";
        private const string LogFormatPlainText = "[{0}] {1}";
        
        private static string FormatTextPLain(string message, object requester) => string.Format(LogFormatPlainText, requester.GetType().Name, message);
        private static string FormatTextRich(string message, string color, object requester) => string.Format(LogFormatRichText, color, requester.GetType().Name, message);
        
        public static event Action<LogEvent> OnMessageLogged;
        public static event Action<LogEvent> OnWarningLogged;
        public static event Action<LogEvent> OnErrorLogged;
    }

    public struct LogEvent
    {
        public LogEvent(string messagePlainText, string messageRichText, object requester) {
            MessagePlainText = messagePlainText;
            MessageRichText = messageRichText;
            Requester = requester;
        }
        
        public string MessagePlainText;
        public string MessageRichText;
        public object Requester;
    }
}