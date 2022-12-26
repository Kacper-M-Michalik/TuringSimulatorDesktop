using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TuringSimulatorDesktop
{
    public static class GlobalUIRenderer
    {
        public static GraphicsDevice Device;
        public static Effect Effect;
        static Matrix Projection;

        public static void Setup(GraphicsDevice SetDevice)
        {
            Device = SetDevice;
            Effect = GlobalInterfaceData.UIEffect;
        }

        public static void RecalculateProjection(int X, int Y, int Width, int Height)
        {
            Projection = Matrix.CreateOrthographicOffCenter(X, X + Width, Y + Height, Y, 0f, 1f);
        }

        public static void Draw(UIMesh DrawMesh)
        {
            Draw(new List<UIMesh>(1) { DrawMesh }, GlobalInterfaceData.FullscreenViewport);
        }

        public static void Draw(UIMesh DrawMesh, Viewport Port)
        {
            Draw(new List<UIMesh>(1) { DrawMesh }, Port);
        }

        public static void Draw(List<UIMesh> MeshList)
        {
            Draw(MeshList, GlobalInterfaceData.FullscreenViewport);
        }

        public static void Draw(List<UIMesh> MeshList, Viewport Port)
        {
            Device.Viewport = Port;
            RecalculateProjection(Port.X, Port.Y, Port.Width, Port.Height);

            foreach (UIMesh RenderObject in MeshList)
            {         
                if (RenderObject.Texture != null && RenderObject.DrawTexture)
                { 
                    Effect.Parameters["HasBaseTexture"].SetValue(true);
                    Effect.Parameters["BaseTextureSampler+BaseTexture"].SetValue(RenderObject.Texture);
                }
                else
                {
                    Effect.Parameters["HasBaseTexture"].SetValue(false);
                }

                /*
                if (RenderObject.OverlayTexture != null && RenderObject.DrawOverlayTexture)
                {
                    Effect.Parameters["HasOverlayTexture"].SetValue(true);
                    Effect.Parameters["OverlayTextureSampler+OverlayTexture"].SetValue(RenderObject.OverlayTexture);
                }
                else
                {
                    Effect.Parameters["HasOverlayTexture"].SetValue(false);
                }
                */

                Effect.Parameters["OverlayColor"].SetValue(RenderObject.OverlayColor.ToVector4());
                Effect.Parameters["Projection"].SetValue(RenderObject.MeshTransformations * Projection);

                foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
                {
                    Pass.Apply();
                    Device.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, RenderObject.Vertices, 0, RenderObject.Vertices.Length, RenderObject.Indices, 0, RenderObject.Indices.Length / 3);
                }
            }
        }

        /*
        public Texture2D RasteriseText()
        {

        }
        */
    }
}
