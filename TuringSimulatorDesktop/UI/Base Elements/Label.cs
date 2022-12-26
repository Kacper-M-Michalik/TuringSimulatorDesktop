using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using FontStashSharp;

namespace TuringSimulatorDesktop.UI
{
    public class Label : IVisualElement
    {
        //add resizing and editing later
        public int Width, Height;
        public bool IsActive = true;
        
        public string Text = "";
        public float FontSize = 12;
        public bool AutoSizeMesh = false;
        
        Vector2 position;
        public Vector2 Position { get => position; set { position = value; MeshData.MeshTransformations = Matrix.CreateWorld(new Vector3(position.X, position.Y, 0), Vector3.Forward, Vector3.Up); } }
        public Vector2 GetBounds { get => new Vector2(Width, Height); }

        public UIMesh MeshData;
        public RenderTarget2D RenderTexture;
        SpriteBatch Batch;

        public Label(int width, int height, Vector2 position)
        {
            MeshData = UIMesh.CreateRectangle(Vector2.Zero, width, height, Color.Transparent);
            RenderTexture = new RenderTarget2D(GlobalInterfaceData.Device, width, height);
            Batch = new SpriteBatch(GlobalInterfaceData.Device);

            Width = width;
            Height = height;
            Position = position;
        }

        public void Draw(Viewport BoundPort = default)
        {
            if (IsActive)
            {
                if (UIUtils.IsDefaultViewport(BoundPort))
                {
                    GlobalUIRenderer.Draw(MeshData);
                }
                else
                {
                    GlobalUIRenderer.Draw(MeshData, BoundPort);
                }
            }
        }
        
        //i think we could just rewwrite with direct draw call
        public void UpdateTexture()
        {
            SpriteFontBase Font = GlobalInterfaceData.Fonts.GetFont(FontSize);

            if (AutoSizeMesh)
            {
                Vector2 Size = Font.MeasureString(Text);
                Width = UIUtils.ConvertFloatToInt(Size.X);
                Height = UIUtils.ConvertFloatToInt(Size.Y);
                MeshData.UpdateMesh(UIMesh.CreateRectangle(Vector2.Zero, Width, Height));
                RenderTexture.Dispose();
                RenderTexture = new RenderTarget2D(GlobalInterfaceData.Device, Width, Height);
            }

            GlobalInterfaceData.Device.SetRenderTarget(RenderTexture);
            GlobalInterfaceData.Device.Clear(Color.Transparent);

            Batch.Begin();
            Batch.DrawString(Font, Text, Vector2.Zero, GlobalInterfaceData.FontColor);
            Batch.End();

            GlobalInterfaceData.Device.SetRenderTarget(null);

            MeshData.Texture = RenderTexture;
        }
        
    }
}
