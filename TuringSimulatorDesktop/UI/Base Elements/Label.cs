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
                position = new Vector2(MathF.Round(value.X), MathF.Round(value.Y));
                Background.Position = position - new Vector2(0, RichText.Size.Y/2f);
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

        public bool IsActive { get; set; } = true;

        RichTextLayout RichText;
        RenderTarget2D RenderTexture;
        public FontSystem Font;
        public Color FontColor = GlobalInterfaceData.Scheme.FontColor;
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
        public bool DrawCentered = false;

        public Icon Background;
        public bool DrawFrame;

        public Label(int width, int height)
        {
            Background = new Icon();
            RichText = new RichTextLayout();
            RichText.VerticalSpacing = 0;
            Font = GlobalInterfaceData.StandardRegularFont;

            Bounds = new Point(width, height);
            Position = Vector2.Zero;
        }
        public Label(int width, int height, FontSystem font)
        {
            Background = new Icon();
            RichText = new RichTextLayout();
            RichText.VerticalSpacing = 0;
            Font = font;

            Bounds = new Point(width, height);
            Position = Vector2.Zero;
        }
        public Label(int width, int height, Vector2 position, FontSystem font)
        {
            Background = new Icon();
            RichText = new RichTextLayout();
            RichText.VerticalSpacing = 0;
            Font = font;

            Bounds = new Point(width, height);
            Position = position;
        }
        public Label(int width, int height, Vector2 position)
        {
            Background = new Icon();
            RichText = new RichTextLayout();
            RichText.VerticalSpacing = 0;
            Font = GlobalInterfaceData.StandardRegularFont;

            Bounds = new Point(width, height);
            Position = position;
        }
        public Label() : this(0, 0)
        {
            AutoSizeMesh = true;
            Background.DrawColor = GlobalInterfaceData.Scheme.UIOverlayDebugColor4;
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
            GlobalInterfaceData.Device.SetRenderTarget(RenderTexture);
            GlobalInterfaceData.Device.Clear(Color.Transparent);

            GlobalInterfaceData.TextBatch.Begin();
            if (DrawCentered)
            {
                RichText.Draw(GlobalInterfaceData.TextBatch, new Vector2(MathF.Round(bounds.X/2f - RichText.Size.X/2f), 0), FontColor);
            }
            else
            {
                RichText.Draw(GlobalInterfaceData.TextBatch, new Vector2(0, 0), FontColor);
            }
            GlobalInterfaceData.TextBatch.End();

            GlobalInterfaceData.Device.SetRenderTarget(null);

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
                RenderTexture = new RenderTarget2D(GlobalInterfaceData.Device, bounds.X, bounds.Y);
                return true;
            }
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                if (DrawFrame) Background.DrawTexture = null;
                else Background.DrawTexture = RenderTexture;
                Background.Draw(BoundPort);          
            }
        }

    }
}
