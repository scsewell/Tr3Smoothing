using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Renderer
{
    public sealed class BatchedVertexSurface<TVertexData> : Surface where TVertexData : struct, IVertexData
    {
        public class Batch
        {
            public VertexBuffer<TVertexData> VertexBuffer { get; private set; }

            private readonly List<SurfaceSetting> m_settingsSet = new List<SurfaceSetting>();
            private readonly List<SurfaceSetting> m_settingsUnSet = new List<SurfaceSetting>();

            public bool NeedsUploading { get; private set; }

            public Batch(VertexBuffer<TVertexData> buffer)
            {
                VertexBuffer = buffer ?? new VertexBuffer<TVertexData>();
            }

            public void MarkAsDirty()
            {
                NeedsUploading = true;
            }

            public void BufferData()
            {
                VertexBuffer.BufferData();
                NeedsUploading = false;
            }

            /// <summary>
            /// Adds <see cref="SurfaceSetting"/>s to this batch.
            /// </summary>
            /// <param name="settings">The settings.</param>
            public void AddSettings(IEnumerable<SurfaceSetting> settings)
            {
                foreach (SurfaceSetting setting in settings)
                {
                    AddSetting(setting);
                }
            }

            /// <summary>
            /// Adds <see cref="SurfaceSetting"/>s to this batch.
            /// </summary>
            /// <param name="settings">The settings.</param>
            public void AddSettings(params SurfaceSetting[] settings)
            {
                foreach (SurfaceSetting setting in settings)
                {
                    AddSetting(setting);
                }
            }

            /// <summary>
            /// Adds a <see cref="SurfaceSetting"/> to this batch.
            /// </summary>
            /// <param name="setting">The setting.</param>
            public void AddSetting(SurfaceSetting setting)
            {
                m_settingsSet.Add(setting);
                if (setting.NeedsUnsetting)
                {
                    m_settingsUnSet.Add(setting);
                }
            }

            /// <summary>
            /// Removes a <see cref="SurfaceSetting"/> from this batch.
            /// </summary>
            /// <param name="setting">The setting.</param>
            public void RemoveSetting(SurfaceSetting setting)
            {
                if (m_settingsSet.Remove(setting) && setting.NeedsUnsetting)
                {
                    m_settingsUnSet.Remove(setting);
                }
            }

            /// <summary>
            /// Removes all <see cref="SurfaceSetting"/>s from this batch.
            /// </summary>
            public void ClearSettings()
            {
                m_settingsSet.Clear();
                m_settingsUnSet.Clear();
            }

            public void SetAllSettings(ShaderProgram program)
            {
                foreach (SurfaceSetting setting in m_settingsSet)
                {
                    setting.Set(program);
                }
            }

            public void UnsetAllSettings(ShaderProgram program)
            {
                foreach (SurfaceSetting setting in m_settingsUnSet)
                {
                    setting.UnSet(program);
                }
            }
        }

        private struct BatchContainer
        {
            public readonly Batch Batch;
            public readonly VertexArray<TVertexData> VertexArray;

            private BatchContainer(Batch batch, VertexArray<TVertexData> vertexArray)
            {
                VertexArray = vertexArray;
                Batch = batch;
            }

            public static BatchContainer Make(VertexBuffer<TVertexData> buffer)
            {
                var batch = new Batch(buffer);
                return new BatchContainer(batch, new VertexArray<TVertexData>(batch.VertexBuffer));
            }

            public void Delete()
            {
                VertexArray.Dispose();
                Batch.ClearSettings();
                Batch.VertexBuffer.Clear();
                Batch.VertexBuffer.Dispose();
            }
        }

        private List<BatchContainer> m_inactiveBatches = new List<BatchContainer>();
        private List<BatchContainer> m_activeBatches = new List<BatchContainer>();
        private readonly Stack<BatchContainer> m_unusedBatches = new Stack<BatchContainer>();

        private readonly PrimitiveType primitiveType;

        public int ActiveBatches { get { return m_activeBatches.Count; } }

        public BatchedVertexSurface(PrimitiveType primitiveType = PrimitiveType.Triangles)
        {
            this.primitiveType = primitiveType;
        }

        protected override void OnNewShaderProgram()
        {
            foreach (var container in m_activeBatches)
            {
                container.VertexArray.SetShaderProgram(m_program);
            }

            foreach (var container in m_inactiveBatches)
            {
                container.VertexArray.SetShaderProgram(m_program);
            }

            foreach (var container in m_unusedBatches)
            {
                container.VertexArray.SetShaderProgram(m_program);
            }
        }

        protected override void OnRender()
        {
            if (m_activeBatches.Count == 0)
            {
                return;
            }

            foreach (var batch in m_activeBatches)
            {
                if (batch.Batch.VertexBuffer.Count == 0)
                {
                    continue;
                }

                batch.Batch.SetAllSettings(m_program);

                GL.BindBuffer(BufferTarget.ArrayBuffer, batch.Batch.VertexBuffer);

                batch.VertexArray.SetVertexData();
                if (batch.Batch.NeedsUploading)
                {
                    batch.Batch.BufferData();
                }

                GL.DrawArrays(primitiveType, 0, batch.Batch.VertexBuffer.Count);

                batch.VertexArray.UnSetVertexData();

                batch.Batch.UnsetAllSettings(m_program);
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public Batch GetEmptyVertexBuffer(bool draw = true)
        {
            var batch = m_unusedBatches.Count > 0 ? m_unusedBatches.Pop() : BatchContainer.Make(null);

            addBatch(batch, draw);

            return batch.Batch;
        }

        public Batch AdoptVertexBuffer(VertexBuffer<TVertexData> buffer, bool draw = true)
        {
            var batch = BatchContainer.Make(buffer);

            addBatch(batch, draw);

            return batch.Batch;
        }

        private void addBatch(BatchContainer batch, bool draw)
        {
            if (m_program != null)
            {
                batch.VertexArray.SetShaderProgram(m_program);
            }

            (draw ? m_activeBatches : m_inactiveBatches).Add(batch);
        }

        public void ActivateVertexBuffer(Batch batch)
        {
            MoveBetweenLists(m_activeBatches, m_inactiveBatches, batch);
        }

        public void InactivateVertexBuffer(Batch batch)
        {
            MoveBetweenLists(m_inactiveBatches, m_activeBatches, batch);
        }

        private void MoveBetweenLists(List<BatchContainer> list, List<BatchContainer> list2, Batch batch)
        {
            var i = list.FindIndex(b => b.Batch == batch);
            var batchContainer = list[i];
            list.RemoveAt(i);

            list2.Add(batchContainer);
        }

        public void SwapActiveAndInactiveBuffers()
        {
            var temp = m_inactiveBatches;
            m_inactiveBatches = m_activeBatches;
            m_activeBatches = temp;
        }

        public void DeleteVertexBuffer(Batch batch, bool cacheForLater = true)
        {
            DeleteFromList(m_activeBatches, batch, cacheForLater);
        }

        public void DeleteInactiveVertexBuffer(Batch batch, bool cacheForLater = true)
        {
            DeleteFromList(m_inactiveBatches, batch, cacheForLater);
        }

        private void DeleteFromList(List<BatchContainer> list, Batch batch, bool cacheForLater)
        {
            var i = list.FindIndex(b => b.Batch == batch);
            var batchContainer = list[i];
            list.RemoveAt(i);
            if (cacheForLater)
            {
                batchContainer.Batch.ClearSettings();
                batchContainer.Batch.VertexBuffer.Clear();
                m_unusedBatches.Push(batchContainer);
            }
            else
            {
                batchContainer.Delete();
            }
        }
    }
}
