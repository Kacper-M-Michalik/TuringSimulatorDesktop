using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public class Icon : IVisualElement, ICanvasInteractable
    {
        Vector2 position;
        public Vector2 Position 
        { 
            get => position; 
            set 
            { 
                position = value;
                WorldSpaceMatrix = Matrix.CreateTranslation(new Vector3(Position, 0));
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

        public void SetProjectionMatrix(Matrix projectionMatrix, Matrix inverseProjectionMatrix)
        {
            ProjectionMatrix = projectionMatrix;
        }

        public bool IsActive { get; set; } = true;

        Matrix ProjectionMatrix = Matrix.Identity;
        Matrix WorldSpaceMatrix = Matrix.Identity;
        public Mesh MeshData = Mesh.CreateRectangle(Vector2.Zero, new Point(10, 10));
        public Color DrawColor = GlobalInterfaceData.Scheme.UIOverlayDebugColor1;
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
                if (DrawTexture != null) GlobalMeshRenderer.Draw(MeshData, WorldSpaceMatrix * ProjectionMatrix, DrawTexture, BoundPort);
                else GlobalMeshRenderer.Draw(MeshData, WorldSpaceMatrix * ProjectionMatrix, DrawColor, BoundPort);   
            }         
        }
    }
}
