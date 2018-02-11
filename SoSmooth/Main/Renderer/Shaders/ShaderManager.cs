using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Loads shaders files embedded into the assembly.
    /// Shaders with the same file name are automatically combined into
    /// a single program.
    /// </summary>
    public class ShaderManager : Singleton<ShaderManager>
    {
        private const string VERT_EXTENTION = "vert";
        private const string GEOM_EXTENTION = "geom";
        private const string FRAG_EXTENTION = "frag";

        private Dictionary<string, ShaderProgram> m_shaderPrograms;
        
        /// <summary>
        /// Loads all shader programs.
        /// </summary>
        public void LoadShaders()
        {
            if (m_shaderPrograms == null)
            {
                Logger.Info("Loading shaders...");

                Assembly assembly = Assembly.GetExecutingAssembly();
                
                Dictionary<string, List<Shader>> nameToShaders = new Dictionary<string, List<Shader>>();

                // look through the embedded resources for any shaders
                foreach (string path in assembly.GetManifestResourceNames())
                {
                    string[] split = path.Split('.');

                    if (split.Length >= 2)
                    {
                        string name = split[split.Length - 2];
                        string type = split[split.Length - 1];

                        // shaders are indicated by file extention
                        if (type == VERT_EXTENTION ||
                            type == GEOM_EXTENTION ||
                            type == FRAG_EXTENTION)
                        {
                            string code = LoadResource(assembly, path);

                            List<Shader> shaders;
                            if (!nameToShaders.TryGetValue(name, out shaders))
                            {
                                shaders = new List<Shader>();
                                nameToShaders.Add(name, shaders);
                            }

                            switch (type)
                            {
                                case VERT_EXTENTION: shaders.Add(new VertexShader(code)); break;
                                case GEOM_EXTENTION: shaders.Add(new GeometryShader(code)); break;
                                case FRAG_EXTENTION: shaders.Add(new FragmentShader(code)); break;
                            }
                        }
                    }
                }

                // Create programs from shaders sharing a name
                m_shaderPrograms = new Dictionary<string, ShaderProgram>();

                foreach (KeyValuePair<string, List<Shader>> nameShaders in nameToShaders)
                {
                    Logger.Info("Creating program: " + nameShaders.Key);

                    ShaderProgram program = new ShaderProgram(nameShaders.Value);
                    m_shaderPrograms.Add(nameShaders.Key, program);

                    // Unload the shaders since we are done with them
                    foreach (Shader shader in nameShaders.Value)
                    {
                        shader.Dispose();
                    }
                }

                Logger.Info("Finished shader load");
            }
        }

        /// <summary>
        /// Gets a shader program by name. The name must match the file names of a
        /// shader code resource bundled into thi assembly.
        /// </summary>
        /// <param name="name">The name of the shader program.</param>
        /// <returns>The shader program or null if none with the name exist.</returns>
        public ShaderProgram GetProgram(string name)
        {
            LoadShaders();

            ShaderProgram program;
            if (!m_shaderPrograms.TryGetValue(name, out program))
            {
                Logger.Info("Could not find shader program with name: " + name);
            }
            return program;
        }

        /// <summary>
        /// Loads an embedded resource from the assembly.
        /// </summary>
        /// <param name="assembly">The target assembly.</param>
        /// <param name="path">The manifest resource name of the resourece to load.</param>
        /// <returns>The file as a string.</returns>
        private static string LoadResource(Assembly assembly, string path)
        {
            Logger.Info("Loading shader: " + path);

            using (Stream resourceStream = assembly.GetManifestResourceStream(path))
            {
                if (resourceStream != null)
                {
                    using (StreamReader reader = new StreamReader(resourceStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            return null;
        }
    }
}
