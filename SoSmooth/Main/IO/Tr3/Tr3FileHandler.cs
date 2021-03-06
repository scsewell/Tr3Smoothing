﻿using System.IO;

namespace SoSmooth.IO.tr3
{
    /// <summary>
    /// Manages the parsing and writing of tr3 files.
    /// </summary>
    public class Tr3FileHandler : Singleton<Tr3FileHandler>
    {
        /// <summary>
        /// Reads a tr3 file at a specified path, parses the information, and returns it.
        /// </summary>
        /// <param name="filePath">A full path to a valid tr3 file.</param>
        /// <returns>A model containing the parsed data.</returns>
        public Model Read(string filePath)
        {
            string fullPath = System.Environment.ExpandEnvironmentVariables(filePath);

            Logger.Info("Parsing .tr3 file at path \"" + fullPath + "\"");

            FileInfo fileInfo = new FileInfo(fullPath);
            if (!fileInfo.Exists)
            {
                Logger.Error("File \"" + fullPath + "\" does not exist!");
                return null;
            }
            
            Tr3Parser parser = new Tr3Parser(fileInfo);
            Model model = parser.Model;

            Logger.Info("Finished parsing file");
            return model;
        }
    }
}
