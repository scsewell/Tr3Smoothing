using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SoSmooth
{
    /// <summary>
    /// Handles logging buffered output to console and file.
    /// </summary>
    public class LogWriter
    {
        private List<string> m_buffer = new List<string>();
        private object m_bufferLock = new object();
        private object m_writeLock = new object();

        private uint m_bytesWritten = 0;

        /// <summary>
        /// The number of bytes written to the log file.
        /// </summary>
        public uint LogFileSize => m_bytesWritten;

        public LogWriter()
        {
            // start a thread for the log write loop
            Task.Run(() => LogLoop());
        }

        /// <summary>
        /// Loop that repeatedly checks the buffer for any new messages, and
        /// if any are found logs them to file and optionally console.
        /// </summary>
        public void LogLoop()
        {
            List<string> toWrite = new List<string>();

            while (true)
            {
                // Aquire any buffered log messages
                lock (m_bufferLock)
                {
                    toWrite.AddRange(m_buffer);
                    m_buffer.Clear();
                }

                // write to the current log file and close the stream
                lock (m_writeLock)
                {
                    using (StreamWriter stream = File.AppendText(Logger.Instance.LogPath))
                    {
                        // log the buffered messages
                        foreach (string line in toWrite)
                        {
                            stream.Write(line);
                            m_bytesWritten += (uint)line.Length;

                            // Optionally print the message out to the console
                            if (Logger.Instance.echoToConsole)
                            {
                                Console.Write(line);
                            }
                        }
                    }
                }

                // clear away written messages
                toWrite.Clear();
            }
        }

        /// <summary>
        /// Adds a message to the message buffer.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void BufferMessage(string message)
        {
            lock (m_bufferLock)
            {
                m_buffer.Add(message);
            }
        }

        /// <summary>
        /// Logs a message synchronously.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void WriteSynchronous(string message)
        {
            lock (m_writeLock)
            {
                // write to the current log file and close the stream
                using (StreamWriter stream = File.AppendText(Logger.Instance.LogPath))
                {
                    stream.Write(message);
                    m_bytesWritten += (uint)message.Length;

                    // Optionally print the message out to the console
                    if (Logger.Instance.echoToConsole)
                    {
                        Console.Write(message);
                    }
                }
            }
        }
    }
}
