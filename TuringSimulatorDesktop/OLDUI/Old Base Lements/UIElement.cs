using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TuringSimulatorDesktop.UI;

namespace TuringSimulatorDesktop.UI
{
    public abstract class UIElement
    {
        public Vector2 Position;

        /*
        Vector2 position;

        public Vector2 Position { 

            get { return position; } 

            set {
                position = value;
                if (MeshData != null) MeshData.MeshTransformations = Matrix.CreateWorld(new Vector3(position.X, position.Y, 0f), Vector3.Forward, Vector3.Up);
            }         
        
        }
        Mesh meshData;
        public Mesh MeshData 
        {
            get { return meshData; }
            set
            {
                meshData = value;
                MeshData.MeshTransformations = Matrix.CreateWorld(new Vector3(position.X, position.Y, 0f), Vector3.Forward, Vector3.Up);
            }        
        }
        */
    }
}
