using System.Collections.Generic;
using System.IO;

namespace SoSmooth.Tr3
{
    /// <summary>
    /// Parses tr3 files.
    /// </summary>
    public class Tr3Parser
    {
        // special names and characters in tr3 file definition
        private const string LINE_COMMENT           = ";";

        private const string SECTION_LINE           = "LINES";
        private const string SECTION_SCALE          = "SCALE";
        private const string SECTION_XY_CALC        = "XY_CALC";
        private const string SECTION_Z_CALC         = "Z_CALC";
        private const string SECTION_SLICES         = "SLICES";
        private const string SECTION_JOINS          = "JOINS";
        private const string SECTION_CAPS           = "CAPS";
        private const string SECTION_SUBSETS        = "SUBSETS";
        private const string SECTION_EXTERNAL       = "EXTERNAL";
        private const string SECTION_VOLUMES        = "VOLUMES";
        private const string SECTION_MATERIALS      = "MATERIALS";
        private const string SECTION_SPRINGS        = "SPRINGS";
        private const string SECTION_CONC_LOADS     = "CONC_LOADS";
        private const string SECTION_LABELS         = "LABELS";
        private const string SECTION_VIEWPOINTS     = "VIEWPOINTS";
        private const string SECTION_DATA           = "DATA";
        private const string SECTION_IMAGES         = "IMAGES";
        private const string SECTION_MARKED_VOXELS  = "MARKED_VOXELS";
        private const string SECTION_COSTS          = "COSTS";
        private const string SECTION_END            = "END";

        // relevant section lines
        private string m_xy_calc = null;
        private string m_z_calc = null;
        private string m_scale = null;
        private List<string> m_lines = new List<string>();
        private List<string> m_slices = new List<string>();
        private List<string> m_data = new List<string>();
        private List<List<string>> m_subsets = new List<List<string>>();

        /// <summary>
        /// Gets the parsed model.
        /// </summary>
        public Model Model { get { return m_model; } }
        private Model m_model;

        /// <summary>
        /// Constructor that parses a tr3 file.
        /// </summary>
        /// <param name="file">The tr3 file to parse.</param>
        public Tr3Parser(FileInfo file)
        {
            // extract the file's content
            using (StreamReader fs = new StreamReader(file.FullName))
            {
                GetSectionLines(fs);
            }

            // parse the content
            m_model = new Model(file);
            Tr3SliceParser.ParseLines(m_model, m_slices);
            Tr3ContourParser.ParseLines(m_model, m_lines);
            Tr3DataParser.ParseLines(m_model, m_data);
            m_model.Initialize();
        }

        /// <summary>
        /// Gets the lines for all sections and stores them to the appropriate fields.
        /// </summary>
        /// <param name="fstream">A stream starting from the begining of a tr3 file.</param>
        private void GetSectionLines(StreamReader fstream)
        {
            List<string> lineGroup = null;
            List<List<string>> currentSubsection = null;
            int lineNumber = -1;

            while (!fstream.EndOfStream)
            {
                // read a line from the stream
                string line = ReadLine(fstream, ref lineNumber);

                // ignore line comments
                if (line.StartsWith(LINE_COMMENT))  { continue; }
                // line is the begining of a specific section
                else if (line == SECTION_LINE)      { lineGroup = m_lines; }
                else if (line == SECTION_SLICES)    { lineGroup = m_slices; }
                else if (line == SECTION_DATA)      { lineGroup = m_data; }
                else if (line == SECTION_SUBSETS)   { currentSubsection = m_subsets; }
                else if (line == SECTION_XY_CALC)   { m_xy_calc = ReadLine(fstream, ref lineNumber); }
                else if (line == SECTION_Z_CALC)    { m_z_calc = ReadLine(fstream, ref lineNumber); }
                else if (line == SECTION_SCALE)     { m_scale = ReadLine(fstream, ref lineNumber); }
                // line is the end of a section or subsection
                else if (line.StartsWith(SECTION_END))
                {
                    if (lineGroup == null)
                    {
                        currentSubsection = null;
                    }
                    else if (currentSubsection != null)
                    {
                        currentSubsection.Add(lineGroup);
                    }
                    lineGroup = null;
                }
                else // line belongs to content of a section
                {
                    // create a new subsection if needed
                    if (currentSubsection != null && lineGroup == null)
                    {
                        lineGroup = new List<string>();
                    }
                    // add the current line if relevant
                    if (lineGroup != null)
                    {
                        lineGroup.Add(line);
                    }
                }
            }
        }

        /// <summary>
        /// Reads a single line from a stream.
        /// </summary>
        /// <param name="stream">The input stream</param>
        /// <param name="lineNumber">The current line number in the stream.</param>
        /// <returns>A line with leading/following whitespace removed.</returns>
        private string ReadLine(StreamReader stream, ref int lineNumber)
        {
            lineNumber++;
            return stream.ReadLine().Trim();
        }
    }
}
