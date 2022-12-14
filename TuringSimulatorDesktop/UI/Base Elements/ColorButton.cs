using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public class ColorButton : Button, IVisualElement, IClickable, IPollable
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
        public event OnButtonClickAway OnClickedAwayEvent;
        public ActionGroup Group { get; private set; }
        public bool IsMarkedForDeletion { get; set; }

        public bool HighlightOnMouseOver;
        public Color BaseColor;
        public Color HighlightColor;
        public ClickType ClickListenType = ClickType.Left;

        Icon Background;

        public ColorButton(ActionGroup group)
        {
            Background = new Icon();
            Group = group;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);
        }
        public ColorButton(int width, int height, ActionGroup group)
        {
            Background = new Icon();
            Bounds = new Point(width, height);
            Position = Vector2.Zero;

            Group = group;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);
        }
        public ColorButton(int width, int height, Vector2 position, ActionGroup group)
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
            if ((ClickListenType == ClickType.Left && InputManager.LeftMousePressed) || (ClickListenType == ClickType.Right && InputManager.RightMousePressed))
                OnClickedEvent?.Invoke(this);
        }

        void IClickable.ClickedAway()
        {
            OnClickedAwayEvent?.Invoke(this);
        }

        public bool IsMouseOver()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + bounds.Y);
        }

        public void PollInput(bool IsInActionGroupFrame)
        {
            if (IsInActionGroupFrame && HighlightOnMouseOver && IsMouseOver())
            {
                Background.DrawColor = HighlightColor;
            }
            else
            {
                Background.DrawColor = BaseColor;
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
