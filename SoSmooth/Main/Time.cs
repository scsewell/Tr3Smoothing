using System;

namespace SoSmooth
{
    /// <summary>
    /// Keeps track of time information.
    /// </summary>
    public static class Time
    {
        private static DateTime m_loadTime = DateTime.Now;
        private static TimeSpan m_deltaTime = TimeSpan.Zero;
        private static DateTime m_lastFrameEnd = DateTime.Now;

        /// <summary>
        /// The total time in seconds up until the end of last update loop.
        /// </summary>
        public static float time
        {
            get { return (float)(m_lastFrameEnd - m_loadTime).TotalSeconds; }
        }
        
        /// <summary>
        /// The time in seconds since the main update loop last ran.
        /// </summary>
        public static float deltaTime
        {
            get { return (float)m_deltaTime.TotalSeconds; }
        }

        /// <summary>
        /// Saves the time that the application started at.
        /// </summary>
        public static void SetStartTime()
        {
            m_loadTime = DateTime.Now;
        }

        /// <summary>
        /// Updates main loop timing information.
        /// </summary>
        public static void FrameStart()
        {
            DateTime frameEnd = DateTime.Now;
            m_deltaTime = frameEnd - m_lastFrameEnd;
            m_lastFrameEnd = frameEnd;
        }
    }
}
