using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public class ButtonIcon : Button, IVisualElement, IClickable, IPollable
    {
        Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                Background.Position = position;
            }
        }

        Point bounds;
        public Point Bounds
        {
            get => bounds;
            set
            {
                bounds = value;
                Background.Bounds = bounds;
            }
        }

        public bool IsActive = true;

        public event OnButtonClick OnClickedEvent;
        public ActionGroup Group { get; private set; }

        public bool HighlightOnMouseOver;
        public Texture2D BaseTexture;
        public Texture2D HighlightTexture;

        Icon Background;

        public ButtonIcon(ActionGroup group)
        {
            Background = new Icon();
            Group = group;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);
        }
        public ButtonIcon(int width, int height, ActionGroup group)
        {
            Background = new Icon();
            Bounds = new Point(width, height);
            Position = Vector2.Zero;

            Group = group;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);
        }
        public ButtonIcon(int width, int height, Vector2 position, ActionGroup group)
        {
            Background = new Icon();
            Bounds = new Point(width, height);
            Position = position;

            Group = group;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);
        }
        public ButtonIcon(int width, int height, UILookupKey BaseKey, ActionGroup group) : this(width, height, group)
        {
            BaseTexture = GlobalRenderingData.TextureLookup[BaseKey];
        }

        public ButtonIcon(int width, int height, UILookupKey BaseKey, UILookupKey HighlightKey, ActionGroup group) : this(width, height, group)
        {
            BaseTexture = GlobalRenderingData.TextureLookup[BaseKey];
            HighlightTexture = GlobalRenderingData.TextureLookup[HighlightKey];
            HighlightOnMouseOver = true;
        }

        public ButtonIcon(int width, int height, UILookupKey BaseKey, Vector2 position, ActionGroup group) : this(width, height, position, group)
        {
            BaseTexture = GlobalRenderingData.TextureLookup[BaseKey];
        }

        public ButtonIcon(int width, int height, UILookupKey BaseKey, UILookupKey HighlightKey, Vector2 position, ActionGroup group) : this(width, height, position, group)
        {
            BaseTexture = GlobalRenderingData.TextureLookup[BaseKey];
            HighlightTexture = GlobalRenderingData.TextureLookup[HighlightKey];
            HighlightOnMouseOver = true;
        }

        void IClickable.Clicked()
        {
            OnClickedEvent?.Invoke(this);
        }

        void IClickable.ClickedAway()
        {

        }

        public bool IsMouseOver()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + bounds.Y);
        }

        public void PollInput(bool IsInActionGroupFrame)
        {
            if (IsInActionGroupFrame && HighlightOnMouseOver && IsMouseOver())
            {
                if (HighlightTexture != null) Background.DrawTexture = HighlightTexture;
            }
            else
            {
                if (BaseTexture != null) Background.DrawTexture = BaseTexture;
            }
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);
            }
        }
    }
}
