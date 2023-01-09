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
        Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                Background.Position = position;
            }
        }

        Point bounds;
        public Point Bounds
        {
            get => bounds;
            set
            {
                bounds = value;
                Background.Bounds = bounds;
                UpdateRenderTexture();
                DrawTextToTexture();
            }
        }

        public bool IsActive = true;

        RichTextLayout RichText;
        RenderTarget2D RenderTexture;
        public FontSystem Font;
        public string Text 
        { 
            get => RichText.Text; 
            set 
            { 
                RichText.Text = value; 
                UpdateLabel();
            } 
        }
        public float FontSize = 12;
        public bool AutoSizeMesh = false;

        public Icon Background;

        public Label(int width, int height)
        {
            Background = new Icon();
            RichText = new RichTextLayout();
            Font = GlobalRenderingData.StandardRegularFont;

            Bounds = new Point(width, height);
            Position = Vector2.Zero;
        }
        public Label(int width, int height, FontSystem font)
        {
            Background = new Icon();
            RichText = new RichTextLayout();
            Font = font;

            Bounds = new Point(width, height);
            Position = Vector2.Zero;
        }public Label(int width, int height, Vector2 position, FontSystem font)
        {
            Background = new Icon();
            RichText = new RichTextLayout();
            Font = font;

            Bounds = new Point(width, height);
            Position = position;
        } public Label(int width, int height, Vector2 position)
        {
            Background = new Icon();
            RichText = new RichTextLayout();
            Font = GlobalRenderingData.StandardRegularFont;

            Bounds = new Point(width, height);
            Position = position;
        }
        public Label() : this(0, 0)
        {
            AutoSizeMesh = true;
        }
        public Label(FontSystem font) : this(0, 0, font)
        {
            AutoSizeMesh = true;
        }      
        public Label(Vector2 position) : this(0, 0, position)
        {
            AutoSizeMesh = true;
        }
        public Label(Vector2 position, FontSystem font) : this(0, 0, position, font)
        {
            AutoSizeMesh = true;
        }

        public void UpdateLabel()
        {
            RichText.Font = Font.GetFont(FontSize);            
            if (AutoSizeMesh)
            {
                Bounds = new Point(UIUtils.ConvertFloatToInt(RichText.Size.X), UIUtils.ConvertFloatToInt(RichText.Size.Y));    
                if (!UpdateRenderTexture()) return;                
            }
            DrawTextToTexture();
        }

        public void DrawTextToTexture()
        {
            GlobalRenderingData.Device.SetRenderTarget(RenderTexture);
            GlobalRenderingData.Device.Clear(Color.Transparent);

            GlobalRenderingData.TextBatch.Begin();
            RichText.Draw(GlobalRenderingData.TextBatch, Vector2.Zero, GlobalRenderingData.FontColor);
            GlobalRenderingData.TextBatch.End();

            GlobalRenderingData.Device.SetRenderTarget(null);

            Background.DrawTexture = RenderTexture;
        }
        
        public bool UpdateRenderTexture()
        {
            RenderTexture?.Dispose();
            if (bounds.X == 0 || bounds.Y == 0)
            {
                return false;
            }
            else
            {
                RenderTexture = new RenderTarget2D(GlobalRenderingData.Device, bounds.X, bounds.Y);
                return true;
            }
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);          
            }
        }
        
    }
}
