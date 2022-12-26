using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FontStashSharp;

namespace TuringSimulatorDesktop.UI
{
    public class TextLabel : UIElement, IDrawable
    {
        UIMesh MeshData;
        public int FontSize;
        public string Text;
        Vector2 Size;

        RenderTarget2D RenderTexture;

        Icon IconData;

        public TextLabel(Vector2 SetPosition, int SetFontSize = 12, string SetText = "")
        {
            Position = SetPosition;
            FontSize = SetFontSize;
            Text = SetText;

            if (Text != "") UpdateTexture();
            else MeshData = UIMesh.CreateRectangle(Vector2.Zero, 10, 10, Color.Purple);
        }

        public void UpdateTexture()
        {
            SpriteFontBase Font = GlobalInterfaceData.Fonts.GetFont(FontSize);

            Size = Font.MeasureString(Text);
            MeshData = UIMesh.CreateRectangle(Vector2.Zero, Size.X, Size.Y, Color.Purple);

            RenderTexture = new RenderTarget2D(GlobalInterfaceData.Device, ConvertFloatToValidInt(Size.X), ConvertFloatToValidInt(Size.Y));
            GlobalInterfaceData.Device.SetRenderTarget(RenderTexture);
            GlobalInterfaceData.Device.Clear(Color.Transparent);

            SpriteBatch Batch = new SpriteBatch(GlobalInterfaceData.Device);
            Batch.Begin();
            Batch.DrawString(Font, Text, Vector2.Zero, Color.White);//GlobalGraphicsData.FontColor);
            Batch.End();

            GlobalInterfaceData.Device.SetRenderTarget(null);

            MeshData.Texture = RenderTexture;
        }

        public int ConvertFloatToValidInt(float Value)
        {
            return Convert.ToInt32(Math.Clamp(MathF.Round(Value, MidpointRounding.AwayFromZero), 1f, float.PositiveInfinity));
        }

        public void Draw()
        {
            //GlobalMeshRenderer.Draw();
        }
    }
}
