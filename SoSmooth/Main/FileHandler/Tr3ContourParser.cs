using System;
using System.Collections.Generic;

namespace SoSmooth
{
    /// <summary>
    /// Parses a tr3 file contour definition.
    /// </summary>
    public class Tr3ContourParser
    {
        private static readonly string[] MAIN_SEPERATORS = new string[] { ":", "  " };
        private static readonly char[] SPACE_SEPERATOR  = new char[] { ' ' };

        // relevant qualifier names and values given in tr3 file definition
        private const string ARG_OPEN_CLOSED    = "-op";
        private const string VAL_CLOSED         = "CL";
        private const string VAL_OPEN           = "OP";

        private const string ARG_LINE_TYPE      = "-L";
        private const string VAL_STRAIGHT       = "S";
        private const string VAL_BEZIER         = "B";
        private const string VAL_ELLIPTICAL     = "E";
        private const string VAL_TUBE           = "T";

        private const string ARG_START          = "-st";
        private const string ARG_FINISH         = "-f";

        /// <summary>
        /// Takes a set of line definitions and returns contours corresponding to those lines.
        /// </summary>
        /// <param name="lines">The set of line definitions</param>
        /// <returns>A set of new contours.</returns>
        public static List<Contour> ParseLines(List<string> lines)
        {
            List<Contour> contours = new List<Contour>();
            Dictionary<string, Contour> nameToContour = new Dictionary<string, Contour>();
            
            foreach (string line in lines)
            {
                // split the line into name, qualifiers, and description portions
                string[] mainSplit = line.Split(MAIN_SEPERATORS, StringSplitOptions.RemoveEmptyEntries);

                // get the name and description
                string name = mainSplit[0];
                string desc = mainSplit[mainSplit.Length - 1];

                // assume the default options
                bool isOpen = true;
                ContourType type = ContourType.Straight;

                // if there are qualifiers given, check for relevant options
                if (mainSplit.Length > 2)
                {
                    // split the qualifiers section by spaces to separate arguments
                    string[] split = mainSplit[1].Split(SPACE_SEPERATOR, StringSplitOptions.RemoveEmptyEntries);

                    string openClosed;
                    if (LineOptionParser.GetValueAsString(split, ARG_OPEN_CLOSED, out openClosed))
                    {
                        isOpen = openClosed != VAL_CLOSED;
                    }

                    string lineType;
                    if (LineOptionParser.GetValueAsString(split, ARG_LINE_TYPE, out lineType))
                    {
                        switch (lineType)
                        {
                            case VAL_STRAIGHT:      type = ContourType.Straight;    break;
                            case VAL_BEZIER:        type = ContourType.Bezier;      break;
                            case VAL_ELLIPTICAL:    type = ContourType.Elliptical;  break;
                            case VAL_TUBE:          type = ContourType.Tubing;      break;
                        }
                    }
                }

                Contour contour = new Contour(name, desc, type, isOpen);
                contours.Add(contour);

                if (nameToContour.ContainsKey(name))
                {
                    Logger.Error("Multiple contours share the name name! This behavior is undefined.");
                }
                else
                {
                    nameToContour.Add(name, contour);
                }
            }

            return contours;
        }
    }
}
