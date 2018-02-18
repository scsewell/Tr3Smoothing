using System;
using System.Collections.Generic;
using System.IO;

namespace SoSmooth.IO.tr3
{
    /// <summary>
    /// Parses a tr3 file slice definition.
    /// </summary>
    public static class Tr3SliceParser
    {
        private static readonly char[] SPACE_SEPERATOR = new char[] { ' ' };
        private static readonly string IMAGE_PATH = "IMAGE_PATH";

        /// <summary>
        /// Takes a set of line definitions and gets the slices
        /// corresponding to those lines.
        /// </summary>
        /// <param name="model">The model to add parsed content to.</param>
        /// <param name="lines">The set of slice definitions.</param>
        public static void ParseLines(Model model, List<string> lines)
        {
            // get the base directory path
            string imgDir = model.File.Directory.FullName;

            int count = 0;
            foreach (string line in lines)
            {
                if (line.StartsWith(IMAGE_PATH))
                {
                    imgDir = Path.Combine(imgDir, line.Substring(line.IndexOf(' ') + 1));
                }
                else
                {
                    // split the options in the line
                    string[] split = line.Split(SPACE_SEPERATOR, StringSplitOptions.RemoveEmptyEntries);

                    string imgPath = Path.Combine(imgDir, split[1]);

                    float z = 0;

                    // z coord is specified
                    if (split.Length > 2)
                    {
                        z = float.Parse(split[2]);
                    }

                    float rotation = 0;
                    float offsetX = 0;
                    float offsetY = 0;

                    // rotation and offset specified
                    if (split.Length > 3)
                    {
                        rotation  = float.Parse(split[3]);
                        offsetX   = float.Parse(split[4]);
                        offsetY   = float.Parse(split[5]);
                    }

                    model.AddSlice(new Slice(count, imgPath, z, rotation, offsetX, offsetY));

                    count++;
                }
            }
        }
    }
}
