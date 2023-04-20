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
    public class ColorButton : Button, IVisualElement, IClickable, IPollable, ICanvasInteractable
    {
        //Update elements/properties on reposition/resize
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

        //For canvas placement
        public void SetProjectionMatrix(Matrix projectionMatrix, Matrix inverseProjectionMatrix)
        {
            ProjectionMatrix = projectionMatrix;
            InverseProjectionMatrix = inverseProjectionMatrix;
            Background.SetProjectionMatrix(projectionMatrix, InverseProjectionMatrix);
        }

        Matrix ProjectionMatrix = Matrix.Identity;
        Matrix InverseProjectionMatrix = Matrix.Identity;
        Matrix WorldSpaceMatrix = Matrix.Identity;

        public bool IsActive { get; set; } = true;

        public event OnButtonClick OnClickedEvent;
        public event OnButtonClickAway OnClickedAwayEvent;
        public ActionGroup Group { get; private set; }
        public bool IsMarkedForDeletion { get; set; }

        public bool HighlightOnMouseOver;
        public Color BaseColor;
        public Color HighlightColor;
        public ClickType ClickListenType = ClickType.Left;

        Icon Background;

        //Constructors
        public ColorButton(ActionGroup group)
        {
            Group = group;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);

            Background = new Icon();

            Bounds = Point.Zero;
            Position = Vector2.Zero;
        }
        public ColorButton(int width, int height, ActionGroup group)
        {
            Group = group;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);

            Background = new Icon();

            Bounds = new Point(width, height);
            Position = Vector2.Zero;
        }
        public ColorButton(int width, int height, Vector2 position, ActionGroup group)
        {
            Group = group;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);

            Background = new Icon();

            Bounds = new Point(width, height);
            Position = position;
        }

        //Can only be visible through IClickable interface 
        void IClickable.Clicked()
        {
            if (ClickListenType == ClickType.Both || (ClickListenType == ClickType.Left && InputManager.LeftMousePressed) || (ClickListenType == ClickType.Right && InputManager.RightMousePressed))
                OnClickedEvent?.Invoke(this);
        }

        void IClickable.ClickedAway()
        {
            OnClickedAwayEvent?.Invoke(this);
        }

        public bool IsMouseOver()
        {
            Vector3 MousePosition = (InputManager.MousePositionMatrix * InverseProjectionMatrix).Translation;
            return (IsActive && MousePosition.X >= Position.X && MousePosition.X <= Position.X + bounds.X && MousePosition.Y >= Position.Y && MousePosition.Y <= Position.Y + bounds.Y);
        }

        //Draws highlight color when mouse hovers over button
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

        //Bound port refers to a bounding area the element may be drawn in, were any parts of the element outside of his area/mask is not drawn
        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);
            }
        }

        //Sets action group to be evaluated by inputManager, marks itself for deletion
        public void Close()
        {
            Group.IsDirtyClickable = true;
            Group.IsDirtyPollable = true;
            IsMarkedForDeletion = true;
        }
    }
}
