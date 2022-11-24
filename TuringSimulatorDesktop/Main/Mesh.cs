using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.UI;

namespace TuringSimulatorDesktop
{
    public class Mesh : IRenderable
    {
        public Matrix MeshTransformations;

        public VertexPositionColorTexture[] Vertices;
        public int[] Indices;

        public Texture2D Texture;

        public Mesh()
        {
            MeshTransformations = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
        }
        public Mesh(VertexPositionColorTexture[] SetVertices, int[] SetIndices, Texture2D SetTexture = null)
        {
            Vertices = SetVertices;
            Indices = SetIndices;
            Texture = SetTexture;

            MeshTransformations = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
        }

        public Mesh GetMesh()
        {
            return this;
        }

        public float GetFurthestRightVertexPoint()
        {
            float FurthestPoint = 0f;
            for (int i = 0; i < Vertices.Length; i++)
            {
                if (Vertices[i].Position.X > FurthestPoint) FurthestPoint = Vertices[i].Position.X;
            }
            return FurthestPoint;
        }

        public float GetFurthestDownVertexPoint()
        {
            float FurthestPoint = 0f;
            for (int i = 0; i < Vertices.Length; i++)
            {
                if (Vertices[i].Position.Y > FurthestPoint) FurthestPoint = Vertices[i].Position.Y;
            }
            return FurthestPoint;
        }

        public static Mesh CreateRectangle(Vector2 Offset, float Width, float Height, Color BackgroundColor)
        {
            Mesh Data = new Mesh();

            Data.Vertices = new VertexPositionColorTexture[]
            {
                new VertexPositionColorTexture(new Vector3(Offset.X, Offset.Y + Height, 0f), BackgroundColor, Vector2.Zero),
                new VertexPositionColorTexture(new Vector3(Offset.X, Offset.Y, 0f), BackgroundColor, Vector2.UnitY),
                new VertexPositionColorTexture(new Vector3(Offset.X + Width, Offset.Y, 0f), BackgroundColor, Vector2.One),
                new VertexPositionColorTexture(new Vector3(Offset.X + Width, Offset.Y + Height, 0f), BackgroundColor, Vector2.UnitX),
            };

            Data.Indices = new int[]
            {
                0, 1, 2, 0, 2, 3
            };

            return Data;
        }

    }
}
