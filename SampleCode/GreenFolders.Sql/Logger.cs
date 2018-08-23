
using System;
using System.Diagnostics;
using System.Text;

namespace GreenFolders.Sql
{
    public static class Logger
    {
        #region Public Methods

        public static void Error(string message)
        {
            try
            {
#if NETFULL
                Write(FormatMessageFromStack(StackTrace(), message), EventLogEntryType.Error);
#endif // NETFULL
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static void Error(Exception ex, string message = "")
        {
            try
            {
                Error($"{(string.IsNullOrEmpty(message) ? "" : message)} \n{StackTrace(ex)}");
            }

            catch (Exception outerEx)
            {
                Debug.WriteLine(outerEx.Message);
            }
        }

        #endregion Public Methods


        #region Private Methods

        private static string FormatMessageFromStack(string stackTrace, string message)
        {
            var result = "";

            try
            {
                result = $"{message}{stackTrace}";
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return result;
        }

#if NETFULL
        private static void Write(string message, EventLogEntryType eventType)
        {
            try
            {
                if (!string.IsNullOrEmpty(message))
                {
                    using (var applicationLog = new EventLog())
                    {
                        applicationLog.Source = "Application";

                        applicationLog.WriteEntry(message, eventType);

                        Debug.WriteLine(message);
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
#endif // NETFULL

        private static string StackTrace(Exception ex)
        {
            var message = new StringBuilder();

            if (ex != null)
            {
                message.Append($"Exception Occured: \nMessage: {ex.Message} \nTarget: {ex.TargetSite} \nStack: {ex.StackTrace} \nInner: ");
                
                var innerException = ex.InnerException;

                while (innerException != null)
                {
                    message.Append(" --> ");
                    message.Append(innerException.Message);

                    innerException = innerException.InnerException;
                }
            }

            return message.ToString();
        }

        private static string StackTrace()
        {
            var message = new StringBuilder();

            message.Append(Environment.NewLine);
            message.Append("Call Stack:");
            message.Append(Environment.NewLine);

            var stackFrames = new StackTrace(1, true).GetFrames();

            if (stackFrames != null)
            {
                foreach (var stackFrame in stackFrames)
                {
                    var method = stackFrame?.GetMethod();

                    if (method != null)
                    {
                        message.Append($" - {method} : File: {stackFrame.GetFileName()} Line: {stackFrame.GetFileLineNumber()}");

                        message.Append(Environment.NewLine);
                    }
                }
            }

            else
            {
                message.Append("Unable to retrieve stack.");
            }

            return message.ToString();
        }

        #endregion Private Methods
    }
}
