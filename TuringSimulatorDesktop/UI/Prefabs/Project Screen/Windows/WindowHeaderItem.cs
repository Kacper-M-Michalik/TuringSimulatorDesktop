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
                SelectionStrip.Position = new Vector2(position.X, position.Y + 24);
                Title.Position = new Vector2(position.X + 16, position.Y + 13);
                CloseButton.Position = new Vector2(position.X + bounds.X - 20, position.Y + 4);
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
                SelectionStrip.Bounds = new Point(bounds.X, 2);
                CloseButton.Position = new Vector2(position.X + bounds.X - 20, position.Y + 4);
            }
        }

        public bool IsActive { get; set; } = true;

        Icon Background;
        Icon SelectionStrip;
        Label Title;
        TextureButton CloseButton;
        ActionGroup Group;
        public bool IsMarkedForDeletion { get; set; }

        public IView View;
        Window OwnerWindow;
        bool Selected;

        public WindowHeaderItem(IView view, Window window, ActionGroup group)
        {
            View = view;
            OwnerWindow = window;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);
            Group = group;

            Background = new Icon(GlobalInterfaceData.Scheme.Header);
            SelectionStrip = new Icon(GlobalInterfaceData.Scheme.BrightAccent);
            SelectionStrip.IsActive = false;
            Title = new Label();
            Title.Font = GlobalInterfaceData.StandardRegularFont;
            Title.FontColor = GlobalInterfaceData.Scheme.FontColorBright;
            Title.Text = view.Title;

            CloseButton = new TextureButton(group);
            CloseButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.CloseIcon];
            CloseButton.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.CloseIconHighlight];
            CloseButton.HighlightOnMouseOver = true;
            CloseButton.Bounds = new Point(16, 16);
            CloseButton.OnClickedEvent += Remove;
            CloseButton.IsActive = false;

            Bounds = new Point(42 + Title.Bounds.X, 26);
        }

        public void Select()
        {
            Selected = true;
            SelectionStrip.IsActive = true;
            Title.Font = GlobalInterfaceData.MediumRegularFont;
            Title.UpdateLabel();
        }

        public void Deselect()
        {
            Selected = false;
            SelectionStrip.IsActive = false;
            Title.Font = GlobalInterfaceData.StandardRegularFont;
            Title.UpdateLabel();
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
                Bounds = new Point(42 + Title.Bounds.X, 26);
                OwnerWindow.UpdateHeader();
            }

            if (Selected || (IsInActionGroupFrame && IsMouseOver()))
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
            SelectionStrip.Draw(BoundPort);
            CloseButton.Draw(BoundPort);
        }

        public void Close()
        {
            Group.IsDirtyClickable = true;
            Group.IsDirtyPollable = true;
            IsMarkedForDeletion = true;
        }
    }
}
