using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SoSmooth
{
    /// <summary>
    /// Formats log messages and sends them to the log writer.
    /// It is thread safe, but writes are briefly buffered so
    /// in event of a fatal crash output may not be complete.
    /// </summary>
    public class Logger : Singleton<Logger>
    {
        /// <summary>
        /// The directory of the log files relative to the application launch directory.
        /// </summary>
        private static readonly string LOG_DIRECTORY    = "Logs";

        /// <summary>
        /// The file extention used for log files.
        /// </summary>
        private static readonly string FILE_EXTENTION   = ".log";

        /// <summary>
        /// The maximum number of logs before the oldest will be automatically removed.
        /// </summary>
        private const int MAX_LOG_COUNT = 30;

        /// <summary>
        /// The column messages start at in the log file, used to keep messages aligned
        /// despite any difference in the length of the time and message type prefix.
        /// </summary>
        private const int MESSAGE_START_PADDING = 32;

        public readonly string logPath;
        private readonly LogWriter m_writer;

        public bool echoToConsole;

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
            string logName = Program.NAME + "_" + logStartTime + FILE_EXTENTION;
            
            logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LOG_DIRECTORY, logName);

            echoToConsole = false;
            m_writer = new LogWriter();
        }
        
        /// <summary>
        /// Logs the given object.
        /// </summary>
        /// <param name="o">An object to print.</param>
        /// <param name="printStackTrace">Prints the stack trace.</param>
        public static void Info(object o, bool printStackTrace = false)
        {
            Info(o.ToString(), printStackTrace);
        }

        /// <summary>
        /// Logs the given object.
        /// </summary>
        /// <param name="o">An object to print.</param>
        /// <param name="printStackTrace">Prints the stack trace.</param>
        public static void Debug(object o, bool printStackTrace = false)
        {
            Debug(o.ToString(), printStackTrace);
        }

        /// <summary>
        /// Logs the given object.
        /// </summary>
        /// <param name="o">An object to print.</param>
        /// <param name="printStackTrace">Prints the stack trace.</param>
        public static void Warning(object o, bool printStackTrace = false)
        {
            Warning(o.ToString(), printStackTrace);
        }

        /// <summary>
        /// Logs the given object.
        /// </summary>
        /// <param name="o">An object to print.</param>
        /// <param name="printStackTrace">Prints the stack trace.</param>
        public static void Error(object o, bool printStackTrace = true)
        {
            Error(o.ToString(), printStackTrace);
        }

        /// <summary>
        /// Logs the given info message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="printStackTrace">Prints the stack trace.</param>
        public static void Info(string message, bool printStackTrace = false)
        {
            Instance.LogMessage(message, LogLevel.Info, printStackTrace);
        }

        /// <summary>
        /// Logs the given debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="printStackTrace">Prints the stack trace.</param>
        public static void Debug(string message, bool printStackTrace = false)
        {
            Instance.LogMessage(message, LogLevel.Debug, printStackTrace);
        }

        /// <summary>
        /// Logs the given warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="printStackTrace">Prints the stack trace.</param>
        public static void Warning(string message, bool printStackTrace = false)
        {
            Instance.LogMessage(message, LogLevel.Warning, printStackTrace);
        }

        /// <summary>
        /// Logs the given error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Error(string message, bool printStackTrace = true)
        {
            Instance.LogMessage(message, LogLevel.Error, printStackTrace);
        }

        /// <summary>
        /// Logs the given exception.
        /// </summary>
        /// <param name="message">The exception to log.</param>
        public static void Exception(object exception)
        {
            // ToString on exeption objects typically includes the stack trace, so we don't need to include it
            Instance.LogMessage(exception.ToString(), LogLevel.Error, false);
        }

        private static readonly string[] NEW_LINES = new string[] { Environment.NewLine };

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
                string[] lines = Environment.StackTrace.Split(NEW_LINES, StringSplitOptions.RemoveEmptyEntries);

                string stackTrace = "";
                for (int i = 0; i < lines.Length; i++)
                {
                    // Don't include the function calls in the logger in the stack trace, as it is not useful
                    if (i > 2 && !lines[i].Contains(typeof(Logger).FullName))
                    {
                        stackTrace += lines[i] + Environment.NewLine;
                    }
                }
                line += stackTrace;
            }

            m_writer.BufferMessage(line);
        }
    }
}
