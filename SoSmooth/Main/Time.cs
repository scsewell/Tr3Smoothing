using System;

namespace SoSmooth
{
    /// <summary>
    /// Keeps track of time information.
    /// </summary>
    public static class Time
    {
        private static DateTime m_loadTime;
        private static TimeSpan m_deltaTime;
        private static DateTime m_lastFrameEnd;

        /// <summary>
        /// The total time in seconds up until the end of last frame.
        /// </summary>
        public static float time
        {
            get { return (float)(m_lastFrameEnd - m_loadTime).TotalSeconds; }
        }
        
        /// <summary>
        /// The time it took to render the last frame to render.
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
