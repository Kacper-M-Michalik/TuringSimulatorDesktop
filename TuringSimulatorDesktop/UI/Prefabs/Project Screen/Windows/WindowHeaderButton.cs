using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class WindowHeaderButton : IVisualElement, IPollable, IClickable
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

        public IView View;
        Window Window;

        Icon Background;
        Label Title;
        Button CloseButton;

        public WindowHeaderButton(IView view, Window window, ActionGroup group)
        {
            View = view;
            Window = window;
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
            Window.SetView(this);
        }

        public void ClickedAway()
        {

        }

        void Remove(Button Sender)
        {
            Window.RemoveView(this);
        }

        public bool IsMouseOver()
        {
            return (InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + bounds.Y);
        }

        public void PollInput(bool IsInActionGroupFrame)
        {
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
            Background.Draw();
            Title.Draw();
            CloseButton.Draw();
        }

    }
}
