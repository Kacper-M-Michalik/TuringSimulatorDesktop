using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public class TextLabel : UIElement
    {
        RenderTarget2D RenderTexture;

        public TextLabel(string Text = "")
        {
            if (Text != "") SetText(Text);
        }

        public void SetText(string NewText)
        {
            Vector2 Size = GlobalGraphicsData.Font.MeasureString(NewText);
            MeshData = Mesh.CreateRectangle(Vector2.Zero, Size.X, Size.Y, Color.Blue);

            RenderTexture = new RenderTarget2D(GlobalGraphicsData.Device, Convert.ToInt32(MathF.Round(Size.X, MidpointRounding.AwayFromZero)), Convert.ToInt32(MathF.Round(Size.Y, MidpointRounding.AwayFromZero)));
            GlobalGraphicsData.Device.SetRenderTarget(RenderTexture);
            GlobalGraphicsData.Device.Clear(Color.Transparent);
            SpriteBatch Batch = new SpriteBatch(GlobalGraphicsData.Device);
            Batch.Begin();
            Batch.DrawString(GlobalGraphicsData.Font, NewText, Vector2.Zero, GlobalGraphicsData.FontColor);
            Batch.End();
            GlobalGraphicsData.Device.SetRenderTarget(null);

            MeshData.Texture = RenderTexture;
        }

        public override int GetBoundX => throw new NotImplementedException();

        public override int GetBoundY => throw new NotImplementedException();
    }
}
