using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop
{
    public class MeshBuffer
    {
        public List<Mesh> Meshes;

        public Mesh Finalise()
        {
            List<VertexPositionTexture> Vertices = new List<VertexPositionTexture>(Meshes.Count * 4);
            List<int> Indices = new List<int>(Meshes.Count * 6);

            int IndiceOffset = 0;
            for (int i = 0; i < Meshes.Count; i++)
            {
                Vertices.AddRange(Meshes[i].Vertices);
                for (int j = 0; j < Meshes[i].Indices.Length; j++)
                {
                    Indices.Add(Meshes[i].Indices[j] + IndiceOffset);
                }
                IndiceOffset += Meshes[i].Vertices.Length;
            }

            return new Mesh(Vertices.ToArray(), Indices.ToArray());
        }
    }
}
