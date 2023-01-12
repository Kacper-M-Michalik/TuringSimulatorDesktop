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
        Vector2 position;
        public Vector2 Position 
        { 
            get => position; 
            set 
            { 
                position = value; 
                PositionMatrix = UIUtils.CreateTranslation(position); 
            } 
        }

        Point bounds;
        public Point Bounds
        {
            get => bounds;
            set
            {
                bounds = value;
                MeshData = Mesh.CreateRectangle(Vector2.Zero, bounds);
            }
        }

        public bool IsActive = true;

        Matrix PositionMatrix;
        public Mesh MeshData;
        public Color DrawColor;
        public Texture2D DrawTexture;

        public Icon() 
        {             
        }
        public Icon(Texture2D texture)
        {
            DrawTexture = texture;
        }
        public Icon(Color color)
        {
            DrawColor = color;
        }
        Icon(int width, int height)
        {            
            Bounds = new Point(width, height);
        }
        public Icon(int width, int height, Texture2D texture) : this(width, height)
        {
            DrawTexture = texture;
            Position = Vector2.Zero;
        }
        public Icon(int width, int height, Color color) : this(width, height)
        {
            DrawColor = color;
            Position = Vector2.Zero;
        }
        public Icon(int width, int height, Vector2 position, Texture2D texture) : this(width, height)
        {
            DrawTexture = texture;
            Position = position;
        }
        public Icon(int width, int height, Vector2 position, Color color) : this(width, height)
        {
            DrawColor = color;
            Position = position;
        }

        public void Draw(Viewport? BoundPort = null)
        {    
            if (IsActive)
            {
                if (DrawTexture != null) GlobalMeshRenderer.Draw(MeshData, PositionMatrix, DrawTexture, BoundPort);
                else GlobalMeshRenderer.Draw(MeshData, PositionMatrix, DrawColor, BoundPort);   
            }         
        }
    }
}
