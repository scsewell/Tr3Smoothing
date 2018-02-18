using System.IO;
using System.Collections.Generic;
using SoSmooth.IO.Vrml.Tokenizer;
using SoSmooth.IO.Vrml.Parser;
using SoSmooth.IO.Vrml.Fields;
using SoSmooth.IO.Vrml.Nodes;
using SoSmooth.Meshes;
using OpenTK;

namespace SoSmooth.IO.Vrml
{
    public class VrmlExtractor : Singleton<VrmlExtractor>
    {
        /// <summary>
        /// Reads a VRML file at a specified path, parses the information, and returns the contained models.
        /// </summary>
        /// <param name="filePath">A full path to a valid VRML file.</param>
        /// <returns>The parsed models.</returns>
        public List<Mesh> Read(string filePath)
        {
            string fullPath = System.Environment.ExpandEnvironmentVariables(filePath);

            Logger.Info("Parsing .wrl file at path \"" + fullPath + "\"");

            FileInfo fileInfo = new FileInfo(fullPath);
            if (!fileInfo.Exists)
            {
                Logger.Error("File \"" + fullPath + "\" does not exist!");
                return null;
            }

            // parse the Vrml file
            VrmlScene scene;
            using (var stream = new StreamReader(filePath))
            {
                VrmlParser parser = new VrmlParser(new VrmlTokenizer(stream));
                scene = parser.Parse();
            }

            List<Mesh> meshes = new List<Mesh>();
            MeshBuilder builder = new MeshBuilder();
            Dictionary<int, ushort> indexToVertexIndex = new Dictionary<int, ushort>();
            List<ushort> triangleIndices = new List<ushort>();

            foreach (Node node in scene.Root.Children)
            {
                if (node is AnchorNode)
                {
                    AnchorNode anchor = node as AnchorNode;
                    foreach (Node child in anchor.Children)
                    {
                        if (child is ShapeNode)
                        {
                            ShapeNode shape = child as ShapeNode;

                            if (shape.Geometry.Node is IndexedFaceSetNode)
                            {
                                builder.Clear();
                                indexToVertexIndex.Clear();
                                triangleIndices.Clear();

                                IndexedFaceSetNode indexFaceSet = shape.Geometry.Node as IndexedFaceSetNode;
                                CoordinateNode coords = indexFaceSet.Coord.Node as CoordinateNode;
                                
                                foreach (SFInt32 index in indexFaceSet.CoordIndex)
                                {
                                    if (index < 0)
                                    {
                                        builder.AddTriangle(new Triangle(triangleIndices[0], triangleIndices[1], triangleIndices[2]));
                                        triangleIndices.Clear();
                                    }
                                    else
                                    {
                                        ushort i;
                                        if (!indexToVertexIndex.TryGetValue(index, out i))
                                        {
                                            SFVec3f pos = coords.Point.GetValue(index);
                                            Vertex v = new Vertex(new Vector3(pos.X, pos.Y, pos.Z));
                                            
                                            i = (ushort)builder.VertexCount;
                                            indexToVertexIndex.Add(index.Value, i);
                                            builder.AddVertex(v);
                                        }
                                        triangleIndices.Add(i);
                                    }
                                }

                                Mesh mesh = builder.CreateMesh(anchor.Description, true, true);
                                mesh.RecalculateNormals();
                                meshes.Add(mesh);
                                Logger.Info($"Found Mesh: {mesh}");
                            }
                        }
                    }
                }
            }

            Logger.Info("Finished parsing file");
            return meshes;
        }
    }
}
