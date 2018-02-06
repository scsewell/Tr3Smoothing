using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoSmooth
{
    public static class Tr3DataParser
    {
        private static readonly char[] SPACE_SEPERATOR = new char[] { ' ' };

        /// <summary>
        /// Takes a set of contour point definitions and gets the points
        /// corresponding to those lines.
        /// </summary>
        /// <param name="model">The model to add parsed content to.</param>
        /// <param name="lines">The set of point definitions.</param>
        public static void ParseLines(Model model, List<string> lines)
        {
            float z = float.MinValue;

            foreach (string line in lines)
            {
                // split the line into name and coordinates
                string[] split = line.Split(SPACE_SEPERATOR, StringSplitOptions.RemoveEmptyEntries);

                string name = split[0];
                float x = float.Parse(split[1]);
                float y = float.Parse(split[2]);

                // only update the z value if it is specified on this line
                if (split.Length > 3)
                {
                    z = float.Parse(split[3]);
                }

                // add the point to the contour with the given name
                model.GetContour(name).AddPoint(new Vector3(x, y, z));
            }
        }
    }
}
