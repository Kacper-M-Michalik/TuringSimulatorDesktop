using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class WindowHeaderItem : IVisualElement, IPollable, IClickable
    {
        Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                Background.Position = position;
                Title.Position = new Vector2(position.X + 4, position.Y);
                CloseButton.Position = new Vector2(position.X + Title.Bounds.X + 4, position.Y);
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

        Icon Background;
        Label Title;
        Button CloseButton;

        public IView View;
        Window OwnerWindow;

        public WindowHeaderItem(IView view, Window window, ActionGroup group)
        {
            View = view;
            OwnerWindow = window;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);

            Background = new Icon(GlobalRenderingData.SubHeaderColor);
            Title = new Label();
            Title.Text = view.Title;

            CloseButton = new Button(group);
            CloseButton.BaseTexture = GlobalRenderingData.TextureLookup[UILookupKey.Debug1];
            CloseButton.Bounds = new Point(16, 16);
            CloseButton.OnClickedEvent += Remove;
            CloseButton.IsActive = false;

            Bounds = Title.Bounds + new Point(21, 0);
        }

        public void Clicked()
        {
            OwnerWindow.SetView(this);
        }

        public void ClickedAway()
        {

        }

        void Remove(Button Sender)
        {
            OwnerWindow.RemoveView(this);
        }

        public bool IsMouseOver()
        {
            return (InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + bounds.Y);
        }

        public void PollInput(bool IsInActionGroupFrame)
        {
            if (View.Title != Title.Text)
            {
                Title.Text = View.Title;
                Bounds = Title.Bounds + new Point(21, 0);
            }

            if (IsInActionGroupFrame && IsMouseOver())
            {
                CloseButton.IsActive = true;
                //do background highlight
            }
            else
            {
                CloseButton.IsActive = false;
            }
        }

        public void Draw(Viewport? BoundPort = null)
        {
            Background.Draw(BoundPort);
            Title.Draw(BoundPort);
            CloseButton.Draw(BoundPort);
        }

    }
}
