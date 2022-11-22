using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TuringSimulatorDesktop
{
    public static class GlobalMeshRenderer
    {
        public static GraphicsDevice Device;
        public static BasicEffect Effect;

        public static void Setup(GraphicsDevice SetDevice, int Width, int Height)
        {
            Device = SetDevice;

            Effect = new BasicEffect(Device);
            Effect.TextureEnabled = true;
            Effect.FogEnabled = false;
            Effect.LightingEnabled = false;
            Effect.VertexColorEnabled = true;
            Effect.World = Matrix.Identity;
            Effect.View = Matrix.Identity;
            RecalculateProjection(0, 0, Width, Height);
        }

        public static void RecalculateProjection(int X, int Y, int Width, int Height)
        {
            Effect.Projection = Matrix.CreateOrthographicOffCenter(X, X + Width, Y + Height, Y, 0f, 1f);
        }

        public static void Draw(List<IRenderable> MeshList, Viewport Port)
        {
            Viewport OriginalPort = Device.Viewport;
            Device.Viewport = Port;
            RecalculateProjection(Port.X, Port.Y, Port.Width, Port.Height);

            foreach (IRenderable RenderObject in MeshList)
            {
                Mesh Data = RenderObject.GetMesh();

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
                    Device.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, Data.Vertices, 0, Data.Vertices.Length, Data.Indices, 0, Data.Indices.Length / 3);
                }
            }

            Device.Viewport = OriginalPort;
        }

        public static void Draw(List<Mesh> MeshList, Viewport Port)
        {
            Viewport OriginalPort = Device.Viewport;
            Device.Viewport = Port;

            foreach (Mesh Data in MeshList)
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
                    Device.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, Data.Vertices, 0, Data.Vertices.Length, Data.Indices, 0, Data.Indices.Length / 3);
                }
            }

            Device.Viewport = OriginalPort;
        }
    }
}
