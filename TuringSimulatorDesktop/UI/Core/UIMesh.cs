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
    public class UIMesh
    {
        public Matrix MeshTransformations;

        public VertexPositionTexture[] Vertices;
        public int[] Indices;

        public Color OverlayColor;

        public bool DrawTexture = true;
        public Texture2D Texture;

        //public bool DrawOverlayTexture = false;
        //public Texture2D OverlayTexture;

        public UIMesh()
        {
            MeshTransformations = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
        }
        public UIMesh(VertexPositionTexture[] SetVertices, int[] SetIndices, Color SetOverlayColor = default, Texture2D SetTexture = null)//, Texture2D SetOverlayTexture = null)
        {
            if (SetOverlayColor == default) SetOverlayColor = Color.Transparent;

            Vertices = SetVertices;
            Indices = SetIndices;

            OverlayColor = SetOverlayColor;
            Texture = SetTexture;
            //OverlayTexture = SetOverlayTexture;

            MeshTransformations = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
        }

        public void UpdateMesh(UIMesh Source)
        {
            Vertices = Source.Vertices;
            Indices = Source.Indices;
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

        public static UIMesh CreateRectangle(Vector2 Offset, float Width, float Height, Color SetOverlayColor = default, Texture2D SetTexture = null)//, Texture2D SetOverlayTexture = null)
        {
            return new UIMesh
            (
                new VertexPositionTexture[]
                {
                     new VertexPositionTexture(new Vector3(Offset.X, Offset.Y + Height, 0f), Vector2.UnitY),
                     new VertexPositionTexture(new Vector3(Offset.X, Offset.Y, 0f), Vector2.Zero),
                     new VertexPositionTexture(new Vector3(Offset.X + Width, Offset.Y, 0f), Vector2.UnitX),
                     new VertexPositionTexture(new Vector3(Offset.X + Width, Offset.Y + Height, 0f), Vector2.One),
                },
                new int[]
                {
                     0, 1, 2, 0, 2, 3
                },
                SetOverlayColor,
                SetTexture//,
                //SetOverlayTexture
            );
        }

    }
}
