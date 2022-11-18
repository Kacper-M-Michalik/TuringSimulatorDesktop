using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public class MeshData
    {
        public VertexPositionColor[] Vertices;
        public int[] Indices;

        public MeshData(VertexPositionColor[] SetVertices, int[] SetIndices)
        {
            Vertices = SetVertices;
            Indices = SetIndices;
        }
    }

    public class MeshRenderer : IDisposable
    {
        bool IsDisposed;
        GraphicsDevice Device;
        public BasicEffect Effect;

        VertexPositionColor[] Vertices;
        int[] Indices;

        List<MeshData> Meshes;

        public MeshRenderer(GraphicsDevice SetDevice)
        {
            Device = SetDevice;
            Meshes = new List<MeshData>();

            Effect = new BasicEffect(Device);
            Effect.TextureEnabled = false;
            Effect.FogEnabled = false;
            Effect.LightingEnabled = false;
            Effect.VertexColorEnabled = true;
            Effect.World = Matrix.Identity;
            Effect.View = Matrix.Identity;
            Effect.Projection = Matrix.CreateOrthographicOffCenter(0, 100, 0, 100, 0f, 1f);
        }

        public void AddMesh(MeshData Data)
        {
            Meshes.Add(Data);
        }

        public void DeleteMesh(MeshData Data)
        {
            Meshes.Remove(Data);
        }

        public void FinaliseMeshAddtionsAndDeletions()
        {
            int TotalVertices = 0;
            int TotalIndices = 0;

            foreach (MeshData Data in Meshes)
            {
                TotalVertices += Data.Vertices.Length;
                TotalIndices += Data.Indices.Length;
            }

            Vertices = new VertexPositionColor[TotalVertices];
            Indices = new int[TotalIndices];
            int CurrentVertexIndex = 0;
            int CurrentIndiceIndex = 0;

            foreach (MeshData Data in Meshes)
            {
                Array.Copy(Data.Vertices, 0, Vertices, CurrentVertexIndex, Data.Vertices.Length);
                CurrentVertexIndex += Data.Vertices.Length;
                Array.Copy(Data.Indices, 0, Indices, CurrentIndiceIndex, Data.Indices.Length);
                CurrentIndiceIndex += Data.Indices.Length;                
            }
        }

        public void Draw()
        {
            foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
            {
                Pass.Apply();
                Device.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, Vertices, 0, Vertices.Length, Indices, 0, Indices.Length / 3);
            }
        }

        public void Dispose()
        {
            if (IsDisposed) return;

            Effect?.Dispose();
            IsDisposed = true;
        }
    }
}
