using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TuringSimulatorDesktop.UI
{
    public static class GlobalMeshRenderer
    {
        static GraphicsDevice Device;
        static Effect Effect;
        static Matrix Projection;

        public static Texture2D GlobalTexture;

        public static void Setup()
        {
            Device = GlobalInterfaceData.Device;
            Effect = GlobalInterfaceData.UIEffect;
        }

        public static void RecalculateProjection(int X, int Y, int Width, int Height)
        {
            Projection = Matrix.CreateOrthographicOffCenter(X, X + Width, Y + Height, Y, 0f, 1f);
        }

        /*
        public static void Draw(UIMesh DrawMesh, Viewport? Port = null)
        {
            Draw(new List<UIMesh>(1) { DrawMesh }, Port);
        }

        public static void Draw(List<UIMesh> MeshList, Viewport? Port = null)
        {
            Viewport port;
            if (Port == null) port = GlobalRenderingData.FullscreenViewport;
            else port = Port.Value;

            Device.Viewport = port;
            RecalculateProjection(port.X, port.Y, port.Width, port.Height);

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
                

                Effect.Parameters["OverlayColor"].SetValue(RenderObject.OverlayColor.ToVector4());
                Effect.Parameters["Projection"].SetValue(RenderObject.MeshTransformations * Projection);

                foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
                {
                    Pass.Apply();
                    Device.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, RenderObject.Vertices, 0, RenderObject.Vertices.Length, RenderObject.Indices, 0, RenderObject.Indices.Length / 3);
                }
            }
        }
        */


        //Not in use yet
        /*
        public static void Draw(MeshBuffer Buffer, Viewport Port)
        {
            Device.Viewport = Port;
            RecalculateProjection(Port.X, Port.Y, Port.Width, Port.Height);

            Effect.Parameters["HasBaseTexture"].SetValue(true);
            Effect.Parameters["BaseTextureSampler+BaseTexture"].SetValue(GlobalTexture);
            Effect.Parameters["OverlayColor"].SetValue(Vector4.Zero);
            Effect.Parameters["Projection"].SetValue(Projection);

            Mesh DrawMesh = Buffer.Finalise();

            foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
            {
                Pass.Apply();
                Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, DrawMesh.Vertices, 0, DrawMesh.Vertices.Length, DrawMesh.Indices, 0, DrawMesh.Indices.Length / 3);
            }
        }
        */

        public static void Draw(Mesh DrawMesh, Matrix Transformations, Texture2D DrawTexture, Viewport? Port = null)
        {
            Viewport port;
            if (Port == null) port = GlobalInterfaceData.FullscreenViewport;
            else port = Port.Value;

            Device.Viewport = port;
            RecalculateProjection(port.X, port.Y, port.Width, port.Height);

            Effect.Parameters["HasBaseTexture"].SetValue(true);
            Effect.Parameters["BaseTextureSampler+BaseTexture"].SetValue(DrawTexture);
            Effect.Parameters["OverlayColor"].SetValue(Vector4.Zero);
            Effect.Parameters["Projection"].SetValue(Transformations * Projection);

            foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
            {
                Pass.Apply();
                Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, DrawMesh.Vertices, 0, DrawMesh.Vertices.Length, DrawMesh.Indices, 0, DrawMesh.Indices.Length / 3);
            }
        }

        public static void Draw(Mesh DrawMesh, Matrix Transformations, Color DrawColor, Viewport? Port = null)
        {
            Viewport port;
            if (Port == null) port = GlobalInterfaceData.FullscreenViewport;
            else port = Port.Value;

            Device.Viewport = port;
            RecalculateProjection(port.X, port.Y, port.Width, port.Height);

            Effect.Parameters["HasBaseTexture"].SetValue(false);
            Effect.Parameters["OverlayColor"].SetValue(DrawColor.ToVector4());
            Effect.Parameters["Projection"].SetValue(Transformations * Projection);

            foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
            {
                Pass.Apply();
                Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, DrawMesh.Vertices, 0, DrawMesh.Vertices.Length, DrawMesh.Indices, 0, DrawMesh.Indices.Length / 3);
            }
        }
    }
}
