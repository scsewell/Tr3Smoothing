using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoSmooth.Meshes;

namespace SoSmooth
{
    public class SmoothingManager : Singleton<SmoothingManager>
    {
        public List<Mesh> m_meshes = new List<Mesh>();
    }
}
