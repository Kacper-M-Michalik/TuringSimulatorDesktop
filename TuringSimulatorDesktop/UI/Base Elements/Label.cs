using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using FontStashSharp;
using FontStashSharp.RichText;

namespace TuringSimulatorDesktop.UI
{
    public class Label : IVisualElement, ICanvasInteractable
    {
        Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                Vector2 RoundedPosition = new Vector2(MathF.Round(value.X), MathF.Round(value.Y - RichText.Size.Y * 0.5f));

                position = RoundedPosition + new Vector2(0, RichText.Size.Y * 0.5f);
                Background.Position = RoundedPosition;
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

        //Richtext interprets special markup/foramtting when being rendered, which is useful in making text on a label prettier for the user
        public RichTextLayout RichText = new RichTextLayout() { };
        RenderTarget2D RenderTexture;
        public FontSystem Font = GlobalInterfaceData.StandardRegularFont;
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

        public void SetProjectionMatrix(Matrix projectionMatrix, Matrix inverseProjectionMatrix)
        {
            Background.SetProjectionMatrix(projectionMatrix, inverseProjectionMatrix);
        }

        public Label(int width, int height)
        {
            Background = new Icon();
            RichText.VerticalSpacing = 0;

            Bounds = new Point(width, height);
            Position = Vector2.Zero;
        }
        public Label(int width, int height, FontSystem font)
        {
            Background = new Icon();
            RichText.VerticalSpacing = 0;
            Font = font;

            Bounds = new Point(width, height);
            Position = Vector2.Zero;
        }
        public Label(int width, int height, Vector2 position, FontSystem font)
        {
            Background = new Icon();
            RichText.VerticalSpacing = 0;
            Font = font;

            Bounds = new Point(width, height);
            Position = position;
        }
        public Label(int width, int height, Vector2 position)
        {
            Background = new Icon();
            RichText.VerticalSpacing = 0;

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

        //Resizes output texture and redraws text
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

        //Draws text contained by label to a texture that is rendered to the user
        public void DrawTextToTexture()
        {
            Background.Position = new Vector2(MathF.Round(position.X), MathF.Round(position.Y - RichText.Size.Y * 0.5f));

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
        
        //Resizes the text texture if necessary
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
