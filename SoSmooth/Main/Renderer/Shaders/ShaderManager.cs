using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using SoSmooth.Rendering;

namespace SoSmooth
{
    /// <summary>
    /// Loads shader files embedded in assemblies. Shaders with the same file
    /// name are automatically combined into a single program. Files ending in .glinc
    /// will be added to all shader programs, allowing for easily writing shared
    /// functionality.
    /// </summary>
    public class ShaderManager : Singleton<ShaderManager>
    {
        /// <summary>
        /// A basic unlit shader.
        /// </summary>
        public static readonly string SHADER_UNLIT = "unlit";

        /// <summary>
        /// A basic lit shader.
        /// </summary>
        public static readonly string SHADER_LIT = "lit";

        /// <summary>
        /// A shader used when there is a graphics error.
        /// </summary>
        private static readonly string SHADER_ERROR = "error";

        private const string GLSL_VERSION = "330";

        // The extentions of shader source files.
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
        
        private Dictionary<Assembly, string[]> m_assemblyToResName = new Dictionary<Assembly, string[]>();
        private Dictionary<string, ShaderProgram> m_shaderPrograms = new Dictionary<string, ShaderProgram>();

        /// <summary>
        /// Loads all shader programs embedded in assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to search for shader files in.</param>
        public void LoadShaders(params Assembly[] assemblies)
        {
            LoadShaders(Assembly.GetExecutingAssembly());

            foreach (Assembly assembly in assemblies)
            {
                LoadShaders(assembly);
            }
        }

        /// <summary>
        /// Loads all shader programs embedded in an assembly.
        /// </summary>
        /// <param name="assemblies">The assembly to search for shader files in.</param>
        public void LoadShaders(Assembly assembly)
        {
            if (!m_assemblyToResName.ContainsKey(assembly))
            {
                m_assemblyToResName.Add(assembly, assembly.GetManifestResourceNames());

                Logger.Info($"Loading shaders from assembly: {assembly.FullName}");

                // Create programs from shaders sharing a name
                foreach (KeyValuePair<string, List<Shader>> nameShaders in GetShaderSources(assembly))
                {
                    string name = nameShaders.Key;
                    List<Shader> shaders = nameShaders.Value;

                    if (shaders.Any(s => !s.IsValid))
                    {
                        Logger.Error($"Can't create program \"{name}\" as source shaders are not valid!");
                        shaders.ForEach(s => s.Dispose());
                        continue;
                    }

                    Logger.Info("Creating program: " + name);
                    ShaderProgram program = new ShaderProgram(name, shaders);

                    if (program.IsValid)
                    {
                        m_shaderPrograms.Add(name, program);
                    }
                    
                    shaders.ForEach(s => s.Dispose());
                }
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
            if (name == null || !m_shaderPrograms.TryGetValue(name, out program))
            {
                Logger.Error($"Could not find shader program \"{name ?? "null"}\"!");
                program = m_shaderPrograms[SHADER_ERROR];
                return false;
            }
            if (!program.IsValid)
            {
                Logger.Error($"Requested shader program \"{name}\" didn't compile successfuly!");
                program = m_shaderPrograms[SHADER_ERROR];
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
        
        /// <summary>
        /// Gets all shaders in an assembly.
        /// </summary>
        /// <param name="assembly">The assembly to load shaders from.</param>
        private Dictionary<string, List<Shader>> GetShaderSources(Assembly assembly)
        {
            string common = GetCommon(assembly);

            Dictionary<string, List<Shader>> nameToShaders = new Dictionary<string, List<Shader>>();
            
            // look through the embedded resources for any shaders
            foreach (string path in m_assemblyToResName[assembly])
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
            return nameToShaders;
        }

        /// <summary>
        /// Gets the common shader file content as a string.
        /// </summary>
        /// <param name="assembly">The source assembly of the resource names.</param>
        private string GetCommon(Assembly assembly)
        {
            StringBuilder sb = new StringBuilder("#version " + GLSL_VERSION + Environment.NewLine);

            // get the common shader files
            foreach (string path in m_assemblyToResName[assembly])
            {
                string[] split = path.Split('.');
                if (split.Length >= 2)
                {
                    if (split[split.Length - 1] == INC_EXTENTION)
                    {
                        sb.Append(LoadResource(assembly, path));
                        sb.Append(Environment.NewLine);
                    }
                }
            }
            return sb.ToString();
        }
    }
}
