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
        private static readonly string LOG_DIRECTORY    = "Logs" + Path.DirectorySeparatorChar;
        private static readonly string FILE_EXTENTION   = ".log";
        private const int   MAX_LOG_COUNT               = 30;
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
        /// Logs the given info message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Info(string message)
        {
            Instance.LogMessage(message, LogLevel.Info, false);
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
        /// Logs the given warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Warning(string message)
        {
            Instance.LogMessage(message, LogLevel.Warning, false);
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
        /// Handles the logging of a message.
        /// </summary>
        /// <param name="message">The message content.</param>
        /// <param name="logLevel">The mesasge type.</param>
        private void LogMessage(string message, LogLevel logLevel, bool showStackTrace)
        {
            string dateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string level = string.Format(" [{0}]", logLevel);

            string line = (dateTime + level).PadRight(MESSAGE_START_PADDING) + message + '\n';

            if (showStackTrace)
            {
                string[] lines = Environment.StackTrace.Split('\n');

                string stackTrace = "";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i > 2)
                    {
                        stackTrace += lines[i] + '\n';
                    }
                }
                line += stackTrace;
            }

            if (m_echoToConsole)
            {
                Console.Write(line);
            }

            using (StreamWriter stream = File.AppendText(m_logPath))
            {
                stream.Write(line);
            }
        }
    }
}
