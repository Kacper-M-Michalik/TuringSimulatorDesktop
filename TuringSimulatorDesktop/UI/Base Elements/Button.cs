using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public delegate void OnButtonClick(Button Sender);

    public class Button : IVisualElement, IClickable, IPollable
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool IsActive = true;

        public bool HighlightOnMouseOver;
        public Texture2D BaseTexture;
        public Texture2D HighlightTexture;

        public UIMesh Background { get; private set; }

        Vector2 position;
        public Vector2 Position { get => position; set { position = value; Background.MeshTransformations = Matrix.CreateWorld(new Vector3(position, 0), Vector3.Forward, Vector3.Up); } }
        public Vector2 GetBounds { get => new Vector2(Width, Height); }

        public event OnButtonClick OnClickedEvent;
        public ActionGroup Group { get; private set; }

        Button(int width, int height, Vector2 position, ActionGroup group)
        {
            Background = UIMesh.CreateRectangle(Vector2.Zero, width, height);

            Width = width;
            Height = height;
            Position = position;

            Group = group;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);
        }

        public Button(int width, int height, UILookupKey BaseKey, Vector2 position, ActionGroup group) : this(width, height, position, group)
        {
            BaseTexture = GlobalInterfaceData.TextureLookup[BaseKey];
        }

        public Button(int width, int height, UILookupKey BaseKey, UILookupKey HighlightKey, Vector2 position, ActionGroup group) : this(width, height, position, group)
        {
            BaseTexture = GlobalInterfaceData.TextureLookup[BaseKey];
            HighlightTexture = GlobalInterfaceData.TextureLookup[HighlightKey];
            HighlightOnMouseOver = true;
        }

        public void UpdateSize(int SetWidth, int SetHeight)
        {
            Width = SetWidth;
            Height = SetHeight;
            Background.UpdateMesh(UIMesh.CreateRectangle(Vector2.Zero, Width, Height));
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
            if (IsInActionGroupFrame && HighlightOnMouseOver && IsMouseOver())
            {
                //if (HighlightTexture == null) Background.OverlayColor = GlobalInterfaceData.UIOverlayDebugColor2;
                //else Background.Texture = HighlightTexture;
                if (HighlightTexture != null) Background.Texture = HighlightTexture;
            }
            else
            {
                if (BaseTexture != null) Background.Texture = BaseTexture;
                //if (BaseTexture == null) Background.OverlayColor = GlobalInterfaceData.UIOverlayDebugColor1;
                // else Background.Texture = BaseTexture;
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
