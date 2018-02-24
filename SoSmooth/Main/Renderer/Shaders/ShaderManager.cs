using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using SoSmooth.Rendering;

namespace SoSmooth
{
    /// <summary>
    /// Loads shaders files embedded into the assembly. Shaders with the same file
    /// name are automatically combined into a single program. Files ending in .glinc
    /// will be added to all shader programs, allowing for easily writing shared
    /// functionality.
    /// </summary>
    public class ShaderManager : Singleton<ShaderManager>
    {
        // The file extentions of shaders in the solution.
        private const string VERT_EXTENTION = "vert";
        private const string GEOM_EXTENTION = "geom";
        private const string FRAG_EXTENTION = "frag";
        private const string INC_EXTENTION  = "glinc";

        private static readonly string[] SHADER_EXTENTIONS = new string[] 
        {
            VERT_EXTENTION,
            GEOM_EXTENTION,
            FRAG_EXTENTION,
        };

        /// <summary>
        /// A basic unlit shader.
        /// </summary>
        public static readonly string SHADER_UNLIT  = "unlit";
        /// <summary>
        /// A basic lit shader.
        /// </summary>
        public static readonly string SHADER_LIT    = "lit";
        
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
                string[] resourceNames = assembly.GetManifestResourceNames();
                
                StringBuilder commonBuilder = new StringBuilder();

                // get the common shader files
                foreach (string path in resourceNames)
                {
                    string[] split = path.Split('.');
                    if (split.Length >= 2)
                    {
                        if (split[split.Length - 1] == INC_EXTENTION)
                        {
                            commonBuilder.Append(LoadResource(assembly, path));
                            commonBuilder.Append(System.Environment.NewLine);
                        }
                    }
                }
                string common = commonBuilder.ToString();

                Dictionary<string, List<Shader>> nameToShaders = new Dictionary<string, List<Shader>>();

                // look through the embedded resources for any shaders
                foreach (string path in resourceNames)
                {
                    string[] split = path.Split('.');

                    if (split.Length >= 2)
                    {
                        string name = split[split.Length - 2];
                        string extention = split[split.Length - 1];

                        // shaders are indicated by file extention
                        if (SHADER_EXTENTIONS.Any(e => extention == e))
                        {
                            string code = LoadResource(assembly, path);

                            List<Shader> shaders;
                            if (!nameToShaders.TryGetValue(name, out shaders))
                            {
                                shaders = new List<Shader>();
                                nameToShaders.Add(name, shaders);
                            }

                            switch (extention)
                            {
                                case VERT_EXTENTION: shaders.Add(new VertexShader(common + code)); break;
                                case GEOM_EXTENTION: shaders.Add(new GeometryShader(common + code)); break;
                                case FRAG_EXTENTION: shaders.Add(new FragmentShader(common + code)); break;
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
        /// shader code resource bundled into this assembly.
        /// </summary>
        /// <param name="name">The name of the shader program.</param>
        /// <param name="program">The shader program, or null if not found.</param>
        /// <returns>True if the shader program was successfully found.</returns>
        public bool GetProgram(string name, out ShaderProgram program)
        {
            program = null;
            if (m_shaderPrograms == null)
            {
                Logger.Error($"Tried to get shader program \"{name}\" before shaders have been loaded!");
                return false;
            }
            if (!m_shaderPrograms.TryGetValue(name, out program))
            {
                Logger.Info($"Could not find shader program \"{name}\"");
                return false;
            }
            return true;
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
