using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public class Mesh
    {
        public Matrix MeshTransformations;

        public VertexPositionColor[] Vertices;
        public int[] Indices;

        public Texture2D Texture;

        public Mesh()
        {
            MeshTransformations = Matrix.CreateWorld(new Vector3(0, 0, 0), Vector3.Forward, Vector3.Up);
        }
        public Mesh(VertexPositionColor[] SetVertices, int[] SetIndices, Texture2D SetTexture = null)
        {
            Vertices = SetVertices;
            Indices = SetIndices;
            Texture = SetTexture;

            MeshTransformations = Matrix.CreateWorld(new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, -1, 0));
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

            Data.Vertices = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(Offset.X, Offset.Y + Height, 0f), BackgroundColor),
                new VertexPositionColor(new Vector3(Offset.X, Offset.Y, 0f), BackgroundColor),
                new VertexPositionColor(new Vector3(Offset.X + Width, Offset.Y, 0f), BackgroundColor),
                new VertexPositionColor(new Vector3(Offset.X + Width, Offset.Y + Height, 0f), BackgroundColor),
            };

            Data.Indices = new int[]
            {
                0, 1, 2, 0, 2, 3
            };

            return Data;
        }

    }
}
