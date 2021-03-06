﻿using System.Collections.Generic;

namespace SoSmooth.IO
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
        /// <param name="flag">The flag to search for.</param>
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
        /// <param name="flag">The flag to search for.</param>
        /// <param name="value">The value paired with the flag, or null if the flag was not specified.</param>
        /// <returns>True if the flag was in the provided arguments.</returns>
        public static bool GetValueAsString(string[] args, string flag, out string value)
        {
            value = null;
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == flag)
                {
                    value = args[i + 1];
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a value paired with an option flag.
        /// </summary>
        /// <param name="args">An array of arguments.</param>
        /// <param name="flag">The flag to search for.</param>
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

        /// <summary>
        /// Gets the indices of all values paired with a repeatable option flag.
        /// </summary>
        /// <param name="args">An array of arguments.</param>
        /// <param name="flag">The flag to search for.</param>
        /// <param name="values">The indicies of all strings following the flag, or empty if the flag was not specified.</param>
        /// <returns>True if the flag was in the provided arguments.</returns>
        public static bool GetValueIndicies(string[] args, string flag, out List<int> indicies)
        {
            indicies = new List<int>();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == flag)
                {
                    indicies.Add(i + 1);
                }
            }
            return indicies.Count > 0;
        }
    }
}
