namespace SoSmooth
{
    /// <summary>
    /// Handles reading through a set of string for specific arguments.
    /// </summary>
    public class LineOptionParser
    {
        /// <summary>
        /// Checks if a given flag was specified.
        /// </summary>
        /// <param name="args">An array of arguments.</param>
        /// <param name="flag">The flag to search for, including any dashes.</param>
        /// <returns>True if the flag was in the provided arguments.</returns>
        public static bool HasFlag(string[] args, string flag)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == flag)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a value paired with an option flag.
        /// </summary>
        /// <param name="args">An array of arguments.</param>
        /// <param name="flag">The flag to search for, including any dashes.</param>
        /// <param name="value">The value paired with the flag, or null if the flag was not specified.</param>
        /// <returns>True if the flag was in the provided arguments.</returns>
        public static bool GetValueAsString(string[] args, string flag, out string value)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == flag)
                {
                    value = args[i + 1];
                    return true;
                }
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Gets a value paired with an option flag.
        /// </summary>
        /// <param name="args">An array of arguments.</param>
        /// <param name="flag">The flag to search for, including any dashes.</param>
        /// <param name="value">The value paired with the flag, or default value if not found.</param>
        /// <returns>True if the flag was in the provided arguments and the value was successfully parsed.</returns>
        public static bool GetValueAsInt(string[] args, string flag, out int value)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == flag)
                {
                    if (int.TryParse(args[i + 1], out value))
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
