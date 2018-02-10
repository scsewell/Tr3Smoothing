using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SoSmooth.Renderering
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
        /// Constructor. 
        /// </summary>
        public ShaderManager()
        {
            Logger.Info("Loading shaders...");

            Assembly assembly = Assembly.GetExecutingAssembly();
            
            List<string> shaderNames = new List<string>();
            Dictionary<string, VertexShader> vertShaders    = new Dictionary<string, VertexShader>();
            Dictionary<string, GeometryShader> geomShaders  = new Dictionary<string, GeometryShader>();
            Dictionary<string, FragmentShader> fragShaders  = new Dictionary<string, FragmentShader>();

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
                        if (!shaderNames.Contains(name))
                        {
                            shaderNames.Add(name);
                        }
                        string code = LoadResource(assembly, path);
                        
                        switch (type)
                        {
                            case VERT_EXTENTION: vertShaders.Add(name, new VertexShader(code)); break;
                            case GEOM_EXTENTION: geomShaders.Add(name, new GeometryShader(code)); break;
                            case FRAG_EXTENTION: fragShaders.Add(name, new FragmentShader(code)); break;
                        }
                    }
                }
            }

            // Create programs from shaders sharing a name
            m_shaderPrograms = new Dictionary<string, ShaderProgram>();
            List<Shader> shaders = new List<Shader>();

            foreach (string name in shaderNames)
            {
                shaders.Clear();
                
                VertexShader vertShader;
                if (vertShaders.TryGetValue(name, out vertShader))
                {
                    shaders.Add(vertShader);
                }

                GeometryShader geomShader;
                if (geomShaders.TryGetValue(name, out geomShader))
                {
                    shaders.Add(geomShader);
                }

                FragmentShader fragShader;
                if (fragShaders.TryGetValue(name, out fragShader))
                {
                    shaders.Add(fragShader);
                }

                Logger.Info("Creating program: " + name);
                ShaderProgram program = new ShaderProgram(shaders);
                m_shaderPrograms.Add(name, program);
                
                // Unload the shaders since we are done with them
                foreach (Shader shader in shaders)
                {
                    shader.Dispose();
                }
            }
            
            Logger.Info("Finished shader load");
        }

        /// <summary>
        /// Gets a shader program by name. The name must match the file names of a
        /// shader code resource bundled into thi assembly.
        /// </summary>
        /// <param name="name">The name of the shader program.</param>
        /// <returns>The shader program or null if none with the name exist.</returns>
        public ShaderProgram GetProgram(string name)
        {
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
