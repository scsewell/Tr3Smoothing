using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SoSmooth
{
    /// <summary>
    /// Handles logging output to console and file.
    /// </summary>
    public class Logger : Singleton<Logger>
    {
        /// <summary>
        /// The directory of the log files relative to the application launch directory.
        /// </summary>
        private static readonly string LOG_DIRECTORY    = "Logs" + Path.DirectorySeparatorChar;

        /// <summary>
        /// The file extention used for log files.
        /// </summary>
        private static readonly string FILE_EXTENTION   = ".log";

        /// <summary>
        /// The maximum number of logs before the oldest will be automatically removed.
        /// </summary>
        private const int   MAX_LOG_COUNT               = 30;

        /// <summary>
        /// The column messages start at in the log file, used to keep messages aligned
        /// despite any difference in the length of the time and message type prefix.
        /// </summary>
        private const int   MESSAGE_START_PADDING       = 32;

        private string m_logPath;
        private bool m_echoToConsole;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Logger()
        {
            // Create a directory for the log files beside the exe if it does not already exist
            if (!Directory.Exists(LOG_DIRECTORY))
            {
                Directory.CreateDirectory(LOG_DIRECTORY);
            }

            // Limit the number of previous logs stored by deleting the oldest
            List<FileInfo> logs = new DirectoryInfo(LOG_DIRECTORY).GetFiles('*' + FILE_EXTENTION).ToList();
            logs.Sort((x, y) => (x.CreationTime.CompareTo(y.CreationTime)));

            while (logs.Count >= MAX_LOG_COUNT)
            {
                logs[0].Delete();
                logs.RemoveAt(0);
            }

            // Get the filepath for this session's log
            string logStartTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            m_logPath = AppDomain.CurrentDomain.BaseDirectory + LOG_DIRECTORY + Program.NAME + "_" + logStartTime + FILE_EXTENTION;
            
            m_echoToConsole = false;
        }

        public static void SetEchoToConsole(bool echo)
        {
            Instance.m_echoToConsole = echo;
        }

        /// <summary>
        /// Logs the given object.
        /// </summary>
        /// <param name="o">An object to print.</param>
        public static void Info(object o)
        {
            Info(o.ToString());
        }

        /// <summary>
        /// Logs the given info message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Info(string message)
        {
            Instance.LogMessage(message, LogLevel.Info, false);
        }

        /// <summary>
        /// Logs the given object.
        /// </summary>
        /// <param name="o">An object to print.</param>
        public static void Debug(object o)
        {
            Debug(o.ToString());
        }

        /// <summary>
        /// Logs the given debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Debug(string message)
        {
            Instance.LogMessage(message, LogLevel.Debug, false);
        }

        /// <summary>
        /// Logs the given object.
        /// </summary>
        /// <param name="o">An object to print.</param>
        public static void Warning(object o)
        {
            Warning(o.ToString());
        }

        /// <summary>
        /// Logs the given warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Warning(string message)
        {
            Instance.LogMessage(message, LogLevel.Warning, false);
        }

        /// <summary>
        /// Logs the given object.
        /// </summary>
        /// <param name="o">An object to print.</param>
        public static void Error(object o)
        {
            Error(o.ToString());
        }

        /// <summary>
        /// Logs the given error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Error(string message)
        {
            Instance.LogMessage(message, LogLevel.Error, true);
        }

        /// <summary>
        /// Logs the given exception.
        /// </summary>
        /// <param name="message">The exception to log.</param>
        public static void Exception(object exception)
        {
            // ToString on exeption objects typically include the stack trace, so we don't need to include it
            Instance.LogMessage(exception.ToString(), LogLevel.Error, false);
        }

        /// <summary>
        /// Synchronously handles the writing of a message to the current log file.
        /// </summary>
        /// <param name="message">The message content.</param>
        /// <param name="logLevel">The mesasge type.</param>
        /// <param name="showStackTrace">If true includes a stack trace.</param>
        private void LogMessage(string message, LogLevel logLevel, bool showStackTrace)
        {
            // Start log message lines with the date and message type
            string dateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string level = string.Format(" [{0}]", logLevel);

            // Add padding to the line so that the messages start in the same column of the log
            string line = (dateTime + level).PadRight(MESSAGE_START_PADDING) + message + Environment.NewLine;

            // Include a stack trace if desired
            if (showStackTrace)
            {
                string[] lines = Environment.StackTrace.Split('\n');

                string stackTrace = "";
                for (int i = 0; i < lines.Length; i++)
                {
                    // Don't include the function calls in the logger in the stack trace, as it is not useful
                    if (i > 2)
                    {
                        stackTrace += lines[i] + Environment.NewLine;
                    }
                }
                line += stackTrace;
            }

            // Optionally print the message out to the console
            if (m_echoToConsole)
            {
                Console.Write(line);
            }

            // Write to the current log file and close the stream
            using (StreamWriter stream = File.AppendText(m_logPath))
            {
                stream.Write(line);
            }
        }
    }
}
