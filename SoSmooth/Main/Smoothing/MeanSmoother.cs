﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using SoSmooth.Meshes;
using OpenTK;

namespace SoSmooth
{
    /// <summary>
    /// A smoothing algorithm that iterates over a mesh, using the average normal of a
    /// triangle and its neighbors to adjust the vertex positions. This algorithm is
    /// implemented as described in "Mesh Smoothing via Mean and Median Filtering Applied
    /// to Face Normals".
    /// </summary>
    public class MeanSmoother : ISmoother
    {
        /// <summary>
        /// The maximum number of iterations that may be performed at once.
        /// </summary>
        public const int MAX_ITERATIONS = 40;

        private static int m_iterations = 10;
        private static float m_strength = 1.0f;

        /// <summary>
        /// The number of smoothing iterations performed when smoothing.
        /// The value is restricted to the range [1, MAX_ITERATIONS].
        /// </summary>
        public static int Iterations
        {
            get { return m_iterations; }
            set { m_iterations = MathHelper.Clamp(value, 1, MAX_ITERATIONS); }
        }

        /// <summary>
        /// How strong the smoothing effect is each iteration.
        /// The value is restricted to the range [0, 1].
        /// </summary>
        public static float Strength
        {
            get { return m_strength; }
            set { m_strength = MathHelper.Clamp(value, 0.0f, 1.0f); }
        }

        private readonly Stopwatch m_stopwatch = new Stopwatch();
        private Vector3[] m_vertices;
        private Triangle[] m_triangles;

        private int[][] m_vertToNeighbors;
        private int[][] m_triToNeighbors;

        private float[] m_triAreas;
        private Vector3[] m_triNormals;
        private Vector3[] m_triCenteroids;
        private Vector3[] m_triNeighborhoodNormals;

        /// <summary>
        /// Smooths a mesh.
        /// </summary>
        /// <param name="verts">The vertices of the mesh.</param>
        /// <param name="tris">The triangles of the mesh.</param>
        /// <returns>The smoothed vertex positions.</returns>
        public Vector3[] Smooth(Vector3[] verts, Triangle[] tris)
        {
            m_stopwatch.Restart();

            m_vertices = verts.Clone() as Vector3[];
            m_triangles = tris;

            CreateBuffers();
            ComputeVertexNeighborTris();
            ComputeTriangleNeighbors();
            
            // do iterations
            for (int i = 0; i < m_iterations; i++)
            {
                SmoothIteration();
            }

            // report completion
            m_stopwatch.Stop();
            Logger.Info($"Finished smoothing {verts.Length} vertices in {m_stopwatch.ElapsedMilliseconds} milliseconds");

            return m_vertices;
        }

        /// <summary>
        /// Ensures the buffers used are able to fit all mesh data.
        /// </summary>
        private void CreateBuffers()
        {
            if (m_triAreas == null)
            {
                m_vertToNeighbors = new int[m_vertices.Length][];
                m_triToNeighbors = new int[m_triangles.Length][];

                m_triAreas = new float[m_triangles.Length];
                m_triNormals = new Vector3[m_triangles.Length];
                m_triCenteroids = new Vector3[m_triangles.Length];
                m_triNeighborhoodNormals = new Vector3[m_triangles.Length];
            }
            else
            {
                Array.Resize(ref m_vertToNeighbors, m_vertices.Length);
                Array.Resize(ref m_triToNeighbors, m_triangles.Length);

                Array.Resize(ref m_triAreas, m_triangles.Length);
                Array.Resize(ref m_triNormals, m_triangles.Length);
                Array.Resize(ref m_triCenteroids, m_triangles.Length);
                Array.Resize(ref m_triNeighborhoodNormals, m_triangles.Length);
            }
        }

        /// <summary>
        /// Computes the indices in the triangle array for the triangles
        /// neighboring each vertex in the mesh.
        /// </summary>
        private void ComputeVertexNeighborTris()
        {
            // use a hash set to avoid adding the same triangle as a neighbor multiple times
            HashSet<int>[] vertToNeighbors = new HashSet<int>[m_vertices.Length];
            for (int i = 0; i < vertToNeighbors.Length; i++)
            {
                vertToNeighbors[i] = new HashSet<int>();
            }
            
            // each triangle adds itself to each of its vertices as a neighbor
            for (int i = 0; i < m_triangles.Length; i++)
            {
                Triangle tri = m_triangles[i];
                vertToNeighbors[tri.index0].Add(i);
                vertToNeighbors[tri.index1].Add(i);
                vertToNeighbors[tri.index2].Add(i);
            }

            // convert the hash sets to an array for faster iteration
            for (int i = 0; i < m_vertToNeighbors.Length; i++)
            {
                m_vertToNeighbors[i] = vertToNeighbors[i].ToArray();
            }
        }

        /// <summary>
        /// Computes the indices in the triangle array for the triangles
        /// adjacent to each triangle.
        /// </summary>
        private void ComputeTriangleNeighbors()
        {
            // clear old values in the array
            for (int i = 0; i < m_triToNeighbors.Length; i++)
            {
                m_triToNeighbors[i] = null;
            }

            // compute the neighborhood for each triangle adjacent to each vertex, if
            // it has not already been done.
            HashSet<int> neighborhood = new HashSet<int>();
            for (int vertIndex = 0; vertIndex < m_vertices.Length; vertIndex++)
            {
                foreach (int triIndex in m_vertToNeighbors[vertIndex])
                {
                    if (m_triToNeighbors[triIndex] == null)
                    {
                        neighborhood.Clear();

                        Triangle tri = m_triangles[triIndex];
                        foreach (int i in m_vertToNeighbors[tri.index0])
                        {
                            neighborhood.Add(i);
                        }
                        foreach (int i in m_vertToNeighbors[tri.index1])
                        {
                            neighborhood.Add(i);
                        }
                        foreach (int i in m_vertToNeighbors[tri.index2])
                        {
                            neighborhood.Add(i);
                        }

                        m_triToNeighbors[triIndex] = neighborhood.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Does a single smoothing iteration.
        /// </summary>
        private void SmoothIteration()
        {
            // compute the area, normal, and centeroid for each triangle
            for (int i = 0; i < m_triangles.Length; i++)
            {
                Triangle tri = m_triangles[i];
                Vector3 v0 = m_vertices[tri.index0];
                Vector3 v1 = m_vertices[tri.index1];
                Vector3 v2 = m_vertices[tri.index2];

                Vector3 e0 = v1 - v0;
                Vector3 e1 = v2 - v0;
                Vector3 cross = Vector3.Cross(e0, e1);

                m_triAreas[i] = cross.Length / 2;
                m_triNormals[i] = cross.Normalized();
                m_triCenteroids[i] = (v0 + v1 + v2) / 3;
            }

            // compute a normal for every triangle as an average of the normals
            // in its neighborhood weighted by each triangle's area
            for (int t = 0; t < m_triToNeighbors.Length; t++)
            {
                int[] neighborhood = m_triToNeighbors[t];
                Vector3 normal = Vector3.Zero;

                for (int n = 0; n < neighborhood.Length; n++)
                {
                    int triIndex = neighborhood[n];
                    float area = m_triAreas[triIndex];

                    // if the triangle has no area the normal is NAN
                    if (area > 0)
                    {
                        normal += area * m_triNormals[triIndex];
                    }
                }
                m_triNeighborhoodNormals[t] = normal.Normalized();
            }

            // move all vertices based on the neighborhood normals of each adjacent triangle
            for (int v = 0; v < m_vertices.Length; v++)
            {
                Vector3 vertex = m_vertices[v];

                int[] neighborhood = m_vertToNeighbors[v];
                float nArea = 0;
                Vector3 vDisp = Vector3.Zero;

                for (int n = 0; n < neighborhood.Length; n++)
                {
                    int triIndex = neighborhood[n];
                    float area = m_triAreas[triIndex];
                    Vector3 normal = m_triNeighborhoodNormals[triIndex];
                    Vector3 vertToCenter = m_triCenteroids[triIndex] - vertex;

                    nArea += area;
                    vDisp += area * Vector3.Dot(vertToCenter, normal) * normal;
                }

                vDisp *= (m_strength / nArea);

                // no not modify the z-coordinates as they should not change
                m_vertices[v] += new Vector3(vDisp.X, vDisp.Y, 0);
            }
        }

        /// <summary>
        /// Gets a nicely formatted string represeting the smoother.
        /// </summary>
        public override string ToString()
        {
            return $"Mean Smoother, Iterations:{Iterations}, Strength:{Strength}";
        }
    }
}
