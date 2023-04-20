using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public enum ClickType { Left, Right, Both };

    public class TextureButton : Button, IVisualElement, IClickable, IPollable
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

        public bool IsActive { get; set; } = true;

        public event OnButtonClick OnClickedEvent;
        public event OnButtonClickAway OnClickedAwayEvent;
        public ActionGroup Group { get; private set; }
        public bool IsMarkedForDeletion { get; set; }

        public bool HighlightOnMouseOver;
        public Texture2D BaseTexture;
        public Texture2D HighlightTexture;
        public ClickType ClickListenType = ClickType.Left;

        Icon Background;

        public TextureButton(ActionGroup group)
        {
            Background = new Icon();
            Group = group;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);

            Bounds = new Point(10, 10);
            Position = Vector2.Zero;
        }
        public TextureButton(int width, int height, ActionGroup group)
        {
            Background = new Icon();
            Bounds = new Point(width, height);
            Position = Vector2.Zero;

            Group = group;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);
        }
        public TextureButton(int width, int height, Vector2 position, ActionGroup group)
        {
            Background = new Icon();
            Bounds = new Point(width, height);
            Position = position;

            Group = group;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);
        }

        void IClickable.Clicked()
        {
            if ((ClickListenType == ClickType.Both) || (ClickListenType == ClickType.Left && InputManager.LeftMousePressed) || (ClickListenType == ClickType.Right && InputManager.RightMousePressed)) 
                OnClickedEvent?.Invoke(this);
        }

        void IClickable.ClickedAway()
        {
            OnClickedAwayEvent?.Invoke(this);
        }

        public bool IsMouseOver()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X < Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y < Position.Y + bounds.Y);
        }

        public void PollInput(bool IsInActionGroupFrame)
        {
            if (IsInActionGroupFrame && HighlightOnMouseOver && IsMouseOver())
            {
                //Draws debug color if no texture applied
                if (HighlightTexture != null) Background.DrawTexture = HighlightTexture;
                else Background.DrawColor = GlobalInterfaceData.Scheme.UIOverlayDebugColor3;
            }
            else
            {
                if (BaseTexture != null) Background.DrawTexture = BaseTexture;
                else Background.DrawColor = GlobalInterfaceData.Scheme.UIOverlayDebugColor2;
            }
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);
            }
        }

        public void Close()
        {
            Group.IsDirtyClickable = true;
            Group.IsDirtyPollable = true;
            IsMarkedForDeletion = true;
        }
    }
}
