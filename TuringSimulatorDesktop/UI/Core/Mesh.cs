using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public class Mesh
    {        
        public VertexPositionTexture[] Vertices;
        //Indices represent triangle faces made of vertices
        public int[] Indices;

        public Mesh()
        {
        }
        public Mesh(VertexPositionTexture[] vertices, int[] indices)
        {
            
            Vertices = vertices;
            Indices = indices;
        }

        //Mark to compiler to inline thsi function
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mesh CreateRectangle(Vector2 Offset, float Width, float Height)
        {
            //Returns a rectangular mesh
            return new Mesh
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
                }
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mesh CreateRectangle(Vector2 Offset, Point Bounds)
        {
            return new Mesh
            (
                new VertexPositionTexture[]
                {
                     new VertexPositionTexture(new Vector3(Offset.X, Offset.Y + Bounds.Y, 0f), Vector2.UnitY),
                     new VertexPositionTexture(new Vector3(Offset.X, Offset.Y, 0f), Vector2.Zero),
                     new VertexPositionTexture(new Vector3(Offset.X + Bounds.X, Offset.Y, 0f), Vector2.UnitX),
                     new VertexPositionTexture(new Vector3(Offset.X + Bounds.X, Offset.Y + Bounds.Y, 0f), Vector2.One),
                },
                new int[]
                {
                     0, 1, 2, 0, 2, 3
                }
            );
        }
    }
}
