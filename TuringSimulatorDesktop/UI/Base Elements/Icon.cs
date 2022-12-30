using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public class Icon : IVisualElement
    {
        public int Width, Height;

        public Mesh MeshData;
        Matrix PositionMatrix;

        public Color DrawColor;
        public Texture2D DrawTexture;

        public Vector2 position;
        public Vector2 Position { get => position; set { position = value; PositionMatrix = Matrix.CreateWorld(new Vector3(position, 0), Vector3.Forward, Vector3.Up); } }

        public Vector2 GetBounds => new Vector2(Width, Height);

        public Icon(int width, int height, Vector2 position)
        {
            Width = width;
            Height = height;
            Position = position;
            MeshData = Mesh.CreateRectangle(Vector2.Zero, Width, Height);
        }
        public Icon(int width, int height, Vector2 position, Texture2D texture) : this(width, height, position)
        {
            DrawTexture = texture;
        }
        public Icon(int width, int height, Vector2 position, Color color) : this(width, height, position)
        {
            DrawColor = color;
        }

        public void Draw(Viewport BoundPort = default)
        {
            if (UIUtils.IsDefaultViewport(BoundPort))
            {
                if (DrawTexture != null) GlobalUIRenderer.Draw(MeshData, PositionMatrix, DrawTexture);
                else GlobalUIRenderer.Draw(MeshData, PositionMatrix, DrawColor);
            }
            else
            {
                if (DrawTexture != null) GlobalUIRenderer.Draw(MeshData, PositionMatrix, DrawTexture);
                else GlobalUIRenderer.Draw(MeshData, PositionMatrix, DrawColor);
            }
        }
    }
}
