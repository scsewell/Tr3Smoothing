namespace SoSmooth
{
    /// <summary>
    /// Handles reading through the command line options for specific arguments.
    /// </summary>
    public class LineOptionParser
    {
        private string[] m_args;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="args">The command line arguments passed into the application.</param>
        public LineOptionParser(string[] args)
        {
            m_args = args;
        }

        /// <summary>
        /// Checks if a given flag was specified.
        /// </summary>
        /// <param name="flag">The flag to search for, including any dashes.</param>
        /// <returns>True if the flag was in the provided arguments.</returns>
        public bool HasFlag(string flag)
        {
            for (int i = 0; i < m_args.Length; i++)
            {
                if (m_args[i] == flag)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a value paired with an option flag.
        /// </summary>
        /// <param name="flag">The flag to search for, including any dashes.</param>
        /// <param name="value">The value paired with the flag, or null if the flag was not specified.</param>
        /// <returns>True if the flag was in the provided arguments.</returns>
        public bool GetValueAsString(string flag, out string value)
        {
            for (int i = 0; i < m_args.Length - 1; i++)
            {
                if (m_args[i] == flag)
                {
                    value = m_args[i + 1];
                    return true;
                }
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Gets a value paired with an option flag.
        /// </summary>
        /// <param name="flag">The flag to search for, including any dashes.</param>
        /// <param name="value">The value paired with the flag, or default value if not found.</param>
        /// <returns>True if the flag was in the provided arguments and the value was successfully parsed.</returns>
        public bool GetValueAsInt(string flag, out int value)
        {
            for (int i = 0; i < m_args.Length - 1; i++)
            {
                if (m_args[i] == flag)
                {
                    if (int.TryParse(m_args[i + 1], out value))
                    {
                        return true;
                    }
                }
            }
            value = default(int);
            return false;
        }
    }
}
