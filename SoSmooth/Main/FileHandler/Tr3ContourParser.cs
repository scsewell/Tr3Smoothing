using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoSmooth
{
    /// <summary>
    /// Parses a tr3 file contour definition.
    /// </summary>
    public class Tr3ContourParser
    {
        private const char NAME_SEPERATOR       = ':';
        private const string ARG_CLOSED         = "-c";

        public static List<Contour> ParseLines(List<string> lines)
        {
            List<Contour> contours = new List<Contour>();

            foreach (string line in lines)
            {
                string name = line.Substring(line.IndexOf(NAME_SEPERATOR));

                Contour contour = new Contour(name, null);
                contours.Add(contour);
            }

            return contours;
        }
    }
}
