using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public delegate void OnClick(Button Sender);

    public class Button : IVisualElement, IClickable, IPollable
    {
        //add resizing and editing later

        public int Width, Height;
        public bool IsActive = true;

        public bool HighlightOnMouseOver = true;
        public Texture2D BaseTexture;
        public Texture2D HighlightTexture;

        public UIMesh Background;

        Vector2 position;
        public Vector2 Position { get => position; set { position = value; Background.MeshTransformations = Matrix.CreateWorld(new Vector3(position.X, position.Y, 0), Vector3.Forward, Vector3.Up); } }
        public Vector2 GetBounds { get => new Vector2(Width, Height); }

        public event OnClick OnClickedEvent;

        public Button(int width, int height, Vector2 position, ActionGroup group)
        {
#if DEBUG
            Background = UIMesh.CreateRectangle(Vector2.Zero, width, height, GlobalInterfaceData.UIOverlayDebugColor);
#else
            MeshData = UIMesh.CreateRectangle(Vector2.Zero, width, height, Color.Transparent);
#endif
            Width = width;
            Height = height;
            Position = position;

            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);
        }

        public void Clicked()
        {
            OnClickedEvent?.Invoke(this);
        }

        public void ClickedAway()
        {

        }

        public bool IsMouseOver()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + Width && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + Height);
        }

        public void PollInput(bool IsInActionGroupFrame)
        {
            if (IsInActionGroupFrame && IsMouseOver())
            {
#if DEBUG
                if (HighlightTexture == null) Background.OverlayColor = GlobalInterfaceData.UIOverlayDebugColorHighlight;
                else Background.Texture = HighlightTexture;
#else
                MeshData.Texture = HighlightTexture;
#endif
            }
            else
            {
#if DEBUG
                if (BaseTexture == null) Background.OverlayColor = GlobalInterfaceData.UIOverlayDebugColor;
                else Background.Texture = BaseTexture;
#else
                MeshData.Texture = BaseTexture;
#endif
            }
        }

        public void Draw(Viewport BoundPort = default)
        {
            if (IsActive)
            {
                if (UIUtils.IsDefaultViewport(BoundPort))
                {
                    GlobalUIRenderer.Draw(Background);
                }
                else
                {
                    GlobalUIRenderer.Draw(Background, BoundPort);
                }
            }
        }
    }
}
