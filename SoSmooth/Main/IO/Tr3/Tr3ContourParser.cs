using System;
using System.Collections.Generic;

namespace SoSmooth.IO.tr3
{
    /// <summary>
    /// Parses a tr3 file contour definition.
    /// </summary>
    public static class Tr3ContourParser
    {
        private static readonly string[] MAIN_SEPERATORS = new string[] { ":", "  " };
        private static readonly char[] SPACE_SEPERATOR = new char[] { ' ' };

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
        private const string VAL_START          = "s";
        private const string VAL_FINISH         = "f";
        private const string VAL_NEAREST        = "n";

        /// <summary>
        /// Takes a set of line definitions and gets the contours and
        /// connections corresponding to those lines.
        /// </summary>
        /// <param name="model">The model to add parsed content to.</param>
        /// <param name="lines">The set of line definitions.</param>
        public static void ParseLines(Model model, List<string> lines)
        {
            foreach (string line in lines)
            {
                // split the line into name, qualifiers, and description portions
                string[] mainSplit = line.Split(MAIN_SEPERATORS, StringSplitOptions.None);

                // get the name and description
                string name = mainSplit[0];
                string desc = mainSplit[2];
                
                // split the qualifiers section by spaces to separate arguments
                string[] args = mainSplit[1].Split(SPACE_SEPERATOR, StringSplitOptions.RemoveEmptyEntries);

                // if there are qualifiers given, check for relevant options
                bool isOpen = GetIsOpen(args);
                ContourType type = GetContourType(args);
                
                model.AddContour(new Contour(name, desc, type, isOpen));

                // add connections to other contours
                GetConnections(model, args, name, SourcePoint.Start);
                GetConnections(model, args, name, SourcePoint.Finish);
            }
        }

        /// <summary>
        /// Finds if a contour is open or closed.
        /// </summary>
        /// <param name="args">The arguments for a line.</param>
        /// <returns>True if the contour is open, and true if unspecified.</returns>
        private static bool GetIsOpen(string[] args)
        {
            string openClosed;
            if (args != null && LineOptionParser.GetValueAsString(args, ARG_OPEN_CLOSED, out openClosed))
            {
                return openClosed != VAL_CLOSED;
            }
            return true;
        }

        /// <summary>
        /// Finds the type of a contour.
        /// </summary>
        /// <param name="args">The arguments for a line.</param>
        /// <returns>The specified type, or straight if unspecified.</returns>
        private static ContourType GetContourType(string[] args)
        {
            string lineType;
            if (args != null && LineOptionParser.GetValueAsString(args, ARG_LINE_TYPE, out lineType))
            {
                switch (lineType)
                {
                    case VAL_STRAIGHT:      return ContourType.Straight;
                    case VAL_BEZIER:        return ContourType.Bezier;
                    case VAL_ELLIPTICAL:    return ContourType.Elliptical;
                    case VAL_TUBE:          return ContourType.Tubing;
                }
            }
            return ContourType.Straight;
        }
        
        /// <summary>
        /// Checks for connections to ther contours and adds them to the model.
        /// </summary>
        /// <param name="model">The model to add the connections to.</param>
        /// <param name="args">The line arguments that might specify connections.</param>
        /// <param name="name">The name of the contour these args correspond to.</param>
        /// <param name="sourcePoint">Whether to add start or finish connections.</param>
        private static void GetConnections(Model model, string[] args, string name, SourcePoint sourcePoint)
        {
            List<int> indicies;
            string flag = (sourcePoint == SourcePoint.Start) ? ARG_START : ARG_FINISH;

            if (args != null && LineOptionParser.GetValueIndicies(args, flag, out indicies))
            {
                foreach (int index in indicies)
                {
                    string targetName;
                    TargetPoint targetPoint;
                    float minZ = float.MinValue;
                    float maxZ = float.MaxValue;

                    if (GetTargetPoint(args[index], out targetPoint))
                    {
                        targetName = args[index + 1];
                    }
                    else
                    {
                        string[] range = args[index].Substring(1, args[index].Length - 2).Split(',');
                        ParseRange(range[0], ref minZ, true);
                        ParseRange(range[1], ref maxZ, false);

                        GetTargetPoint(args[index + 1], out targetPoint);
                        targetName = args[index + 2];
                    }

                    model.AddConnection(new Connection(name, targetName, sourcePoint, targetPoint, minZ, maxZ));
                }
            }
        }

        /// <summary>
        /// Gets the target point behavior from an argument.
        /// </summary>
        /// <param name="arg">The argument to parse.</param>
        /// <param name="targetPoint">The parsed value, set to start by default.</param>
        /// <returns>True if the parse was valid.</returns>
        private static bool GetTargetPoint(string arg, out TargetPoint targetPoint)
        {
            targetPoint = TargetPoint.Start;
            switch (arg)
            {
                case VAL_START:     targetPoint = TargetPoint.Start;    return true;
                case VAL_FINISH:    targetPoint = TargetPoint.Finish;   return true;
                case VAL_NEAREST:   targetPoint = TargetPoint.Nearest;  return true;
                default: return false;
            }
        }

        /// <summary>
        /// Parses a range value from the connection arguments.
        /// </summary>
        /// <param name="arg">The value to parse, including = and decimal point.</param>
        /// <param name="val">The variable to store the parsed value in.</param>
        /// <param name="isMin">True if parsing the lower bound of the range.</param>
        private static void ParseRange(string arg, ref float val, bool isMin)
        {
            bool isEqual = arg.StartsWith("=");
            if (arg.Length > (isEqual ? 1 : 0))
            {
                if (isEqual)
                {
                    val = float.Parse(arg.Substring(1, arg.Length - 1));
                }
                else
                {
                    val = float.Parse(arg) + (isMin ? 1 : -1);
                }
            }
        }
    }
}
