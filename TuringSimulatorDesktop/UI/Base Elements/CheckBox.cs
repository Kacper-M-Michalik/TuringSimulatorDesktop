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
    public delegate void OnCheckBoxClick(CheckBox Sender);

    public class CheckBox : IVisualElement, IClickable, IPollable
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

        public event OnCheckBoxClick OnClickedEvent;
        public ActionGroup Group { get; private set; }
        public bool IsMarkedForDeletion { get; set; }
        public bool Checked;

        public bool HighlightOnMouseOver = true;
        public Texture2D BaseUncheckedTexture;
        public Texture2D BaseCheckedTexture;
        public Texture2D HighlightUncheckedTexture;
        public Texture2D HighlightCheckedTexture;

        Icon Background;

        public CheckBox(ActionGroup group)
        {
            Background = new Icon();
            Group = group;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);
        }
        public CheckBox(int width, int height, ActionGroup group)
        {
            Background = new Icon();
            Bounds = new Point(width, height);
            Position = Vector2.Zero;

            Group = group;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);
        }
        public CheckBox(int width, int height, Vector2 position, ActionGroup group)
        {
            Background = new Icon();
            Bounds = new Point(width, height);
            Position = position;

            Group = group;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);
        }

        public void Clicked()
        {
            Checked = !Checked;
            OnClickedEvent?.Invoke(this);
        }

        public void ClickedAway()
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
                if (Checked)
                {
                   // if (HighlightCheckedTexture == null) Background.OverlayColor = GlobalInterfaceData.UIOverlayDebugColor4;
                   Background.DrawTexture = HighlightCheckedTexture;
                }
                else
                {
                    // if (HighlightUncheckedTexture == null) Background.OverlayColor = GlobalInterfaceData.UIOverlayDebugColor2;
                    Background.DrawTexture = HighlightUncheckedTexture;
                }
                
            }
            else
            {
                if (Checked)
                {
                    //if (BaseCheckedTexture == null) Background.OverlayColor = GlobalInterfaceData.UIOverlayDebugColor3;
                    Background.DrawTexture = BaseCheckedTexture;
                }
                else
                {
                    //if (BaseUncheckedTexture == null) Background.OverlayColor = GlobalInterfaceData.UIOverlayDebugColor1;
                    Background.DrawTexture = BaseUncheckedTexture;
                }
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
