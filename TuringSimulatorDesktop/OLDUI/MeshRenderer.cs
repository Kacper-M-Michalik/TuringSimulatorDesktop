using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TuringSimulatorDesktop.UI
{
    public class MeshRenderer //: IDisposable
    {
        /*
        private bool IsDisposed;
        private GraphicsDevice Device;
        public BasicEffect Effect;

        /*
        VertexPositionColor[] Vertices;
        int[] Indices;
        

        List<Mesh> Meshes;

        public MeshRenderer(GraphicsDevice SetDevice, int Width, int Height)
        {
            Device = SetDevice;
            Meshes = new List<Mesh>();

            Effect = new BasicEffect(Device);
            Effect.TextureEnabled = true;
            Effect.FogEnabled = false;
            Effect.LightingEnabled = false;
            Effect.VertexColorEnabled = true;
            Effect.World = Matrix.Identity;
            Effect.View = Matrix.Identity;
            RecalculateProjection(0, 0, Width, Height);
        }
        public void RecalculateProjection(int X, int Y, int Width, int Height)
        {
            Effect.Projection = Matrix.CreateOrthographicOffCenter(X, X + Width, Y + Height, Y, 0f, 1f);
        }

        public void AddMesh(Mesh Data)
        {
            Meshes.Add(Data);
        }

        public void DeleteMesh(Mesh Data)
        {
            Meshes.Remove(Data);
        }

        /*
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
        

        public void Draw()
        {
            foreach (Mesh Data in Meshes)
            {                
                if (Data.Texture != null)
                {
                    Effect.Texture = Data.Texture;
                    Effect.TextureEnabled = true;
                }
                else
                {
                    Effect.Texture = null;
                    Effect.TextureEnabled = false;
                }
                Effect.World = Data.MeshTransformations;

                foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
                {
                    Pass.Apply();
                    Device.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Data.Vertices, 0, Data.Vertices.Length, Data.Indices, 0, Data.Indices.Length / 3);
                }
            }

        }

        public void Dispose()
        {
            if (IsDisposed) return;

            Effect?.Dispose();
            Meshes.Clear();
            IsDisposed = true;
        }
        */
    }
}
