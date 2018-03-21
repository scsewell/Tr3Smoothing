using System;
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
    /// <summary>
    /// Extracts content from a VRML scene.
    /// </summary>
    public class VrmlExtractor : Singleton<VrmlExtractor>
    {
        /// <summary>
        /// Reads a VRML file at a specified path, parses the information, and returns the contained models.
        /// </summary>
        /// <param name="filePath">A full path to a valid VRML file.</param>
        /// <returns>The extracted models.</returns>
        public List<Mesh> Read(string filePath)
        {
            string fullPath = Environment.ExpandEnvironmentVariables(filePath);

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
            Dictionary<int, uint> indexToVertexIndex = new Dictionary<int, uint>();
            List<uint> triangleIndices = new List<uint>();

            // Do a depth first search on the VRML scene to find all meshes
            Stack<GroupingNode> toVisit = new Stack<GroupingNode>();
            toVisit.Push(scene.Root);

            HashSet<Node> visited = new HashSet<Node>(toVisit);

            string currentDescription = "DefaultName";
            while (toVisit.Count > 0)
            {
                GroupingNode node = toVisit.Pop();

                // keep track of the most recent description in the heirarchy to name child meshes using
                if (node is AnchorNode)
                {
                    currentDescription = (node as AnchorNode).Description;
                }

                foreach (Node child in node.Children)
                {
                    // If the child node may have children nodes we must visit it if we have not already
                    if (child is GroupingNode)
                    {
                        if (!visited.Contains(child))
                        {
                            toVisit.Push(child as GroupingNode);
                            visited.Add(child);
                        }
                    }
                    // If the node is a mesh extract it
                    if (child is ShapeNode)
                    {
                        ShapeNode shape = child as ShapeNode;

                        if (shape.Geometry.Node is IndexedFaceSetNode)
                        {
                            IndexedFaceSetNode indexFaceSet = shape.Geometry.Node as IndexedFaceSetNode;
                            CoordinateNode coords = indexFaceSet.Coord.Node as CoordinateNode;

                            // Must have at least one index given to be a mesh
                            if (indexFaceSet.CoordIndex.Length == 0)
                            {
                                continue;
                            }

                            // The indicies in the mesh we are building may not match those in the file if
                            // not all points from the coords are used, so maintain a mapping from original
                            // index in the file to the corresponding index in the mesh currently being built.
                            builder.Clear();
                            indexToVertexIndex.Clear();
                            triangleIndices.Clear();
                            
                            foreach (SFInt32 index in indexFaceSet.CoordIndex)
                            {
                                if (index < 0)
                                {
                                    builder.AddTriangle(new Triangle(triangleIndices[0], triangleIndices[1], triangleIndices[2]));
                                    triangleIndices.Clear();
                                }
                                else
                                {
                                    uint i;
                                    if (!indexToVertexIndex.TryGetValue(index, out i))
                                    {
                                        SFVec3f pos = coords.Point.GetValue(index);

                                        i = (uint)builder.VertexCount;
                                        indexToVertexIndex.Add(index.Value, i);
                                        builder.AddVertex(new Vector3(pos.X, pos.Y, pos.Z));
                                    }
                                    triangleIndices.Add(i);
                                }
                            }

                            // Generate the mesh and calculate the vertex normals
                            Mesh mesh = builder.CreateMesh(currentDescription);
                            meshes.Add(mesh);
                            Logger.Info($"Found Mesh: {mesh}");
                        }
                    }
                }
            }

            Logger.Info("Finished parsing file");
            return meshes;
        }
    }
}
