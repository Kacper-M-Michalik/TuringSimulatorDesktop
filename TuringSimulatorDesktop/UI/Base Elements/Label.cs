using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using FontStashSharp;
using FontStashSharp.RichText;

namespace TuringSimulatorDesktop.UI
{
    public class Label : IVisualElement
    {
        //add resizing and editing later
        public int Width, Height;
        public bool IsActive = true;

        RichTextLayout RichText;
        public FontSystem Font;
        public string Text { get => RichText.Text; set { RichText.Text = value; UpdateTexture(); } }
        public float FontSize = 12f;
        public bool AutoSizeMesh = false;
        
        Vector2 position;
        public Vector2 Position { get => position; set { position = value; MeshData.MeshTransformations = Matrix.CreateWorld(new Vector3(position.X, position.Y, 0), Vector3.Forward, Vector3.Up); } }
        public Vector2 GetBounds { get => new Vector2(Width, Height); }

        public UIMesh MeshData;
        public RenderTarget2D RenderTexture;

        public Label(int width, int height, Vector2 position, FontSystem font)
        {
            Width = width;
            Height = height;            

            RichText = new RichTextLayout();
            Font = font;

            MeshData = UIMesh.CreateRectangle(Vector2.Zero, Width, Height, Color.Transparent);
            UpdateRenderTexture();

            Position = position;
        }
        public Label(Vector2 position, FontSystem font)
        {
            AutoSizeMesh = true;
            Width = 0;
            Height = 0;

            RichText = new RichTextLayout();
            Font = font;

            MeshData = UIMesh.CreateRectangle(Vector2.Zero, Width, Height, Color.Transparent);
            UpdateRenderTexture();

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
        
        public void UpdateTexture()
        {
            RichText.Font = Font.GetFont(FontSize);
            
            if (AutoSizeMesh)
            {
                Width = UIUtils.ConvertFloatToInt(RichText.Size.X);
                Height = UIUtils.ConvertFloatToInt(RichText.Size.Y);
                MeshData.UpdateMesh(UIMesh.CreateRectangle(Vector2.Zero, Width, Height));

                if (!UpdateRenderTexture()) return;                
            }
            
            GlobalInterfaceData.Device.SetRenderTarget(RenderTexture);
            GlobalInterfaceData.Device.Clear(Color.Transparent);

            GlobalInterfaceData.TextBatch.Begin();
            RichText.Draw(GlobalInterfaceData.TextBatch, Vector2.Zero, GlobalInterfaceData.FontColor);
            GlobalInterfaceData.TextBatch.End();

            GlobalInterfaceData.Device.SetRenderTarget(null);

            MeshData.Texture = RenderTexture;
        }
        
        public bool UpdateRenderTexture()
        {
            RenderTexture?.Dispose();
            if (Width == 0 || Height == 0)
            {
                return false;
            }
            else
            {
                RenderTexture = new RenderTarget2D(GlobalInterfaceData.Device, Width, Height);
                return true;
            }
        }
    }
}
